using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CorporateWeb.Entities; // AppUser sınıfının olduğu yer (gerekirse değiştir)
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
            // Veritabanındaki şifreyle eşleştirme yapıyor
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Giriş başarılı, Admin paneline geç
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre yanlış aga!";
            return View();
        }
    }
}