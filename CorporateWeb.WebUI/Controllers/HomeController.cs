using CorporateWeb.DataAccess;
using CorporateWeb.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace CorporateWeb.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly CorporateDbContext _context;

        public HomeController(CorporateDbContext context)
        {
            _context = context;
        }

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

        public IActionResult About()
        {
            var aboutInfo = _context.Pages.FirstOrDefault(x => x.IsActive);
            return View(aboutInfo);
        }

        // GET: İletişim sayfasını açan metod
        public IActionResult Contact()
        {
            return View();
        }

        // POST: Mesaj gönderildiğinde çalışan metod
        [HttpPost]
        public IActionResult Contact(string name, string email, string message)
        {
            try
            {
                var senderEmail = "enesakdeniz306@gmail.com";
                var senderPassword = "eanojqureumvbikx";

                var mail = new MailMessage();
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(senderEmail);
                mail.Subject = $"Portfolyo Yeni Mesaj: {name}";
                mail.Body = $"Gönderen: {name} \nE-Posta Adresi: {email} \n\nMesaj:\n{message}";

                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                smtpClient.Send(mail);

                ViewBag.Message = "Mesajınız başarıyla gönderildi! En kısa sürede dönüş yapacağım.";
                ViewBag.AlertType = "success";
            }
            catch (System.Exception ex)
            {
                ViewBag.Message = "Mesaj gönderilirken bir hata oluştu. Lütfen tekrar deneyin. Detay: " + ex.Message;
                ViewBag.AlertType = "danger";
            }

            return View();
        }

        // --- YENİ EKLENEN HİZMETLER VE HABERLER SAYFALARI ---

        public IActionResult Services()
        {
            // Hizmetleri aktiflik ve sırasına göre çekiyoruz
            var services = _context.Services.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToList();
            return View(services);
        }

        public IActionResult News()
        {
            // Haberleri tarihe göre en yeniden eskiye doğru sıralayıp çekiyoruz
            var news = _context.News.Where(x => x.IsActive).OrderByDescending(x => x.CreatedDate).ToList();
            return View(news);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}