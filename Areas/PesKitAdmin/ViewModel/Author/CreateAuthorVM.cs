using System.ComponentModel.DataAnnotations;

namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
    public class CreateAuthorVM
    {
        [Required]
        [MaxLength(25,ErrorMessage ="Invalid Name")]
        public string Name { get; set; }
    }
}
