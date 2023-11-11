using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BLOG.Models;
using BLOG.Data;
using System.Composition;
using Microsoft.Extensions.Hosting;

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

        public PostsController(BlogDbContext context)
        {
            _context = context;
        }

        public IList<UtilsComponents> Posts { get; set; } = default!;

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var allRecords = from post in _context.Posts
                             join user in _context.Users on post.AuthorId equals user.Id
                             orderby post.PostDate descending
                             select new UtilsComponents
                             {
                                 Post = post,
                                 AppUser = user
                             };

            List<UtilsComponents> currentPosts = await allRecords.ToListAsync();

            foreach (var recordSRM in currentPosts)
            {
                var new_comments = from comment in _context.Comments
                                   join user in _context.Users on comment.AuthorId equals user.Id
                                   where comment.PostId == recordSRM.Post.Id
                                   select new CommentInfo
                                   {
                                       Comment = comment,
                                       AppUser = user
                                   };

                recordSRM.CommentInfo = await new_comments.ToListAsync();
            }

            return View(currentPosts);
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

            var tempPost = _context.Posts.Where(m => m.Author == id);
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Body")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
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
            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
