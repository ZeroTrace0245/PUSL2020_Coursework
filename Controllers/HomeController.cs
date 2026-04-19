using Microsoft.AspNetCore.Mvc;
using PUSL2020_Coursework.Data;

namespace PUSL2020_Coursework.Controllers
{
    public class HomeController : Controller
    {
        private readonly PASDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(PASDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
