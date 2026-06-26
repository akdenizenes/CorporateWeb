using System;

namespace CorporateWeb.Entities
{
    public class Page
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; } // Boş geçilebilsin diye nullable yaptık
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}