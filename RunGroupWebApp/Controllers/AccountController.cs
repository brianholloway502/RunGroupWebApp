using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Data;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Displays the Login page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        /// <summary>
        /// Posts the login page to the server when the user attemps to log in to the application.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            // If model state invalid, do not proceed.
            if (!ModelState.IsValid) 
            {
                return View(loginViewModel);
            }

            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);

            // User was found.
            if (user != null) 
            {
                // Now check the password for the user.
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded) 
                    {
                        return RedirectToAction("Index", "Race");
                    }
                }
                // Otherwise password failed.
                TempData["Error"] = "Wrong credentials. Please, try again.";
                return View(loginViewModel);
            }
            // User not found.
            TempData["Error"] = "Wrong credentials. Please, try again.";
            return View(loginViewModel);
        }

        /// <summary>
        /// Get method that retrieves the Register page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        /// <summary>
        /// Post the data which registers the user in the application database.
        /// </summary>
        /// <param name="registerViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) 
            { 
                return View(registerViewModel);
            }

            // Look for the user.
            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);

            // User was found so we do not want to re-register the same person.
            if (user != null) 
            {
                TempData["Error"] = "This email address is already in use.";
                return View(registerViewModel);
            }

            // Otherwise since email does not exist, we want to create the new user.
            var newUser = new AppUser
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress
            };

            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
            }

            return RedirectToAction("Index", "Race");
        }

        /// <summary>
        /// Logs user out of the application.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Race");
        }
    }
}
