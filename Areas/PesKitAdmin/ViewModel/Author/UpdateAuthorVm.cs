using System.ComponentModel.DataAnnotations;

namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
    public class UpdateAuthorVm
    {
        [Required]
        public string Name { get; set; }
    }
}
