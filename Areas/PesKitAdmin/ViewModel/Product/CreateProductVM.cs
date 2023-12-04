namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
    public class CreateProductVM
    {
        public string Name { get; set; }
        public IFormFile Photo { get; set; }
        public decimal Price { get; set; }
    }
}
