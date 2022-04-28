using Microsoft.EntityFrameworkCore;

namespace CoursesApi.Models
{
    public class CoursesContext: DbContext
    {
        public CoursesContext(DbContextOptions<CoursesContext> options)
            : base(options)
        {
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

        public void MarkAsModified<TEntity>(TEntity item)
        {
            Entry(item!).State = EntityState.Modified;
        }
    }
}
