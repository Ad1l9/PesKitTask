using PesKitTask.Utilities.Enums;
using System.ComponentModel.DataAnnotations;

namespace PesKitTask.ViewModel
{
    public class RegisterVM
    {
        [Required]
        [MinLength(4, ErrorMessage = "Ad uzunlugu 4den boyuk olmalidir")]
        [MaxLength(25, ErrorMessage = "Ad 25 simvoldan cox ola bilmez")]
        public string Name { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Username uzunlugu 4den boyuk olmalidir")]
        [MaxLength(25, ErrorMessage = "Username 25 simvoldan cox ola bilmez")]
        public string UserName { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Email-in uzunlugu 4den boyuk olmalidir")]
        [MaxLength(256, ErrorMessage = "Email 256 simvoldan cox ola bilmez")]
        [RegularExpression("^[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*@[a-zA-Z]{2,}(?:\\.[a-zA-Z]{2,})+$",
        ErrorMessage = "Email duzgun formatda deyil")]
        public string Email { get; set; }

        public IFormFile? Image { get; set; }


        [Required]
        [MinLength(4, ErrorMessage = "Soyad uzunlugu 4den boyuk olmalidir")]
        [MaxLength(25, ErrorMessage = "Soyad 25 simvoldan cox ola bilmez")]
        public string Surname { get; set; }
        public Gender Gender { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
