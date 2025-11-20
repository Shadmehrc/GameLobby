using Microsoft.AspNetCore.Mvc;
using Players.Grpc;

namespace Lobby.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerInfoController : ControllerBase
    {
        private readonly PlayerGrpc.PlayerGrpcClient _playerClient;

        public PlayerInfoController(PlayerGrpc.PlayerGrpcClient playerClient)
        {
            _playerClient = playerClient;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var reply = await _playerClient.GetPlayerAsync(new GetPlayerRequest
                {
                    Id = id
                });

                return Ok(new
                {
                    reply.Id,
                    reply.UserName,
                    reply.Status,
                    reply.CreatedAt
                });
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                return NotFound(new { message = ex.Status.Detail });
            }
        }
    }
}
