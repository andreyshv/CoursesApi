using Microsoft.EntityFrameworkCore;
using CoursesApi.Models;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

#pragma warning disable CA1050 // ќбъ€вите типы в пространствах имен
public partial class Program { }
#pragma warning restore CA1050 // ќбъ€вите типы в пространствах имен
