using BLOG.Data;
using BLOG.Models;
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
        private dynamic mymodel = new ExpandoObject();

        public HomeController(ILogger<HomeController> logger, BlogDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            // Category
            mymodel.Categories = await _context.Categories.ToListAsync();

            var allUsers = await _context.Users.ToListAsync();
            mymodel.Users = allUsers;

            // Posts
            var tempPosts = from m in _context.Posts orderby m.PostDate descending select m;
            var tempPostsList = await tempPosts.ToListAsync();
            mymodel.PostsCount = tempPostsList.Count;
            mymodel.Posts = PaginateData(tempPostsList, page, 5);

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

        private static List<Post> PaginateData(List<Post> data, int page, int pageSize)
        {
            return data.Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();
        }
    }
}