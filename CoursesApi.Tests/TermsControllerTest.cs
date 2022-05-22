using CoursesApi.Controllers;
using CoursesApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoursesApi.Tests
{
    public class TermsControllerTest : IClassFixture<TestFixture>
    {
        public TestFixture Fixture { get; }

        public TermsControllerTest(TestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task GetCourses()
        {
            using var context = Fixture.CreateContext();
            var controller = new TermsController(context);

            long studentId = context.Terms.First().StudentId;
            int count = context.Terms.Where(c => c.StudentId == studentId).Count();

            var result = await controller.GetTerms(studentId);

            var actionResult = Assert.IsType<ActionResult<IEnumerable<TermDTO>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<TermDTO>>(actionResult.Value);
            Assert.Equal(count, returnValue?.Count() ?? 0);
        }

        [Fact]
        public async Task GetCourse()
        {
            using var context = Fixture.CreateContext();
            var controller = new TermsController(context);

            var refItem = context.Terms.First();

            var result = await controller.GetTerm(refItem.Id);

            var actionResult = Assert.IsType<ActionResult<TermDTO>>(result);
            var item = Assert.IsType<TermDTO>(actionResult.Value);

            Assert.Equal(refItem.StudentId, item?.StudentId);
            Assert.Equal(refItem.StartDate, item?.StartDate);
        }

        [Fact]
        public async Task GetStudent_NotFound()
        {
            using var context = Fixture.CreateContext();
            var controller = new TermsController(context);

            var result = await controller.GetTerm(99999);

            var actionResult = Assert.IsType<ActionResult<TermDTO>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task AddCourse()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();

            var controller = new TermsController(context);

            var studentId = context.Students.First().Id;
            var refItem = new TermDTO
            {
                StudentId = studentId,
                StartDate = new DateTime(2022, 4, 25),
                EndDate = new DateTime(2022, 4, 29)
            };

            var result = await controller.PostTerm(refItem);
            
            var actionResult = Assert.IsType<ActionResult<TermDTO>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var item = Assert.IsType<TermDTO>(createdAtActionResult.Value);

            context.ChangeTracker.Clear();

            Assert.Equal(refItem.StudentId, item.StudentId);
            Assert.Equal(refItem.StartDate, item.StartDate);
            Assert.Equal(refItem.EndDate, item.EndDate);
        }

        [Theory]
        [ClassData(typeof(HolidayData))]
        public async Task AddHoliday(TermDTO term, int refHolidays)
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();

            var controller = new TermsController(context);

            var studentId = context.Students.First().Id;
            term.StudentId = studentId;

            var courses = context.Courses
                .Where(c => c.StudentId == studentId);

            int refValue = courses
                .AsEnumerable()
                .Select(c => c.TuitionWeeks)
                .Sum();

            Assert.True(refValue > 0);

            var result = await controller.PostTerm(term);

            var actionResult = Assert.IsType<ActionResult<TermDTO>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.IsType<TermDTO>(createdAtActionResult.Value);

            context.ChangeTracker.Clear();

            int value = courses
                .AsEnumerable()
                .Select(c => c.TuitionWeeks)
                .Sum();

            Assert.Equal(refValue, value);
            
            int holidays = context.Courses
                .AsEnumerable()
                .Select(c => c.Holidays)
                .Sum();

            Assert.Equal(refHolidays, holidays);
        }

        // todo: test holiday term change and delete

    }
}
