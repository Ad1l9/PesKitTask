using Microsoft.AspNetCore.Identity;
using PesKitTask.Utilities.Enums;

namespace PesKitTask.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
    }
}
