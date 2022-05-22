using CoursesApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.Collections.Generic;
using System;

namespace CoursesApi.IntegrationTests
{
    public class CoursesApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        public CustomWebApplicationFactory<Program> Factory { get; }
        public HttpClient Client { get; }

        public CoursesApiTests(CustomWebApplicationFactory<Program> factory)
        {
            Factory = factory;
            Client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetStudents()
        {
            var response = await Client.GetAsync("/api/Students");

            response.EnsureSuccessStatusCode();
            
            var stringResult = await response.Content.ReadAsStringAsync();
            StudentDTO[]? students = JsonSerializer.Deserialize<StudentDTO[]>(stringResult);
            
            Assert.NotNull(students);
            Assert.Equal(Factory.Students.Count, students!.Length);
        }

        [Fact]
        public async Task PostStudent_ValidationFailed()
        {
            var item = new StudentDTO
            {
                FullName = "x",
                Email = "x",
                Version = new byte[] { 0 }
            };

            //context.Database.BeginTransaction();
            
            var response = await Client.PostAsync("/api/Students", Helper.GetStringContent(item));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var stringResult = await response.Content.ReadAsStringAsync();
            var errors = Helper.GetValidationErrors(stringResult);

            Assert.Equal(2, errors.Count);
            Assert.True(errors.ContainsKey("Email"));
            Assert.True(errors.ContainsKey("FullName"));
        }

        [Theory]
        [ClassData(typeof(CoursesInvalidData))]
        public async Task PostCourse_ValidationFailed(TermDTO course, string fieldName)
        {
            //context.Database.BeginTransaction();
            course.StudentId = Factory.Students[0].Id;

            var response = await Client.PostAsync($"/api/Terms", Helper.GetStringContent(course));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var stringResult = await response.Content.ReadAsStringAsync();
            var errors = Helper.GetValidationErrors(stringResult);

            if (!string.IsNullOrEmpty(fieldName))
            {
                Assert.Single(errors);
                Assert.True(errors.ContainsKey(fieldName));
            }
        }
    }

    public class CoursesInvalidData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // date range
            yield return new object[]
            {
                new TermDTO { StartDate = new DateTime(2022, 4, 25), EndDate = new DateTime(2022, 4, 25), Version = new byte[] { 0 } },
                "EndDate"
            };
            // wrong start day
            yield return new object[]
            {
                new TermDTO { StartDate = new DateTime(2022, 4, 26), EndDate = new DateTime(2022, 4, 29), Version = new byte[] { 0 } },
                "StartDate"
            };
            // wrong end day
            yield return new object[]
            {
                new TermDTO { StartDate = new DateTime(2022, 4, 25), EndDate = new DateTime(2022, 4, 28), Version = new byte[] { 0 } },
                "EndDate"
            };
            // overlapped
            yield return new object[]
            {
                new TermDTO { StartDate = new DateTime(2022, 4, 11), EndDate = new DateTime(2022, 4, 15), Version = new byte[] { 0 } },
                ""
            };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}