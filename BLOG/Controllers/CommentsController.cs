using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BLOG.Models;
using BLOG.Data;
using Microsoft.Extensions.Hosting;
using System.Composition;
using Microsoft.AspNetCore.Authorization;

namespace BLOG.Controllers
{
    public class CommentsController : Controller
    {
        private readonly BlogDbContext _context;

        public CommentsController(BlogDbContext context)
        {
            _context = context;
        }

        public UtilsComponents Posts { get; set; } = default!;

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var blogDbContext = _context.Comments.Include(c => c.Post);
            return View(await blogDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public async Task<IActionResult> Create(int postId)
        {
            var query = await (from post in _context.Posts
                               join user in _context.Users on post.AuthorId equals user.Id
                               where post.Id == postId
                               select new UtilsComponents
                               {
                                   Post = post,
                                   AppUser = user
                               })
                 .FirstOrDefaultAsync();

            if (query != null)
            {
                UtilsComponents temp = new UtilsComponents
                {
                    Post = query.Post,
                    AppUser = query.AppUser,
                    CommentInfo = await GetCommentsAsync(query.Post.Id),
                };

                Posts = temp;

                return View(Posts);
            }
            else
            {
                // Manejar el caso cuando query es null, por ejemplo, redirigir a una página de error.
                return NotFound(); // o return RedirectToAction("Error");
            }
        }

        public async Task<List<CommentInfo>> GetCommentsAsync(int id)
        {
            var new_comments = from comment in _context.Comments
                               join user in _context.Users on comment.AuthorId equals user.Id
                               where comment.PostId == id
                               select new CommentInfo
                               {
                                   Comment = comment,
                                   AppUser = user
                               };

            return await new_comments.ToListAsync();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(int postId, string text, string AuthorId)
        {
            Comment comment = new Comment
            {
                PostId = postId,
                Text = text,
                AuthorId = AuthorId
            };

            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Comments", new { postId });
            }

            return RedirectToAction("Create", "Comments", new { postId });
        }


        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", comment.PostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,PostId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", comment.PostId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set 'BlogDbContext.Comments'  is null.");
            }
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
          return (_context.Comments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
