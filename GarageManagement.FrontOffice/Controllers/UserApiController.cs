using Microsoft.AspNetCore.Mvc;
using GarageManagement.FrontOffice.Models;
using GarageManagement.FrontOffice.Services;
using System.Threading.Tasks;

namespace GarageManagement.FrontOffice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserApiController : ControllerBase
    {
        private readonly UserService _userService;

        public UserApiController(UserService userService)
        {
            _userService = userService;
        }

        // GET api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // PUT api/user/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(long id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            var updatedUser = await _userService.UpdateUserAsync(user);
            if (updatedUser == null)
            {
                return NotFound();
            }

            return NoContent();  // 204 No Content, indique que la mise à jour a été effectuée avec succès.
        }
    }
}