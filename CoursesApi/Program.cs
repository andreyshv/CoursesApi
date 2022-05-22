using Microsoft.EntityFrameworkCore;
using CoursesApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// The following line enables Application Insights telemetry collection.
// https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core
builder.Services.AddApplicationInsightsTelemetry();

#if DEBUG
// for local debugging

// https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-6.0#dp

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
        );
});

builder.Services.AddSwaggerGen();
#endif

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

// route to api
app.MapWhen(context => context.Request.Path.StartsWithSegments("/api"),
    configuration =>
    {
        configuration.UseRouting();
        //configuration.UseAuthorization();
        configuration.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    });

// route to react app
app.MapWhen(context => !context.Request.Path.StartsWithSegments("/api"),
    configuration =>
    { 
        configuration.UseStaticFiles();
        configuration.UseRouting();
        //configuration.UseAuthorization();
        configuration.UseEndpoints(endpoints =>
        {
            endpoints.MapFallbackToFile("index.html");
        });
    });

app.Run();

#pragma warning disable CA1050 // ќбъ€вите типы в пространствах имен
public partial class Program { }
#pragma warning restore CA1050 // ќбъ€вите типы в пространствах имен
