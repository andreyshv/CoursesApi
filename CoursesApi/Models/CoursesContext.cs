using Microsoft.EntityFrameworkCore;

namespace CoursesApi.Models
{
    public class CoursesContext: DbContext
    {
        public CoursesContext(DbContextOptions<CoursesContext> options)
            : base(options)
        {
            Database.EnsureCreated();
#if DEBUG
            if (!Students.Any())
            {
                var students = new List<Student>()
                        {
                            new Student { FullName = "First Student", Email = "student1@contoso.com" },
                            new Student { FullName = "Second Student", Email = "student2@contoso.com" }
                        };

                AddRange(students);
                SaveChanges();

                var studentId = students[0].Id;
                var courses = new List<Term>()
                        {
                            new Term { StudentId = studentId, StartDate = new DateTime(2022, 4, 4), EndDate = new DateTime(2022, 4, 8) },
                            new Term { StudentId = studentId, StartDate = new DateTime(2022, 4, 11), EndDate = new DateTime(2022, 4, 22) }
                        };

                AddRange(courses);
                SaveChanges();
            }
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Field-only property
            modelBuilder.Entity<Term>()
                .Property<int>("_holidays");
        }

        public DbSet<Term> Terms { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;

        public IQueryable<Term> Courses =>
            Terms.Where(h => h.Type == Term.TermType.Course);
        public IQueryable<Term> Holidays =>
            Terms.Where(h => h.Type == Term.TermType.Holiday);
    }
}
