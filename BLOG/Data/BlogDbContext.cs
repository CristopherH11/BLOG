using BLOG.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BLOG.Data;
public class BlogDbContext : IdentityDbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<AppUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Post>().ToTable("Posts");
        builder.Entity<Comment>().ToTable("Comments");
        builder.Entity<Category>().ToTable("Category");

        builder.Entity<Comment>().HasKey(e => e.Id);

        builder.Entity<Post>().HasKey(e => e.Id);

        builder.Entity<Category>().HasKey(e => e.Id);

        builder.Entity<Post>()
            .HasOne(e => e.User)
            .WithMany(g => g.Posts)
            .HasForeignKey(g => new { g.AuthorId});

        builder.Entity<Comment>()
            .HasOne(e => e.Post)
            .WithMany(g => g.Comments)
            .HasForeignKey(g => new { g.PostId });

        builder.Entity<Post>()
            .HasMany(e => e.Comments)
            .WithOne(g => g.Post);

        builder.Entity<Comment>()
            .HasOne(e => e.User)
            .WithMany(g => g.Comments)
            .HasForeignKey(g => new { g.AuthorId });
    }
}

