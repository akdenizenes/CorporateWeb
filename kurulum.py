import os
import subprocess

def run_command(command):
    print(f"Çalıştırılıyor: {command}")
    subprocess.run(command, shell=True, check=True)

def write_file(filepath, content):
    os.makedirs(os.path.dirname(filepath), exist_ok=True)
    with open(filepath, "w", encoding="utf-8") as f:
        f.write(content.strip())
    print(f"Oluşturuldu: {filepath}")

# 1. Proje ve Katmanların Oluşturulması
print("--- .NET Projeleri Oluşturuluyor ---")
commands = [
    "dotnet new sln -n CorporateWeb",
    "dotnet new classlib -n CorporateWeb.Entities -f net8.0",
    "dotnet new classlib -n CorporateWeb.DataAccess -f net8.0",
    "dotnet new classlib -n CorporateWeb.Business -f net8.0",
    "dotnet new mvc -n CorporateWeb.WebUI -f net8.0",
    "dotnet sln add CorporateWeb.Entities/CorporateWeb.Entities.csproj",
    "dotnet sln add CorporateWeb.DataAccess/CorporateWeb.DataAccess.csproj",
    "dotnet sln add CorporateWeb.Business/CorporateWeb.Business.csproj",
    "dotnet sln add CorporateWeb.WebUI/CorporateWeb.WebUI.csproj",
    "dotnet add CorporateWeb.DataAccess/CorporateWeb.DataAccess.csproj reference CorporateWeb.Entities/CorporateWeb.Entities.csproj",
    "dotnet add CorporateWeb.Business/CorporateWeb.Business.csproj reference CorporateWeb.DataAccess/CorporateWeb.DataAccess.csproj",
    "dotnet add CorporateWeb.Business/CorporateWeb.Business.csproj reference CorporateWeb.Entities/CorporateWeb.Entities.csproj",
    "dotnet add CorporateWeb.WebUI/CorporateWeb.WebUI.csproj reference CorporateWeb.Business/CorporateWeb.Business.csproj",
    "dotnet add CorporateWeb.WebUI/CorporateWeb.WebUI.csproj reference CorporateWeb.Entities/CorporateWeb.Entities.csproj",
    # MySQL ve Identity Paketleri
    "dotnet add CorporateWeb.DataAccess package Pomelo.EntityFrameworkCore.MySql -v 8.0.2",
    "dotnet add CorporateWeb.DataAccess package Microsoft.AspNetCore.Identity.EntityFrameworkCore -v 8.0.0",
    "dotnet add CorporateWeb.WebUI package Microsoft.EntityFrameworkCore.Design -v 8.0.0",
    "dotnet add CorporateWeb.WebUI package Microsoft.AspNetCore.Identity.UI -v 8.0.0"
]

for cmd in commands:
    run_command(cmd)

# 2. Kodların Dosyalara Yazılması
print("\n--- C# Kodları Dosyalara Yazılıyor ---")

# Entities
write_file("CorporateWeb.Entities/BaseEntity.cs", """
using System;
namespace CorporateWeb.Entities {
    public abstract class BaseEntity {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }
}
""")

write_file("CorporateWeb.Entities/Service.cs", """
namespace CorporateWeb.Entities {
    public class Service : BaseEntity { }
}
""")

write_file("CorporateWeb.Entities/AppUser.cs", """
using Microsoft.AspNetCore.Identity;
namespace CorporateWeb.Entities {
    public class AppUser : IdentityUser {
        public string FullName { get; set; } = string.Empty;
    }
}
""")

# DataAccess
write_file("CorporateWeb.DataAccess/CorporateDbContext.cs", """
using CorporateWeb.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace CorporateWeb.DataAccess {
    public class CorporateDbContext : IdentityDbContext<AppUser> {
        public CorporateDbContext(DbContextOptions<CorporateDbContext> options) : base(options) { }
        public DbSet<Service> Services { get; set; }
    }
}
""")

write_file("CorporateWeb.DataAccess/IRepository.cs", """
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace CorporateWeb.DataAccess {
    public interface IRepository<T> where T : class {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
""")

write_file("CorporateWeb.DataAccess/Repository.cs", """
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace CorporateWeb.DataAccess {
    public class Repository<T> : IRepository<T> where T : class {
        private readonly CorporateDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(CorporateDbContext context) {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null) {
            IQueryable<T> query = _dbSet;
            if (filter != null) query = query.Where(filter);
            return await query.ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Delete(T entity) => _dbSet.Remove(entity);
    }
}
""")

write_file("CorporateWeb.DataAccess/IUnitOfWork.cs", """
using System;
using System.Threading.Tasks;
namespace CorporateWeb.DataAccess {
    public interface IUnitOfWork : IDisposable {
        IRepository<Entities.Service> Services { get; }
        Task<int> SaveAsync();
    }
}
""")

write_file("CorporateWeb.DataAccess/UnitOfWork.cs", """
using System.Threading.Tasks;
namespace CorporateWeb.DataAccess {
    public class UnitOfWork : IUnitOfWork {
        private readonly CorporateDbContext _context;
        public IRepository<Entities.Service> Services { get; private set; }
        public UnitOfWork(CorporateDbContext context) {
            _context = context;
            Services = new Repository<Entities.Service>(_context);
        }
        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
""")

# Business
write_file("CorporateWeb.Business/IServiceManager.cs", """
using CorporateWeb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CorporateWeb.Business {
    public interface IServiceManager {
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task AddServiceAsync(Service service);
    }
}
""")

write_file("CorporateWeb.Business/ServiceManager.cs", """
using CorporateWeb.DataAccess;
using CorporateWeb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CorporateWeb.Business {
    public class ServiceManager : IServiceManager {
        private readonly IUnitOfWork _unitOfWork;
        public ServiceManager(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Service>> GetAllServicesAsync() {
            return await _unitOfWork.Services.GetAllAsync(s => s.IsActive);
        }
        public async Task AddServiceAsync(Service service) {
            await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.SaveAsync();
        }
    }
}
""")

# WebUI
write_file("CorporateWeb.WebUI/appsettings.json", """
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=CorporateWebDb;Uid=root;Pwd=sifren;"
  }
}
""")

write_file("CorporateWeb.WebUI/Program.cs", """
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

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
""")

# Fazlalık varsayılan Class1.cs dosyalarını silme
try:
    os.remove("CorporateWeb.Entities/Class1.cs")
    os.remove("CorporateWeb.DataAccess/Class1.cs")
    os.remove("CorporateWeb.Business/Class1.cs")
except:
    pass

print("\n🚀 BÜTÜN PROJE BAŞARIYLA KURULDU!")
print("Sadece WebUI klasörüne girip appsettings.json içindeki veritabanı şifreni düzelt, ardından 'dotnet run' ile çalıştır!")