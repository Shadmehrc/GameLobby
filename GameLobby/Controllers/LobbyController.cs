using Application.Interfaces.ServiceInterfaces;
using Application.Model;
using Microsoft.AspNetCore.Mvc;

namespace GameLobby.Controllers
{
    [ApiController]
    [Route("Lobby")]
    public class LobbyController(ILobbyService lobbyService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<bool>> Create(CreateLobbyModel model)
        {
            var result = await lobbyService.Create(model);
            return Ok();
        }
        [HttpGet("city/{city}")]
        public async Task<ActionResult<bool>> JoinLobby(string city)
        {
            var result = await lobbyService.JoinLobby();
            return Ok();
        }
    }
}
