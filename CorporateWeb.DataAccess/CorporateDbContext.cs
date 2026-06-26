using CorporateWeb.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace CorporateWeb.DataAccess
{
    public class CorporateDbContext : IdentityDbContext<AppUser>
    {
        public CorporateDbContext(DbContextOptions<CorporateDbContext> options) : base(options) { }
        public DbSet<Service> Services { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Page> Pages { get; set; }
    }
}