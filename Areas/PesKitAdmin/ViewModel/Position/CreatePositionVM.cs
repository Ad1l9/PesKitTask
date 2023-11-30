using System.ComponentModel.DataAnnotations;

namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
    public class CreatePositionVM
    {
        [Required]
        public string Name { get; set; }
    }
}
