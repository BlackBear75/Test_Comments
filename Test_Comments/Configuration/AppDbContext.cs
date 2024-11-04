using Microsoft.EntityFrameworkCore;
using Test_Comments.Entities;
using Test_Comments.Entities.CommentGroup;
using Test_Comments.Entities.UserGroup;

namespace Test_Comments.Configuration;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

   
    public DbSet<Comment> Comments { get; set; }
    public DbSet<User> Users { get; set; }
    
}