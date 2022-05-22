using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq.Expressions;

namespace CoursesApi.Models
{
    public class TermDTO : BaseItemDTO, IValidatableObject
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public Term.TermType Type { get; set; }

        public long StudentId { get; set; }
        public int Holidays { get; set; }
        public int TuitionWeeks { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    $"Incorrect {Type} duration",
                    new[] { nameof(EndDate) });
            }

            if (StartDate.DayOfWeek != DayOfWeek.Monday)
            {
                yield return new ValidationResult(
                    $"The {Type} should start on Monday",
                    new[] { nameof(StartDate) });
            }

            if (EndDate.DayOfWeek != DayOfWeek.Friday)
            {
                yield return new ValidationResult(
                    $"The {Type} should end on Friday",
                    new[] { nameof(EndDate) });
            }
        }
    }

    [Index(nameof(StartDate))]
    [Index(nameof(EndDate))]
    public class Term : BaseItem
    {
        private int _holidays;
        private DateTime _endDate;

        public enum TermType { Course, Holiday}

        //[Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        //[Column(TypeName = "date")]
        public DateTime EndDate 
        { 
            get { return _endDate.AddDays(Holidays); } 
            set { _endDate = value; } 
        }
        
        public TermType Type { get; set; }

        public long StudentId { get; set; }

        public int Holidays { get => _holidays; }

        public void SetHolidays(int days)
        {
            Debug.Assert(days >= 0 && days % 7 == 0);
            Debug.Assert(Type == TermType.Course);

            _holidays = days;
        }

        public int TuitionWeeks
        {
            get => (Type == TermType.Course) 
                ? (int)Math.Ceiling((EndDate - StartDate).Days / 7.0) - Holidays / 7 
                : 0;
        }
        public Term() { }

        public Term(TermDTO dto)
        {
            StartDate = dto.StartDate;
            EndDate = dto.EndDate;
            StudentId = dto.StudentId;
            Type = dto.Type;
        }

        public TermDTO ToDTO()
        {
            TermDTO dto = new()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                StudentId = StudentId,

                // Read Only
                Id = Id,
                Version = Version,
                Type = Type,
                Holidays = Holidays,
                TuitionWeeks = TuitionWeeks
            };

            return dto;
        }

        public int Intersect(DateTime start, DateTime end)
        {
            // start date of intersection
            if (StartDate > start) {
                start = StartDate;
            }
            // end date of intersection
            if (EndDate < end) {
                end = EndDate;
            }
            
            if (end <= start) {
                return 0;
            }

            int holidays = (end - start).Days;
            
            // round days to whole week
            return (int)Math.Ceiling(holidays / 7.0) * 7;
        }

        public static Expression<Func<Term, bool>> FilterByDate(Term h)
        {
            return c => 
                (c.StartDate >= h.StartDate && c.StartDate <= h.EndDate)
                || (c.EndDate >= h.StartDate && c.EndDate <= h.EndDate);
        }

        public bool IsOverlapped(Term course)
            => course.InRange(StartDate) || course.InRange(EndDate);

        public bool InRange(DateTime date)
            => StartDate >= date && EndDate <= date;
    }
}
