using System.ComponentModel.DataAnnotations;

namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
	public class CreateTagVM
	{
		[Required]
        public string Name { get; set; }
    }
}
