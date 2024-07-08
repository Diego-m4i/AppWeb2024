    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Models;
    using WebApp.ViewModels;

    namespace WebApp.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        [Authorize]
        public class ProfileController : ControllerBase
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public ProfileController(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }

            [HttpGet]
            public async Task<IActionResult> GetProfile()
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(new { user.Email, user.PasswordHash });
            }
        }
    }