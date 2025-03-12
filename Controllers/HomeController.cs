using Microsoft.AspNetCore.Mvc;
using DigitalWarehouse.Data;  
using DigitalWarehouse.Models;  
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace DigitalWarehouse.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Hämtar alla produkter och skickar dem till vyn
        public IActionResult Index()
        {
            // Hämta alla produkter från databasen + kategori
            var products = _context.Products
                                    .Include(p => p.Category)  // kategori
                                    .ToList();

            // Skicka produkterna till vyn
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
