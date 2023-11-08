using BLOG.Models;
using BLOG.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Dynamic;

namespace BLOG.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlogDbContext _context;

        public HomeController(ILogger<HomeController> logger, BlogDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Category
            dynamic mymodel = new ExpandoObject();
            mymodel.Categories = await _context.Categories.ToListAsync();

            // Authors
            var tempAuthors = from m in _context.Posts select m.Author;
            mymodel.Authors = await tempAuthors.Distinct().ToListAsync();

            // Posts
            var tempPosts = from m in _context.Posts orderby m.PostDate descending select m;
            mymodel.Posts = await tempPosts.ToListAsync();

            return View(mymodel);
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