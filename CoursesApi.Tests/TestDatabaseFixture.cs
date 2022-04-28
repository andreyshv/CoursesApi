using CoursesApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursesApi.Tests
{
    public class TestDatabaseFixture
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=CoursesApiTest;Trusted_Connection=True";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public TestDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        var student = new Student { FullName = "First Student", Email = "student1@contoso.com" };
                        
                        context.AddRange(
                            student,
                            new Student { FullName = "Second Student", Email = "student2@contoso.com" });

                        context.AddRange(
                            new Term { StudentId = student.Id, StartDate = new DateTime(2022, 4, 4), EndDate = new DateTime(2022, 4, 8) },
                            new Term { StudentId = student.Id, StartDate = new DateTime(2022, 4, 11), EndDate = new DateTime(2022, 4, 22) });

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public CoursesContext CreateContext()
            => new(
                new DbContextOptionsBuilder<CoursesContext>()
                    .UseSqlServer(ConnectionString)
                    .Options);
    }
}
