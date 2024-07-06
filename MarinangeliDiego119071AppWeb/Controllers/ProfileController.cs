using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Threading.Tasks;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataProtector _protector;

        public ProfileController(UserManager<ApplicationUser> userManager, IDataProtectionProvider dataProtectionProvider)
        {
            _userManager = userManager;
            _protector = dataProtectionProvider.CreateProtector("profile-data-purpose");
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetProtectedUserDataAsync();
            if (user == null)
            {
                return NotFound("User not found");
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await GetProtectedUserDataAsync();
            if (user == null)
            {
                return NotFound("User not found");
            }

            var model = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await GetProtectedUserDataAsync();
                if (user == null)
                {
                    return NotFound("User not found");
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        private async Task<ApplicationUser> GetProtectedUserDataAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return null;
            }

            // Esempio di protezione dell'ID dell'utente
            user.Id = _protector.Protect(user.Id);

            return user;
        }
    }
}
