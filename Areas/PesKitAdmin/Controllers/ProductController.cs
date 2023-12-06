using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKitTask.Areas.PesKitAdmin.ViewModel;
using PesKitTask.DAL;
using PesKitTask.Models;
using PesKitTask.Utilities.Extension;

namespace PesKitTask.Areas.PesKitAdmin.Controllers
{
    [Area("PesKitAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            List<Product> productlist = await _context.Products.ToListAsync();
            return View(productlist);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (!vm.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError("Photo", "File tipi uygun deyil");
                return View();
            }
            if (!vm.Photo.ValidateSize(600))
            {
                ModelState.AddModelError("Photo", "File olcusu uygun deyil");
                return View();
            }

            Product product = new Product()
            {
                Name = vm.Name,
                Price = vm.Price,
                ImageUrl = await vm.Photo.CreateFile(_env.WebRootPath, "assets", "img"),
            };



            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products
                .FirstOrDefaultAsync(d => d.Id == id);


            if (product is null) return NotFound();

            UpdateProductVM depVM = new()
            {
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
            };
            return View(depVM);
        }

    }
}
