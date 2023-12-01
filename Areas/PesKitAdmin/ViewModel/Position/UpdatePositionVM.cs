using System.ComponentModel.DataAnnotations;

namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
    public class UpdatePositionVM
    {
        [Required]
        public string Name { get; set; }
    }
}
