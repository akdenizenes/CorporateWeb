using System;

namespace CorporateWeb.Entities // Kendi namespace yapına göre düzenleyebilirsin
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}