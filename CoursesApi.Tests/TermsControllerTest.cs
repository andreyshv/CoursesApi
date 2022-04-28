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
    public class TermsControllerTest : IClassFixture<TestDatabaseFixture>
    {
        public TestDatabaseFixture Fixture { get; }

        public TermsControllerTest(TestDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async void GetCourses()
        {
            using var context = Fixture.CreateContext();
            var controller = new TermsController(context);

            long studentId = context.Terms.First().StudentId;
            int count = context.Terms.Where(c => c.StudentId == studentId).Count();

            var result = await controller.GetTerms(studentId);

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Term>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Term>>(actionResult.Value);
            Assert.Equal(count, returnValue?.Count() ?? 0);
        }

        [Fact]
        public async void GetCourse()
        {
            using var context = Fixture.CreateContext();
            var controller = new TermsController(context);

            var refItem = context.Terms.First();

            var result = await controller.GetTerms(refItem.Id);

            var actionResult = Assert.IsType<ActionResult<Term>>(result);
            var item = Assert.IsType<Term>(actionResult.Value);
            Assert.Equal(refItem.StudentId, item?.StudentId);
            Assert.Equal(refItem.StartDate, item?.StartDate);
        }

        [Fact]
        public async void GetStudent_NotFound()
        {
            using var context = Fixture.CreateContext();
            var controller = new TermsController(context);

            var result = await controller.GetCourse(99999);

            var actionResult = Assert.IsType<ActionResult<Term>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async void AddCourse()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();

            var controller = new TermsController(context);

            var studentId = context.Students.First().Id;
            var refItem = new Term
            {
                StudentId = studentId,
                StartDate = new DateTime(2022, 4, 25),
                EndDate = new DateTime(2022, 4, 29)
            };

            var result = await controller.PostTerm(refItem);
            
            var actionResult = Assert.IsType<ActionResult<Term>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.IsType<Term>(createdAtActionResult.Value);

            context.ChangeTracker.Clear();

            var item = context.Terms.SingleOrDefault(c => c.Id == refItem.Id);
            Assert.Equal(refItem.StudentId, item?.StudentId);
            Assert.Equal(refItem.StartDate, item?.StartDate);
        }
    }
}
