using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Test_Comments.Base;
using Test_Comments.Base.Repository;
using Test_Comments.Entities.CommentGroup.Repository;
using Test_Comments.Entities.UserGroup.Repository;
using Microsoft.OpenApi.Models;
using Test_Comments.Services;

namespace Test_Comments.Configuration;

public static class DependencyStartup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        AddDbContext(builder.Services, builder.Configuration);
        AddRepositories(builder.Services);
        AddCorsPolicy(builder.Services);
        AddInfrastructure(builder.Services);
        AddServices(builder.Services);
        
        
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped(typeof(IUserRepository<>), typeof(UserRepository<>));
        services.AddScoped(typeof(ICommentRepository<>), typeof(CommentRepository<>));
    }
    
    public static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        
      

      
    }
    private static void AddInfrastructure(IServiceCollection services)
    {
        services.AddAuthorization(); 
        services.AddControllers(); 
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test Comments API", Version = "v1" });
        });
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
