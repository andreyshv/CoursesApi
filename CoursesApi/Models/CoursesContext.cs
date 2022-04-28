using Microsoft.EntityFrameworkCore;

namespace CoursesApi.Models
{
    public class CoursesContext: DbContext
    {
        public CoursesContext(DbContextOptions<CoursesContext> options)
            : base(options)
        {
        }

        public DbSet<Term> Terms { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;

        public void MarkAsModified<TEntity>(TEntity item)
        {
            Entry(item!).State = EntityState.Modified;
        }
    }
}
