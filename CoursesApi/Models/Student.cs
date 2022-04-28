using System.ComponentModel.DataAnnotations;

namespace CoursesApi.Models
{
    public class Student
    {
        public long Id { get; set; }

        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = null!;
        
        [EmailAddress]
        [MaxLength(320)]
        public string Email { get; set; } = null!;
        //public List<Course> Courses { get; set; }
    }
}
