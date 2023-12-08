using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PesKitTask.DAL;
using PesKitTask.Models;
using PesKitTask.ViewModel;

namespace PesKitTask.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
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
            return View(basketVM);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();
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

            List<BasketItem> basketVM = new();

                foreach (BasketCookieItem basketCookieItem in basket)
                {
                    Product producte = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketCookieItem.Id);

                    if (producte is not null)
                    {
                        BasketItem basketItem = new()
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


            return PartialView("_BasketItemPartial",basketVM);

        }

        public async Task<IActionResult> RemoveBasket(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();
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


            return Redirect(Request.Headers["Referer"]);
        }


        public async Task<IActionResult> Decrement(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
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
            return RedirectToAction(nameof(Index));
        }
    }
}
