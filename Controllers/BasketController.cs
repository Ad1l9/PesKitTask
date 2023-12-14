using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PesKitTask.DAL;
using PesKitTask.Models;
using PesKitTask.ViewModel;
using System.Security.Claims;

namespace PesKitTask.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _usermanager;

        public BasketController(AppDbContext context, UserManager<AppUser> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketVM = new();

            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _usermanager.Users.Include(u => u.BasketItems.Where(p => p.OrderId == null)).ThenInclude(p => p.Product).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));


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
                        Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketCookieItem.Id);
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


            return View(basketVM);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();


            if (User.Identity.IsAuthenticated)
            {

                AppUser user = await _usermanager.Users.Include(u => u.BasketItems.Where(p => p.OrderId == null)).ThenInclude(p => p.Product).FirstOrDefaultAsync(u => u.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value);

                BasketItem basketItem = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);

                if (basketItem is null)
                {
                    basketItem = new()
                    {
                        AppUserId = user.Id,
                        ProductId = product.Id,
                        Price = product.Price,
                        Count = 1
                    };
                    user.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count++;
                }
                await _context.SaveChangesAsync();

                List<BasketItemVM> basketVM = new();
                foreach (BasketItem item in user.BasketItems)
                {
                    basketVM.Add(new()
                    {
                        Id = item.Id,
                        Name = item.Product.Name,
                        Price = item.Product.Price,
                        ImageUrl = item.Product.ImageUrl,
                        Count = item.Count,
                        SubTotal = item.Count * item.Product.Price
                    });


                }


                return PartialView("_BasketItemPartial", basketVM);
            }
            else
            {
                List<BasketItemVM> basketVM = new();
                List<BasketCookieItem> basket;
                if (Request.Cookies["Basket"] is not null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItem>>(Request.Cookies["Basket"]);
                    BasketCookieItem item = basket.FirstOrDefault(b => b.Id == id);
                    if (item is null)
                    {
                        BasketCookieItem itemVM = new()
                        {
                            Id = product.Id,
                            Count = 1
                        };
                        basket.Add(itemVM);
                    }
                    else
                    {
                        item.Count++;
                    }
                }
                else
                {
                    basket = new();
                    BasketCookieItem itemVM = new()
                    {
                        Id = product.Id,
                        Count = 1
                    };
                    basket.Add(itemVM);
                }
                string json = JsonConvert.SerializeObject(basket);

                Response.Cookies.Append("Basket", json);



                foreach (BasketCookieItem basketCookieItem in basket)
                {
                    Product producte = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketCookieItem.Id);

                    if (producte is not null)
                    {
                        BasketItemVM basketItem = new()
                        {
                            Id = producte.Id,
                            Name = producte.Name,
                            ImageUrl = producte.ImageUrl,
                            Price = producte.Price,
                            Count = basketCookieItem.Count,
                            SubTotal = producte.Price * basketCookieItem.Count,
                        };
                        basketVM.Add(basketItem);
                    }
                }

                return PartialView("_BasketItemPartial", basketVM);

            }

        }

        public async Task<IActionResult> RemoveBasket(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _usermanager.Users.Include(u => u.BasketItems.Where(p => p.OrderId == null)).ThenInclude(p => p.Product).FirstOrDefaultAsync(u => u.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value);

                BasketItem basketItem = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);
                if (basketItem is not null)
                {
                    user.BasketItems.Remove(basketItem);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                List<BasketCookieItem> basket;

                if (Request.Cookies["Basket"] is not null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItem>>(Request.Cookies["Basket"]);
                    BasketCookieItem item = basket.FirstOrDefault(b => b.Id == id);
                    if (item is not null)
                    {
                        basket.Remove(item);

                        string json = JsonConvert.SerializeObject(basket);

                        Response.Cookies.Append("Basket", json);
                    }
                }
            }

            return Redirect(Request.Headers["Referer"]);
        }


        public async Task<IActionResult> Decrement(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _usermanager.Users.Include(u => u.BasketItems.Where(p => p.OrderId == null)).ThenInclude(p => p.Product).FirstOrDefaultAsync(u => u.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value);

                BasketItem basketItem = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);

                if(basketItem is not null)
                {
                    basketItem.Count--;
                    if (basketItem.Count == 0)
                    {
                        user.BasketItems.Remove(basketItem);
                    }
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                List<BasketCookieItem> basket;
                if (Request.Cookies["Basket"] is not null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItem>>(Request.Cookies["Basket"]);
                    var item = basket.FirstOrDefault(b => b.Id == id);
                    if (item is not null)
                    {
                        item.Count--;
                        if (item.Count == 0)
                        {
                            basket.Remove(item);
                        }
                        string json = JsonConvert.SerializeObject(basket);
                        Response.Cookies.Append("Basket", json);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Checkout()
        {
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _usermanager.Users
                .Include(u => u.BasketItems.Where(u => u.OrderId == null))
                .ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                OrderVM orderVM = new OrderVM()
                {
                    BasketItems = user.BasketItems,
                };
                return View(orderVM);
            }
            else return RedirectToAction(nameof(Index), "Basket");

        }
    }
}
