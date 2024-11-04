using Test_Comments.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices(); 

var app = builder.Build();

app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseSwagger(); 
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test Comments API V1");
    c.RoutePrefix = string.Empty; 
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();