using Microsoft.EntityFrameworkCore;
using Test_Comments.Entities;
using Test_Comments.Entities.CommentsGroup;

namespace Test_Comments.Configuration;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

   
    public DbSet<Comment> Comments { get; set; }
}