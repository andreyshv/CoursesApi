using CoursesApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursesApi.Tests
{
    public class TestFixture
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=CoursesApiTest;Trusted_Connection=True";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public TestFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        var students = new List<Student>()
                        {
                            new Student { FullName = "First Student", Email = "student1@contoso.com" },
                            new Student { FullName = "Second Student", Email = "student2@contoso.com" }
                        };

                        context.AddRange(students);

                        context.SaveChanges();

                        var studentId = students[0].Id;

                        var courses = new List<Term>()
                        {
                            new Term { StudentId = studentId, StartDate = new DateTime(2022, 4, 4), EndDate = new DateTime(2022, 4, 8) },
                            new Term { StudentId = studentId, StartDate = new DateTime(2022, 4, 11), EndDate = new DateTime(2022, 4, 22) }
                        };

                        context.AddRange(courses);

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
    
    public class HolidayData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new Term { StartDate = new DateTime(2022, 4, 4), EndDate = new DateTime(2022, 4, 8), Type = Term.TermType.Holiday },
                7
            };

            yield return new object[]
            {
                new Term { StartDate = new DateTime(2022, 4, 18), EndDate = new DateTime(2022, 4, 29), Type = Term.TermType.Holiday },
                7
            };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
