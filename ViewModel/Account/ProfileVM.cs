namespace PesKitTask.ViewModel
{ 
    public class ProfileVM
    {
        public IFormFile? Image { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Bio { get; set; } = "";

    }
}
