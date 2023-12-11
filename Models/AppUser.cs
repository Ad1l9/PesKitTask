using Microsoft.AspNetCore.Identity;
using PesKitTask.Utilities.Enums;
using PesKitTask.ViewModel;

namespace PesKitTask.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
        public string ImageUrl { get; set; } = "defaultpp.jpg";

        public List<BasketItem> BasketItems { get; set; }
    }
}
