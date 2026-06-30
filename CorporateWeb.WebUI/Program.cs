using CorporateWeb.DataAccess;
using CorporateWeb.Entities;
using CorporateWeb.Business;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CorporateDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<CorporateDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CorporateDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    try {
        context.Database.Migrate();
        if (!roleManager.RoleExistsAsync("Admin").Result) {
            roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
        }
        if (userManager.FindByNameAsync("admin").Result == null) {
            var user = new AppUser { UserName = "admin", Email = "admin@corporateweb.com", FullName = "System Admin", EmailConfirmed = true };
            var result = userManager.CreateAsync(user, "Admin@12345").Result;
            if (result.Succeeded) {
                userManager.AddToRoleAsync(user, "Admin").Wait();
            }
        }
    } catch (Exception ex) {
        Console.WriteLine($"Veritabanı Hatası: {ex.Message}");
    }
}

// Statik dosya izinleri (Flutter oyununun .wasm dosyalarını okuyabilmesi için)
var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
provider.Mappings[".wasm"] = "application/wasm";
provider.Mappings[".mjs"] = "text/javascript";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();