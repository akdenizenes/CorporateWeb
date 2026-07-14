using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateWeb.DataAccess;
using System.Threading.Tasks;
using CorporateWeb.Entities;
using System; // Required for DateTime usage

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

        #region Page Editing
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

#region News Operations (Create & Edit)
        
        [HttpGet]
        public IActionResult CreateNews()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        // 'bool isActive' parameter added
        public async Task<IActionResult> CreateNews(string title, string description, string imageUrl, DateTime createdDate, bool isActive)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                imageUrl = "/img/match3-news.png";
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
                CreatedDate = createdDate,
                IsActive = isActive // Persist the toggle value coming from the UI to the database
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
        // 'bool isActive' parameter added
        public async Task<IActionResult> EditNews(int id, string title, string description, string imageUrl, DateTime createdDate, bool isActive)
        {
            var newsItem = await _context.News.FindAsync(id);
            if (newsItem == null) return NotFound();

            newsItem.Title = title;
            newsItem.Description = description;
            
            // Keep the existing image if the new image URL is empty
            if (!string.IsNullOrEmpty(imageUrl))
            {
                newsItem.ImageUrl = imageUrl;
            }
            
            newsItem.CreatedDate = createdDate;
            newsItem.IsActive = isActive; // Write the current toggle state to the database

            _context.News.Update(newsItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Service Editing
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