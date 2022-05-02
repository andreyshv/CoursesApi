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
            policy.AllowAnyOrigin()
                .AllowAnyMethod();
        });
});

builder.Services.AddSwaggerGen();

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

    app.UseSwagger();
    app.UseSwaggerUI();
}

#if DEBUG
app.UseCors();
#endif

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// route to react app
var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
app.MapFallbackToFile(filePath);

app.Run();

#pragma warning disable CA1050 // �������� ���� � ������������� ����
public partial class Program { }
#pragma warning restore CA1050 // �������� ���� � ������������� ����
