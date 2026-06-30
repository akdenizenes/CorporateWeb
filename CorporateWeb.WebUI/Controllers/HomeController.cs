using CorporateWeb.DataAccess;
using CorporateWeb.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // BUNU EKLEDİK (appsettings.json'ı okuyabilmek için gerekli kütüphane)
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace CorporateWeb.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly CorporateDbContext _context;
        private readonly IConfiguration _configuration; // BUNU EKLEDİK (Bağımlılığı tanımladık)

        // CONSTRUCTOR'I GÜNCELLEDİK (IConfiguration'ı içeri aldık)
        public HomeController(CorporateDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: /Home/Index
        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                Services = _context.Services.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToList(),
                News = _context.News.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToList(),
                AboutPage = _context.Pages.FirstOrDefault(x => x.IsActive)
            };
            return View(viewModel);
        }

        // GET: /Home/About
        public IActionResult About()
        {
            var aboutInfo = _context.Pages.FirstOrDefault(x => x.IsActive);
            return View(aboutInfo);
        }

        // GET: /Home/Contact
        public IActionResult Contact()
        {
            return View();
        }

        // POST: /Home/Contact
        [HttpPost]
        public IActionResult Contact(string name, string email, string message)
        {
            try
            {
                // VERİLERİ ARTIK SABİT YAZMAK YERİNE APPSETTINGS.JSON'DAN ÇEKİYORUZ
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderPassword = _configuration["EmailSettings:SenderPassword"];

                var mail = new MailMessage();
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(senderEmail); // Feedback emails will be delivered to your own inbox
                mail.Subject = $"Portfolio New Message from: {name}";
                mail.Body = $"Sender Name: {name} \nEmail Address: {email} \n\nMessage:\n{message}";

                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                smtpClient.Send(mail);

                ViewBag.Message = "Your message has been sent successfully! I will get back to you as soon as possible.";
                ViewBag.AlertType = "success";
            }
            catch (System.Exception ex)
            {
                ViewBag.Message = "An error occurred while sending your message. Please try again later. Detail: " + ex.Message;
                ViewBag.AlertType = "danger";
            }

            return View();
        }

        // --- DYNAMIC CONTENT PAGES ---

        // GET: /Home/Services
        public IActionResult Services()
        {
            // Fetching active services sorted by their custom display order
            var services = _context.Services.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToList();
            return View(services);
        }

        // GET: /Home/News
        public IActionResult News()
        {
            // Fetching active news articles ordered from newest to oldest
            var news = _context.News.Where(x => x.IsActive).OrderByDescending(x => x.CreatedDate).ToList();
            return View(news);
        }

        // GET: /Home/Privacy
        public IActionResult Privacy()
        {
            return View();
        }
    }
}