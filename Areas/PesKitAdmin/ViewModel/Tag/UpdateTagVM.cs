using System.ComponentModel.DataAnnotations;

namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
    public class UpdateTagVM
    {
        [Required]
        public string Name { get; set; }
    }
}
