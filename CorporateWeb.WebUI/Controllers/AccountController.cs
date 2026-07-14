using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CorporateWeb.Entities; // Namespace where the AppUser class lives (change if needed)
using System.Threading.Tasks;

namespace CorporateWeb.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Match the provided password against the one stored in the database
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Login succeeded, redirect to the Admin panel
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }
    }
}