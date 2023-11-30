using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKitTask.Areas.PesKitAdmin.ViewModel;
using PesKitTask.DAL;
using PesKitTask.Models;

namespace PesKitTask.Areas.PesKitAdmin.Controllers
{
    [Area("PesKitTask")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Employee> employees = await _context.Employees.ToListAsync();
            return View(employees);
        }
        public IActionResult Create()
        {
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> Create(CreateEm vm)
        //{
        //    Author author = new() { Name = vm.Name.Trim() };

        //    if (!ModelState.IsValid) return View();

        //    bool isInclude = await _context.Authors.AnyAsync(a => a.Name == author.Name);
        //    if (isInclude)
        //    {
        //        ModelState.AddModelError("Name", "Bu Username movcuddur");
        //        return View(vm);
        //    }
        //    _context.Authors.AddAsync(author);
        //    _context.SaveChanges();

        //    return RedirectToAction(nameof(Index));
        //}


        //public async Task<IActionResult> Delete(Author author)
        //{
        //    _context.Authors.Remove(author);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
