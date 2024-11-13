﻿using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Test_Comments.Base;
using Test_Comments.Base.Repository;
using Test_Comments.Entities.UserGroup.Repository;
using Microsoft.OpenApi.Models;
using Test_Comments.Entities.RecordGroup.Repository;
using Test_Comments.Services;

namespace Test_Comments.Configuration;

public static class DependencyStartup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        AddDbContext(builder.Services, builder.Configuration);
        AddRepositories(builder.Services);
        AddCorsPolicy(builder.Services,builder.Configuration);
        AddInfrastructure(builder.Services);
        AddServices(builder.Services);
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        using (var serviceProvider = services.BuildServiceProvider())
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated(); 
        }
    }


    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped(typeof(IUserRepository<>), typeof(UserRepository<>));
        services.AddScoped(typeof(IRecordRepository<>), typeof(RecordRepository<>));
        
    }
    
    public static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRecordService, RecordService>();
        services.AddScoped<ICaptchaService, CaptchaService>();
        
    
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
        
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            
        });
    }
    private static void AddCorsPolicy(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDistributedMemoryCache();

        var corsOrigin = configuration.GetSection("Cors")["Origin"];
    
        Console.WriteLine($"CORS дозволений для домену: {corsOrigin}");
    
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                    builder.WithOrigins(corsOrigin) 
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
        });
    }

}