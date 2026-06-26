import os

def write_file(filepath, content):
    os.makedirs(os.path.dirname(filepath), exist_ok=True)
    with open(filepath, "w", encoding="utf-8") as f:
        f.write(content.strip())
    print(f"Güncellendi: {filepath}")

# HomeController Güncellemesi
write_file("CorporateWeb.WebUI/Controllers/HomeController.cs", """
using Microsoft.AspNetCore.Mvc;
using CorporateWeb.Business;
using System.Threading.Tasks;

namespace CorporateWeb.WebUI.Controllers {
    public class HomeController : Controller {
        private readonly IServiceManager _serviceManager;

        public HomeController(IServiceManager serviceManager) {
            _serviceManager = serviceManager;
        }

        public async Task<IActionResult> Index() {
            var services = await _serviceManager.GetAllServicesAsync();
            return View(services);
        }

        public IActionResult Privacy() {
            return View();
        }
    }
}
""")

# Anasayfa (Index.cshtml)
write_file("CorporateWeb.WebUI/Views/Home/Index.cshtml", """
@model IEnumerable<CorporateWeb.Entities.Service>
@{
    ViewData["Title"] = "Anasayfa";
}
<div class="container my-5">
    <div class="row text-center mb-5">
        <h1 class="display-4 fw-bold">Kurumsal Dünyamıza Hoş Geldiniz</h1>
        <p class="lead text-muted">Geleceğin teknolojilerini profesyonel çözümlerle buluşturuyoruz.</p>
    </div>

    <div class="row g-4">
        <h2 class="h3 border-bottom pb-2">Hizmetlerimiz</h2>
        @if (Model == null || !Model.Any())
        {
            <div class="alert alert-info text-center">Henüz eklenmiş aktif bir hizmet bulunmamaktadır. Admin panelinden ekleyebilirsiniz.</div>
        }
        else
        {
            @foreach (var service in Model)
            {
                <div class="col-md-4">
                    <div class="card h-100 shadow-sm border-0">
                        <img src="@(string.IsNullOrEmpty(service.ImageUrl) ? "https://via.placeholder.com/350x200" : service.ImageUrl)" class="card-img-top" alt="@service.Title">
                        <div class="card-body">
                            <h5 class="card-title fw-bold">@service.Title</h5>
                            <p class="card-text">@service.Description</p>
                        </div>
                    </div>
                </div>
            }
        }
    </div>
</div>
""")

# Layout (Bootstrap 5 Eklemesi)
write_file("CorporateWeb.WebUI/Views/Shared/_Layout.cshtml", """
<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - CorporateWeb</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
</head>
<body class="d-flex flex-column min-vh-100 bg-light">
    <partial name="_NavbarPartial" />
    <main role="main" class="flex-grow-1">
        @RenderBody()
    </main>
    <partial name="_FooterPartial" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
""")

# Navbar
write_file("CorporateWeb.WebUI/Views/Shared/_NavbarPartial.cshtml", """
<nav class="navbar navbar-expand-lg navbar-dark bg-dark shadow">
    <div class="container">
        <a class="navbar-brand fw-bold" href="/">CorporateWeb</a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
            <ul class="navbar-nav ms-auto">
                <li class="nav-item"><a class="nav-link active" href="/">Anasayfa</a></li>
                <li class="nav-item"><a class="nav-link" href="#">Hakkımızda</a></li>
                <li class="nav-item"><a class="nav-link" href="#">Hizmetler</a></li>
                <li class="nav-item"><a class="nav-link" href="#">Haberler</a></li>
                <li class="nav-item"><a class="nav-link" href="#">İletişim</a></li>
            </ul>
        </div>
    </div>
</nav>
""")

# Footer
write_file("CorporateWeb.WebUI/Views/Shared/_FooterPartial.cshtml", """
<footer class="bg-dark text-light text-center py-4 mt-auto border-top border-secondary">
    <div class="container">
        <p class="mb-0">&copy; 2026 CorporateWeb. Tüm Hakları Saklıdır.</p>
    </div>
</footer>
""")

print("Tasarım dosyaları başarıyla güncellendi! 😎")