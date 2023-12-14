using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PesKitTask.DAL;
using PesKitTask.Models;
using PesKitTask.ViewModel;
using System.Security.Claims;

namespace PesKitTask.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {


        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<AppUser> _usermanager;


        public HeaderViewComponent(AppDbContext context, IHttpContextAccessor accessor, UserManager<AppUser> usermanager)
        {
            _context = context;
            _accessor = accessor;
            _usermanager = usermanager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            List<BasketItemVM> basketVM = new();
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _usermanager.Users.Include(u => u.BasketItems.Where(p => p.OrderId == null)).ThenInclude(p => p.Product).FirstOrDefaultAsync(u => u.Id == _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                
                foreach (BasketItem item in user.BasketItems)
                {
                    basketVM.Add(new()
                    {
                        Id = item.ProductId,
                        Name = item.Product.Name,
                        Price = item.Product.Price,
                        ImageUrl = item.Product.ImageUrl,
                        Count = item.Count,
                        SubTotal = item.Count * item.Product.Price
                    });


                }
                await _context.SaveChangesAsync();
            }
            else
            {
                if (Request.Cookies["Basket"] is not null)
                {
                    List<BasketCookieItem> basket = JsonConvert.DeserializeObject<List<BasketCookieItem>>(Request.Cookies["Basket"]);

                    foreach (BasketCookieItem basketCookieItem in basket)
                    {
                        Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketCookieItem.Id);
                        if (product is not null)
                        {
                            BasketItemVM basketItem = new()
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
            }
            
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            HeaderVM vm = new() { Settings = settings, Basket = basketVM };

            return View(vm);
        }

    }
}