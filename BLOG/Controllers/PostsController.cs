using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BLOG.Models;
using BLOG.Data;
using System.Composition;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Dynamic;
using System.Security.Claims;

namespace BLOG.Controllers
{

    public class CommentInfo
    {

        public AppUser AppUser { get; set; }
        public Comment Comment { get; set; }
    }
    public class UtilsComponents
    {
        public Post Post { get; set; }

        public AppUser AppUser { get; set; }
        public List<CommentInfo> CommentInfo { get; set; } = new List<CommentInfo>();
    }

    public class PostsController : Controller
    {
        private readonly BlogDbContext _context;
        private dynamic mymodel = new ExpandoObject();

        public PostsController(BlogDbContext context)
        {
            _context = context;
        }

        public IList<UtilsComponents> Posts { get; set; } = default!;

        // GET: Posts
        public async Task<IActionResult> Index(int page = 1)
        {
            // Category
            mymodel.Categories = await _context.Categories.ToListAsync();

            var allUsers = await _context.Users.ToListAsync();
            mymodel.Users = allUsers;

            var tempPosts = from m in _context.Posts orderby m.PostDate descending select m;
            var tempPostsList = await tempPosts.ToListAsync();
            mymodel.PostsCount = tempPostsList.Count;
            mymodel.Posts = PaginateData(tempPostsList, page, 5);

            return View(mymodel);
        }

        private static List<Post> PaginateData(List<Post> data, int page, int pageSize)
        {
            return data.Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();
        }

        // GET: Posts/Category/5
        public async Task<IActionResult> Category(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var tempPost = _context.Posts.Where(m => m.CategoryId == id);
            var post = await tempPost.ToListAsync();

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Author/5
        public async Task<IActionResult> Author(string? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var tempPost = _context.Posts.Where(m => m.AuthorId == id);
            var post = await tempPost.ToListAsync();

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            mymodel.Categories = await _context.Categories.ToListAsync();

            mymodel.Post = new Post();

            return View(mymodel);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Title, string Body, int category)
        {
            Post new_post = new Post
            {
                Title = Title,
                Body = Body,
                PostDate = DateTime.Now,
                AuthorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                CategoryId = category
            };

            if (ModelState.IsValid)
            {
                _context.Add(new_post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(new_post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'BlogDbContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);

            var comments = from comment in _context.Comments
                           where comment.PostId == id
                           select comment;

            List<Comment> allcomments = await comments.ToListAsync();

            _context.Comments.RemoveRange(allcomments);

            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool PostExists(int id)
        {
          return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
