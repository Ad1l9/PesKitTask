namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
	public class UpdateDepartmentVM
    {
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
