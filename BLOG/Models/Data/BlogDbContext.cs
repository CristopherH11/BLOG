using BLOG.Models;
using Microsoft.EntityFrameworkCore;

namespace BLOG.Models.Data;
public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<AppUser> Users { get; set; }
}

