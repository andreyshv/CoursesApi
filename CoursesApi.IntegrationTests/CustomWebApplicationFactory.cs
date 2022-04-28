using CoursesApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CoursesApi.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=CoursesApiIntTest;Trusted_Connection=True";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //services.AddControllers(); ???

                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<CoursesContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<CoursesContext>(options =>
                {
                    //options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseSqlServer(ConnectionString);
                });

                using var scope = services.BuildServiceProvider().CreateScope();
                
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<CoursesContext>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                try
                {
                    InitContext(context);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the " +
                        "database with test messages. Error: {Message}", ex.Message);
                }
            });
        }

        public List<Student> Students { get; private set; } = null!;
        public List<Term> Courses { get; private set; } = null!;

        private void InitContext(DbContext context)
        {
            Students = new List<Student>()
            {
                new Student { FullName = "First Student", Email = "student1@contoso.com" },
                new Student { FullName = "Second Student", Email = "student2@contoso.com" }
            };

            context.AddRange(Students);

            context.SaveChanges();

            var id = Students[0].Id;

            Courses = new List<Term>()
            {
                new Term { StudentId = id, StartDate = new DateTime(2022, 4, 4), EndDate = new DateTime(2022, 4, 8) },
                new Term { StudentId = id, StartDate = new DateTime(2022, 4, 11), EndDate = new DateTime(2022, 4, 22) }
            };

            context.AddRange(Courses);

            context.SaveChanges();
        }
    }
}
