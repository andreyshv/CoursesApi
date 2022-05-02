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
app.MapWhen(context => !context.Request.Path.StartsWithSegments("/api"),
    configuration =>
    { 
        configuration.UseStaticFiles();
        configuration.UseRouting();
        configuration.UseEndpoints(endpoints =>
        {
            endpoints.MapFallbackToFile("index.html");
        });
    });

app.Run();

#pragma warning disable CA1050 // ќбъ€вите типы в пространствах имен
public partial class Program { }
#pragma warning restore CA1050 // ќбъ€вите типы в пространствах имен
