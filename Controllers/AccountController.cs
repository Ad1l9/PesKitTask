using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PesKitTask.Models;
using PesKitTask.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PesKitTask.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)return View();

            registerVM.Name = registerVM.Name.Trim();
            registerVM.Surname = registerVM.Surname.Trim();

            string name = Char.ToUpper(registerVM.Name[0]) + registerVM.Name.Substring(1).ToLower();
            string surname = Char.ToUpper(registerVM.Surname[0]) + registerVM.Surname.Substring(1).ToLower();

            AppUser user = new()
            {
                Name=name,
                Surname=surname,
                Email=registerVM.Email,
                Gender=registerVM.Gender,
                UserName=registerVM.UserName
            };

            var result = await _userManager.CreateAsync(user,registerVM.Password);

            if (!result.Succeeded)
            {
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(System.String.Empty, error.Description);
                    return View();
                }
                
            }
                await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
