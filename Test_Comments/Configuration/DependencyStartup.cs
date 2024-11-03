using Microsoft.EntityFrameworkCore;
using Test_Comments.Base;
using Test_Comments.Base.Repository;
using Test_Comments.Entities.CommentGroup.Repository;
using Test_Comments.Entities.UserGroup.Repository;

namespace Test_Comments.Configuration;

public static class DependencyStartup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        AddDbContext(builder.Services, builder.Configuration);
        AddRepositories(builder.Services);
        AddCorsPolicy(builder.Services);
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped(typeof(IUserRepository<>),typeof(UserRepository<>));
        services.AddScoped(typeof(ICommentRepository<>),typeof(CommentRepository<>));
    }
    private static void AddCorsPolicy(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularApp", builder =>
            {
                builder.WithOrigins("http://localhost:4200") 
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
    
   
}