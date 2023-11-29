namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
    public class CreateProjectVM
    {
        public string Name { get; set; }
        public IFormFile MainPhoto { get; set; }
        public List<IFormFile>? Photos { get; set; }
    }
}
