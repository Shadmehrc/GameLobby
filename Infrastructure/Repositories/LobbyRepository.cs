using Application.Interfaces.RepositoryInterfaces;
using Application.Model;
using Domain.Configuration;
using Domain.Entities;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Repositories;

public class LobbyRepository : ILobbyRepository
{
    private readonly IDatabase _db;
    private readonly string _ns;

    public LobbyRepository(IConnectionMultiplexer mux, IOptions<RedisOptions> opt)
    {
        _db = mux.GetDatabase();
        _ns = (opt.Value.InstancePrefix ?? "lobby").TrimEnd(':') + ":";
    }

    private string Meta(long id) => $"{_ns}lobby:{id}:meta";
    private string Members(long id) => $"{_ns}lobby:{id}:members";
    private string OpenIndex => $"{_ns}lobby:index:open";
    private string Seq => $"{_ns}lobby:seq";

    public async Task<Result<long>> CreateLobbyAsync()
    {
        var id = await _db.StringIncrementAsync(Seq);
        await _db.HashSetAsync(Meta(id), new HashEntry[]
        {
            new("status","open"),
            new("clusterLocked","0"),
            new("capacity","64")
        });
        await _db.SetAddAsync(OpenIndex, Meta(id));
        return Result<long>.Ok(id, "Lobby created");
    }

    private const string UnlockLua = @"if redis.call('GET', KEYS[1]) == ARGV[1] then return redis.call('DEL', KEYS[1]) else return 0 end";

    public async Task<Result<LobbyModel>> JoinLobbyAsync(long lobbyId, string playerId)
    {
        var meta = Meta(lobbyId);
        var members = Members(lobbyId);
        var lockKey = $"{_ns}lobby:{lobbyId}:lock";
        var lockId = Guid.NewGuid().ToString("N");

        // acquire distributed lock (spin with jitter)
        var rnd = new Random();
        for (int i = 0; i < 40; i++)
        {
            if (await _db.StringSetAsync(lockKey, lockId, TimeSpan.FromSeconds(3), When.NotExists))
                break;
            await Task.Delay(rnd.Next(20, 40));
            if (i == 39) return Result<LobbyModel>.Fail("Lobby is busy, try again", ErrorCode.Unknown);
        }

        try
        {
            var m = (await _db.HashGetAllAsync(meta)).ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
            //var entries = await _db.HashGetAllAsync(meta);
            //var m = new Dictionary<string, string>(entries.Length);
            //foreach (var e in entries)
            //    m[e.Name.ToString()] = e.Value.ToString();


            if (m.Count == 0)
                return Result<LobbyModel>.Fail("Lobby not found", ErrorCode.NotFound);

            if (m.GetValueOrDefault("clusterLocked") == "1")
                return Result<LobbyModel>.Fail("Cluster is locked", ErrorCode.Locked);

            if (!string.Equals(m.GetValueOrDefault("status"), "open", StringComparison.OrdinalIgnoreCase))
                return Result<LobbyModel>.Fail("Lobby is not open", ErrorCode.NotOpen);

            int lobbyCapacity = int.TryParse(m.GetValueOrDefault("capacity", "64"), out var c) ? c : 64;

            // idempotent
            if (await _db.SetContainsAsync(members, playerId))
            {
                var lobby0 = await GetLobbyAsync(lobbyId) ?? new LobbyModel { Id = lobbyId };
                return Result<LobbyModel>.Ok(lobby0, $"You have joined the lobby with ID {lobbyId}");
            }

            var filledCapacity = (int)
            await _db.SetLengthAsync(members);

            if (filledCapacity >= lobbyCapacity)
            {
                await _db.HashSetAsync(meta, new HashEntry[] { new("status", "full"), new("clusterLocked", "1") });
                return Result<LobbyModel>.Fail("Lobby is full", ErrorCode.Full);
            }

            // add member
            Console.WriteLine($"Player {playerId} joined the lobby");
            await _db.SetAddAsync(members, playerId);
            filledCapacity++;

            if (filledCapacity >= lobbyCapacity)
            {
                await _db.HashSetAsync(meta, [new("status", "full"), new("clusterLocked", "1")]);
            }

            var lobby = await GetLobbyAsync(lobbyId) ?? new LobbyModel { Id = lobbyId };
            return Result<LobbyModel>.Ok(lobby, $"You have joined the lobby with ID {lobbyId}");
        }
        finally
        {
            await _db.ScriptEvaluateAsync(UnlockLua, new RedisKey[] { lockKey }, new RedisValue[] { lockId });
        }
    }
    public async Task<LobbyModel?> GetLobbyAsync(long lobbyID)
    {
        var meta = await _db.HashGetAllAsync(Meta(lobbyID));
        var d = meta.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        var count = (int)await _db.SetLengthAsync(Members(lobbyID));
        return new LobbyModel
        {
            Id = lobbyID,
            Capacity = int.Parse(d.GetValueOrDefault("capacity", "64")),
            ClusterLocked = d.GetValueOrDefault("clusterLocked") == "1",
            Status = d.GetValueOrDefault("status") switch
            {
                "open" => LobbyStatus.Open,
                "full" => LobbyStatus.Full,
                _ => LobbyStatus.Closed
            },
            MemberCount = count
        };
    }

}
