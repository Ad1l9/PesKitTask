using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PesKitTask.Areas.PesKitAdmin.ViewModel;
using PesKitTask.DAL;
using PesKitTask.Models;

namespace PesKitTask.Areas.PesKitAdmin.Controllers
{
    [Area("PesKitAdmin")]
    public class PositionController : Controller
    {
        private readonly AppDbContext _context;

        public PositionController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Position> positions = new List<Position>();
            return View(positions);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePositionVM vm)
        {
            Author author = new() { Name = vm.Name.Trim() };

            if (!ModelState.IsValid) return View();

            bool isInclude = await _context.Positions.AnyAsync(a => a.Name == author.Name);
            if (isInclude)
            {
                ModelState.AddModelError("Name", "Bu Username movcuddur");
                return View(vm);
            }
            _context.Authors.AddAsync(author);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(Author author)
        {
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
