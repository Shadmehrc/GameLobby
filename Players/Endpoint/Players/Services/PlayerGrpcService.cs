using Grpc.Core;
using Players.Grpc;
using Application.Interfaces.ServiceInterfaces; 
namespace Players.GrpcServices;

public class PlayerGrpcService(IPlayerService _playerService) : PlayerGrpc.PlayerGrpcBase
{

    public override async Task<GetPlayerReply> GetPlayer(GetPlayerRequest request, ServerCallContext context)
    {
        var player = await _playerService.GetByIdAsync(request.Id);

        if (player is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Player '{request.Id}' not found."));
        }

        return new GetPlayerReply
        {
            Id = player.Id,
            UserName = player.UserName,
            Status = player.Status.ToString(),
            CreatedAt = player.CreatedAt.ToUniversalTime().ToString("O")
        };
    }
}
