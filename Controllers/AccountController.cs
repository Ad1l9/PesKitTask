using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PesKitTask.Models;
using PesKitTask.Utilities.Enums;
using PesKitTask.Utilities.Extension;
using PesKitTask.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PesKitTask.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
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
            if(registerVM.Image is not null)
            {
                if (!registerVM.Image.ValidateType("image/"))
                {
                    ModelState.AddModelError("Photo", "File tipi uygun deyil");
                    return View();
                }
                if (!registerVM.Image.ValidateSize(600))
                {
                    ModelState.AddModelError("Photo", "File olcusu uygun deyil");
                    return View();
                }
                await registerVM.Image.CreateFile(_env.WebRootPath, "assets", "img");
            }

            var result = await _userManager.CreateAsync(user,registerVM.Password);

            if (!result.Succeeded)
            {
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(System.String.Empty, error.Description);
                    return View();
                }
                
            }
            await _userManager.AddToRoleAsync(user, UserRoles.Member.ToString());
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM,string? returnUrl)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByNameAsync(loginVM.EmailOrUsername);
            if(user is null)
            {
                user = await _userManager.FindByEmailAsync(loginVM.EmailOrUsername);
                if(user is null)
                {
                    ModelState.AddModelError(System.String.Empty, "Username, Email or Password is incorrect");
                    return View();
                }
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.IsRemembered, true);

            if (result.IsLockedOut)
            {
                TimeSpan difference = (TimeSpan)(user.LockoutEnd - DateTimeOffset.Now);
                ModelState.AddModelError(System.String.Empty, $"Too many attempts, please try {difference.Minutes}min {difference.Seconds}sec later");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(System.String.Empty, "Username, Email or Password is incorrect");
                return View();
            }
            if (returnUrl is not null) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CreateRoles()
        {
            foreach (UserRoles role in Enum.GetValues(typeof(UserRoles)))
            {
                if(!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new(){
                        Name=role.ToString()
                    });
                }
            }
            return RedirectToAction("Index","Home");
        }


        public async Task<IActionResult> MyAccount(string name)
        {
            AppUser user = await _userManager.FindByNameAsync(name);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> MyAccount(RegisterVM vm)
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
