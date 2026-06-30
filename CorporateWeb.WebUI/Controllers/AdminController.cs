using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateWeb.DataAccess;
using System.Threading.Tasks;
using CorporateWeb.Entities;
using System; // DateTime kullanımı için gerekli

namespace CorporateWeb.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly CorporateDbContext _context;

        public AdminController(CorporateDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Pages = await _context.Pages.ToListAsync();
            ViewBag.News = await _context.News.ToListAsync();
            ViewBag.Services = await _context.Services.ToListAsync();
            
            return View();
        }

        #region Page Düzenleme
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
        
        [HttpGet]
        public IActionResult CreateNews()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> CreateNews(string title, string description, string imageUrl, DateTime createdDate)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                imageUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?q=80&w=800&auto=format&fit=crop";
            }

            if (createdDate == default)
            {
                createdDate = DateTime.Now;
            }

            var newInsight = new News 
            { 
                Title = title, 
                Description = description,
                ImageUrl = imageUrl,
                CreatedDate = createdDate
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
        public async Task<IActionResult> EditNews(int id, string title, string description, string imageUrl, DateTime createdDate)
        {
            var newsItem = await _context.News.FindAsync(id);
            if (newsItem == null) return NotFound();

            newsItem.Title = title;
            newsItem.Description = description;
            
            // Eğer yeni resim URL'si boş gelirse eskisini koru
            if (!string.IsNullOrEmpty(imageUrl))
            {
                newsItem.ImageUrl = imageUrl;
            }
            
            newsItem.CreatedDate = createdDate;

            _context.News.Update(newsItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Service Düzenleme
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