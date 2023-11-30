using System.ComponentModel.DataAnnotations;

namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
    public class CreateDepartmentVM
    {
        public IFormFile Photo { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
