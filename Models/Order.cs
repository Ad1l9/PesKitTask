using PesKitTask.ViewModel;

namespace PesKitTask.Models
{
    public class Order
    {
        public int Id { get; set; }
        public bool? Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string Address { get; set; }
        public DateTime PurchaseAt { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<BasketItemVM> BasketItems { get; set; }
    }
}
