using PesKitTask.Models;
using System.Drawing;

namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
    public class UpdateProjectVM
    {
        public string Name { get; set; }


        public IFormFile? MainPhoto { get; set; }
        public List<IFormFile>? Photos { get; set; }


        public List<int>? ImageIds { get; set; }
        public List<ProjectImage>? ProjectImages { get; set; }
    }
}
