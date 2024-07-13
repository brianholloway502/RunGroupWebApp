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
    }
}
