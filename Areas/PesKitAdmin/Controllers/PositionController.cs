using Microsoft.AspNetCore.Mvc;
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
            List<Position> positions = await _context.Positions.ToListAsync();
            return View(positions);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePositionVM vm)
        {
            Position position = new() { Name = vm.Name.Trim() };

            if (!ModelState.IsValid) return View();

            bool isInclude = await _context.Positions.AnyAsync(p => p.Name == position.Name);
            if (isInclude)
            {
                ModelState.AddModelError("Name", "Bu position movcuddur");
                return View(vm);
            }
            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(Position position)
        {
            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            
            Position position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
            
            if (position is null) return NotFound();

            UpdatePositionVM vm = new UpdatePositionVM()
            {
                Name = position.Name,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdatePositionVM positionVM)
        {
            Position existed= await _context.Positions.FirstOrDefaultAsync(p=>p.Id == id);
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(positionVM);
            }

            bool result = _context.Projects.Any(c => c.Name == positionVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele position artiq movcuddur");
            }


            existed.Name = positionVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
