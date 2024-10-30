using Microsoft.EntityFrameworkCore;

namespace Test_Comments.Configuration;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

   
 
}