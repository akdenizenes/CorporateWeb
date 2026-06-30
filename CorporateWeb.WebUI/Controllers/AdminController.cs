using Microsoft.AspNetCore.Authorization; 

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateWeb.DataAccess;
using System.Threading.Tasks;
using CorporateWeb.Entities; 

namespace CorporateWeb.WebUI.Controllers
{
    // 2. KİLİDİ BURAYA KOYUYORUZ (Tüm Controller'ı korumaya alır)
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly CorporateDbContext _context;

        public AdminController(CorporateDbContext context)
        {
            _context = context;
        }

        // Admin Paneli Ana Ekranı (Services Tablosu da eklendi!)
        public async Task<IActionResult> Index()
        {
            ViewBag.Pages = await _context.Pages.ToListAsync();
            ViewBag.News = await _context.News.ToListAsync();
            
            // Eğer DbSet adın Services değilse burayı db'deki isme göre düzeltirsin knk
            ViewBag.Services = await _context.Services.ToListAsync(); 
            
            return View();
        }

        #region Page Düzenleme (Hakkında vs.)
        [HttpGet]
        public async Task<IActionResult> EditPage(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page == null) return NotFound();
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPage(int id, string title, string description)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page == null) return NotFound();
            page.Title = title;
            page.Description = description;
            _context.Pages.Update(page);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region News İşlemleri (Create & Edit)
        
        // YENİ: Proje/Haber Ekleme Ekranı
        [HttpGet]
        public IActionResult CreateNews()
        {
            return View();
        }

        // YENİ: Proje/Haber Kaydetme (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNews(string title, string description)
        {
            var newInsight = new News 
            { 
                Title = title, 
                Description = description 
                // Eğer veritabanında Date gibi zorunlu alanlar varsa buraya ekleriz
            };
            await _context.News.AddAsync(newInsight);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditNews(int id)
        {
            var newsItem = await _context.News.FindAsync(id);
            if (newsItem == null) return NotFound();
            return View(newsItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNews(int id, string title, string description)
        {
            var newsItem = await _context.News.FindAsync(id);
            if (newsItem == null) return NotFound();
            newsItem.Title = title;
            newsItem.Description = description;
            _context.News.Update(newsItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Service Düzenleme (Web Development, GenAI vs.)
        
        [HttpGet]
        public async Task<IActionResult> EditService(int id)
        {
            var serviceItem = await _context.Services.FindAsync(id);
            if (serviceItem == null) return NotFound();
            return View(serviceItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(int id, string title, string description)
        {
            var serviceItem = await _context.Services.FindAsync(id);
            if (serviceItem == null) return NotFound();
            
            serviceItem.Title = title;
            serviceItem.Description = description;
            
            _context.Services.Update(serviceItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}