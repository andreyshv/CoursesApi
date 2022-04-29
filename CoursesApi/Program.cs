using Microsoft.EntityFrameworkCore;
using CoursesApi.Models;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
// https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-6.0#dp

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("https://localhost:7158",
                                "https://localhost:44490");
        });
});
#endif

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<CoursesContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("CoursesDatabase")));
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

#if DEBUG
app.UseCors();
#endif

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

#pragma warning disable CA1050 // ќбъ€вите типы в пространствах имен
public partial class Program { }
#pragma warning restore CA1050 // ќбъ€вите типы в пространствах имен
