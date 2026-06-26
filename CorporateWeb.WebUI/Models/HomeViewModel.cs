using CorporateWeb.Entities;
using System.Collections.Generic;

namespace CorporateWeb.WebUI.Models
{
    public class HomeViewModel
    {
        public List<Service> Services { get; set; }
        public List<News> News { get; set; }
        public Page AboutPage { get; set; }
    }
}