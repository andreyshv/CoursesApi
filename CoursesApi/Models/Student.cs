using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesApi.Models
{
    public class StudentDTO : BaseItemDTO
    {
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;
    }

    public class Student : BaseItem
    {
        [Column(TypeName = "nvarchar(100)")]
        public string FullName { get; set; } = null!;
        
        [Column(TypeName = "varchar(320)")]
        public string Email { get; set; } = null!;

        public Student() { }

        public Student(StudentDTO dto)
        {
            FullName = dto.FullName;
            Email = dto.Email;
        }

        public StudentDTO ToDTO()
        {
            return new StudentDTO 
            { 
                FullName = FullName, 
                Email = Email,

                // read only
                Id = Id,
                Version = Version
            };
        }
    }
}
