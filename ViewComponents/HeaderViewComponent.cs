using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PesKitTask.DAL;
using PesKitTask.Models;
using PesKitTask.ViewModel;

namespace ProniaTask.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {


        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _accessor;

        public HeaderViewComponent(AppDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            List<BasketItem> basketVM = new();
            if (Request.Cookies["Basket"] is not null)
            {
                List<BasketCookieItem> basket = JsonConvert.DeserializeObject<List<BasketCookieItem>>(Request.Cookies["Basket"]);

                foreach (BasketCookieItem basketCookieItem in basket)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketCookieItem.Id);
                    if (product is not null)
                    {
                        BasketItem basketItem = new()
                        {
                            Id = product.Id,
                            Name = product.Name,
                            ImageUrl = product.ImageUrl,
                            Price = product.Price,
                            Count = basketCookieItem.Count,
                            SubTotal = product.Price * basketCookieItem.Count,
                        };
                        basketVM.Add(basketItem);
                    }

                }

            }
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            HeaderVM vm = new() { Settings = settings, Basket = basketVM };

            return View(vm);
        }

    }
}