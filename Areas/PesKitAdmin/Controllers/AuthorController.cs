using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKitTask.Areas.PesKitAdmin.ViewModel;
using PesKitTask.DAL;
using PesKitTask.Models;

namespace PesKitTask.Areas.PesKitAdmin.Controllers
{
    [Area("PesKitAdmin")]
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            List<Author> authors = await _context.Authors.ToListAsync();
            return View(authors);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateAuthorVM vm)
        {
            Author author = new() { Name = vm.Name.Trim() };

            if(!ModelState.IsValid) return View();

            bool isInclude=await _context.Authors.AnyAsync(a=>a.Name == author.Name);
            if (isInclude)
            {
                ModelState.AddModelError("Name","Bu Username movcuddur");
                return View(vm);
            }
            await _context.Authors.AddAsync(author);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Author? author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);

            if (author is null) return NotFound();

            UpdateAuthorVm vm = new UpdateAuthorVm()
            {
                Name = author.Name,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateAuthorVm authorVm)
        {
            Author? existed = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(authorVm);
            }

            bool result = _context.Authors.Any(c => c.Name == authorVm.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele author artiq movcuddur");
            }


            existed.Name = authorVm.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Author author)
        {
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
            var author = await _context.Authors.Include(c => c.Blogs).FirstOrDefaultAsync(s => s.Id == id);
            if (author == null) return NotFound();

            return View(author);
        }
    }
}
