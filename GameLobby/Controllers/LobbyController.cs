using Application.Interfaces.ServiceInterfaces;
using Application.Model;
using Microsoft.AspNetCore.Mvc;

namespace GameLobby.Controllers
{
    [ApiController]
    [Route("Lobby")]
    public class LobbyController(ILobbyService _svc) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var res = await _svc.CreateLobby();
            return res.IsSuccess
                ? Ok(new { lobbyId = res.Value })
                : StatusCode(500, new { error = res.Message, code = res.Code });
        }

        [HttpPost("{lobbyID}/join")]
        public async Task<IActionResult> Join(long lobbyID, string playerID)
        {
            var res = await _svc.JoinLobby(lobbyID, playerID);
            return res.IsSuccess
                ? Ok(new { message = res.Message })
                : Conflict(new { error = res.Message, code = res.Code });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var lobby = await _svc.GetLobby(id);
            return lobby is null ? NotFound() : Ok(lobby);
        }

    }
}
