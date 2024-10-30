using Microsoft.EntityFrameworkCore;
using Test_Comments.Base;
using Test_Comments.Base.Repository;
using Test_Comments.Entities.CommentsGroup.Repository;

namespace Test_Comments.Configuration;

public static class DependencyStartup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        AddDbContext(builder.Services, builder.Configuration);
        AddRepositories(builder.Services);
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        
        services.AddScoped(typeof(ICommentsRepository<>),typeof(CommentsRepository<>));
    }
    
   
}