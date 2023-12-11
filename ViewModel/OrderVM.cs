using PesKitTask.Models;

namespace PesKitTask.ViewModel
{
    public class OrderVM
    {
        public string Address { get; set; }
        public List<BasketItem>? BasketItems { get; set; }
    }
}
