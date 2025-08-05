using Microsoft.AspNetCore.Mvc;
using TKPM.Data;
using TKPM.Models;
using System.Linq;

namespace TKPM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }
    }
}