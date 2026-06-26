using Microsoft.AspNetCore.Identity;
namespace CorporateWeb.Entities {
    public class AppUser : IdentityUser {
        public string FullName { get; set; } = string.Empty;
    }
}