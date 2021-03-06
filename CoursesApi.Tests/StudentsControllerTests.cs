using CoursesApi.Controllers;
using CoursesApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CoursesApi.Tests
{
    public class StudentsControllerTests : IClassFixture<TestFixture>
    {
        public TestFixture Fixture { get; }

        public StudentsControllerTests(TestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task GetStudents()
        {
            using var context = Fixture.CreateContext();
            var controller = new StudentsController(context);

            var result = await controller.GetStudents();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<StudentDTO>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<StudentDTO>>(actionResult.Value);
            Assert.Equal(context.Students.Count(), returnValue?.Count() ?? 0);
        }

        [Fact]
        public async Task GetStudent()
        {
            using var context = Fixture.CreateContext();
            var controller = new StudentsController(context);

            var refItem = context.Students.First();

            var result = await controller.GetStudent(refItem.Id);
            
            var actionResult = Assert.IsType<ActionResult<StudentDTO>>(result);
            var item = Assert.IsType<StudentDTO>(actionResult.Value);
            Assert.Equal(refItem.FullName, item?.FullName);
        }

        [Fact]
        public async Task GetStudent_NotFound()
        {
            using var context = Fixture.CreateContext();
            var controller = new StudentsController(context);

            var result = await controller.GetStudent(99999);

            var actionResult = Assert.IsType<ActionResult<StudentDTO>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result); 
        }

        [Fact]
        public async Task PostStudent()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();

            var controller = new StudentsController(context);
            var refItem = new StudentDTO { 
                FullName = "Student N", 
                Email = "N@contoso.com"
            };

            var result = await controller.PostStudent(refItem);

            var actionResult = Assert.IsType<ActionResult<StudentDTO>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var item = Assert.IsType<StudentDTO>(createdAtActionResult.Value);

            context.ChangeTracker.Clear();

            Assert.Equal(refItem.FullName, item?.FullName);
        }

        [Fact]
        public async Task PutStudent()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();

            var controller = new StudentsController(context);
            long id = context.Students.First().Id;

            var refItem = context.Students.First();
            refItem.FullName = "Student X";

            await controller.PutStudent(refItem.Id, refItem.ToDTO());

            context.ChangeTracker.Clear();

            var item = context.Students.SingleOrDefault(s => s.Id == id);
            Assert.Equal(refItem.FullName, item?.FullName);
        }

        [Fact]
        public async Task PutStudent_BadRequest()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();

            var controller = new StudentsController(context);

            var refItem = context.Students.First();
            refItem.FullName = "Student X";

            var result = await controller.PutStudent(refItem.Id + 1, refItem.ToDTO());

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutStudent_NotFound()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();

            var controller = new StudentsController(context);

            var refItem = new StudentDTO
            {
                Id = 9999,
                FullName = "A",
                Email = "A@contoso.com"
            };

            var result = await controller.PutStudent(9999, refItem);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteStudent()
        {
            using var context = Fixture.CreateContext();
            context.Database.BeginTransaction();

            var controller = new StudentsController(context);
            int count = context.Students.Count() - 1;
            long id = context.Students.First().Id;

            await controller.DeleteStudent(id);

            context.ChangeTracker.Clear();

            var item = context.Students.SingleOrDefault(s => s.Id == id);
            Assert.Null(item);
            Assert.Equal(count, context.Students.Count());
        }
    }
}