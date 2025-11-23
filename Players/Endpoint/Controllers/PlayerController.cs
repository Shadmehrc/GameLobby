using Application.Interfaces.ServiceInterfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Players.Controllers
{
    [ApiController]
    [Route("Player")]
    public class PlayerController(IPlayerService playerService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(string Id, string UserName)
        {
            var res = await playerService.CreateAsync(Id, UserName);
            return Ok(new { lobbyId = res.Id });
        }
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            var player = await playerService.GetByIdAsync(id);

            if (player is null)
                return NotFound();

            return Ok(new
            {
                player.Id,
                player.UserName,
                Status = player.Status.ToString(),
                player.CreatedAt
            });
        }
    }
}
