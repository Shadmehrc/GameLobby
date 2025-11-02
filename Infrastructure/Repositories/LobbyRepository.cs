using Application.Interfaces.RepositoryInterfaces;
using Application.Model;
using Domain.Configuration;
using Domain.Entities;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Repositories;

public class LobbyRepository(IConnectionMultiplexer mux, IOptions<RedisOptions> opt, IOptions<LobbyConfigs> lobbyConfigs) : ILobbyRepository
{
    private readonly IDatabase _dbContext = mux.GetDatabase();
    private readonly string _namespace = (opt.Value.InstancePrefix) + ":";
    private readonly IOptions<LobbyConfigs> _lobbyConfigs = lobbyConfigs;


    private string Meta(long lobbyID) => $"{_namespace}lobby:{lobbyID}:meta";
    private string Members(long lobbyID) => $"{_namespace}lobby:{lobbyID}:members";
    private string OpenIndex => $"{_namespace}lobby:index:open";
    private string Seq => $"{_namespace}lobby:seq";
    private const string UnlockLua = @"if redis.call('GET', KEYS[1]) == ARGV[1] then return redis.call('DEL', KEYS[1]) else return 0 end";

    public async Task<Result<long>> CreateLobbyAsync()
    {
        var id = await _dbContext.StringIncrementAsync(Seq);
        await _dbContext.HashSetAsync(Meta(id), new HashEntry[]
        {
            new("status","open"),
            new("capacity",_lobbyConfigs.Value.Capacity)
        });
        await _dbContext.SetAddAsync(OpenIndex, Meta(id));
        return Result<long>.Ok(id, "Lobby created");
    }

    public async Task<Result<Lobby>> JoinLobbyAsync(long lobbyId, string playerId)
    {
        var meta = Meta(lobbyId);
        var members = Members(lobbyId);
        var lockKey = $"{_namespace}lobby:{lobbyId}:lock";
        var lockId = Guid.NewGuid().ToString("N");

        // acquire distributed lock (spin with jitter)
        var rnd = new Random();
        for (int i = 0; i < 40; i++)
        {
            if (await _dbContext.StringSetAsync(lockKey, lockId, TimeSpan.FromSeconds(3), When.NotExists))
                break;
            await Task.Delay(rnd.Next(20, 40));
            if (i == 39) return Result<Lobby>.Fail("Lobby is busy, try again", ErrorCode.Unknown);
        }

        try
        {
            var lobbyData = (await _dbContext.HashGetAllAsync(meta)).ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());

            if (lobbyData.Count == 0)
                return Result<Lobby>.Fail("Lobby not found", ErrorCode.NotFound);

            int lobbyCapacity = Convert.ToInt32(lobbyData.GetValueOrDefault("capacity"));


            if (await _dbContext.SetContainsAsync(members, playerId))
            {
                var lobby0 = await GetLobbyAsync(lobbyId) ?? new Lobby { Id = lobbyId };
                return Result<Lobby>.Fail( $"You Already joined the lobby with ID {lobbyId}");
            }

            var filledCapacity = (int)
            await _dbContext.SetLengthAsync(members);

            if (filledCapacity >= lobbyCapacity)
            {
                await _dbContext.HashSetAsync(meta, new HashEntry[] { new("status", "full")});
                return Result<Lobby>.Fail("Lobby is full", ErrorCode.Full);
            }

            Console.WriteLine($"Player {playerId} joined the lobby");
            await _dbContext.SetAddAsync(members, playerId);
            filledCapacity++;

            if (filledCapacity >= lobbyCapacity)
            {
                await _dbContext.HashSetAsync(meta, [new("status", "full")]);
            }

            var lobby = await GetLobbyAsync(lobbyId) ?? new Lobby { Id = lobbyId };
            return Result<Lobby>.Ok(lobby, $"You have joined the lobby with ID {lobbyId}");
        }
        finally
        {
            await _dbContext.ScriptEvaluateAsync(UnlockLua, new RedisKey[] { lockKey }, new RedisValue[] { lockId });
        }
    }
    public async Task<Lobby?> GetLobbyAsync(long lobbyID)
    {
        var meta = await _dbContext.HashGetAllAsync(Meta(lobbyID));
        var lobbyData = meta.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        if (lobbyData.Count==0)
        {
            throw new Exception("Lobby not found");
        }

        var memberCount = (int)await _dbContext.SetLengthAsync(Members(lobbyID));
        var result = new Lobby
        {
            Id = lobbyID,
            Capacity = int.Parse(lobbyData.GetValueOrDefault("capacity", "64")),
            Status = lobbyData.GetValueOrDefault("status") 
            switch
            {
                "open" => LobbyStatus.Open,
                "full" => LobbyStatus.Full,
                _ => LobbyStatus.Closed
            },
            MemberCount = memberCount
        };
        return result;
    }

}
