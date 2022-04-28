using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesApi.Models
{
    [Index(nameof(StartDate))]
    [Index(nameof(EndDate))]
    public class Term : IValidatableObject
    {
        private int _holidays;

        public enum TermType { Course, Holiday}

        public long Id { get; set; }
        
        [DataType(DataType.Date)]
        //?[Column(TypeName = "date")]
        public DateTime StartDate { get; set; }
        
        [DataType(DataType.Date)]
        //?[Column(TypeName = "date")]
        public DateTime EndDate { get; set; }
        
        public TermType Type { get; set; }

        public long StudentId { get; set; }

        public int Holidays { 
            get => _holidays;
            set 
            {
                if (_holidays < 0)
                {
                    throw new InvalidOperationException($"{nameof(Holidays)} should get a positive value");
                }

                if (_holidays != value)
                {

                } 
            } 
        }

        public void RemoveHolidays()
        {
            if (Type != TermType.Course)
            {
                throw new InvalidOperationException();
            }

            Holidays = 0;
        }

        public void AddHolidays(Term term)
        {
            if (Type != TermType.Course
                || term.Type != TermType.Holiday)
            {
                throw new InvalidOperationException();
            }

            // start date of intersection
            var startDate = (StartDate > term.StartDate) ? StartDate : term.StartDate;
            // end date of intersection
            var endDate = (EndDate < term.EndDate) ? EndDate : term.EndDate;
            int holidays = (endDate - startDate).Days;
            // round days to whole week
            holidays = (int)Math.Ceiling(holidays / 7.0) * 7;

            if (holidays < Weeks())
            {
                throw new InvalidOperationException($"{nameof(Holidays)} excieds ");
            }

        }

        public bool IsOverlapped(Term course)
            => course.InRange(StartDate) || course.InRange(EndDate);

        public bool InRange(DateTime date)
            => StartDate >= date && EndDate <= date;

        public int Weeks()
        {
            return (int)(((EndDate - StartDate).TotalDays + 3) / 7);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    $"Incorrect {Type} duration", 
                    new [] { nameof(EndDate) });
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
}
