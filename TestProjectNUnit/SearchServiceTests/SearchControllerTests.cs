using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SearchService.Controllers;
using SearchService.Models;
using Microsoft.EntityFrameworkCore;
using SearchService.DbModels;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SearchService.Tests
{
    [TestFixture]
    public class SearchControllerTests
    {
        private SearchController __controller;
        private ApplicationDbContext __context;
        private Mock<ApplicationDbContext> _mockContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase("TestDb_" + Guid.NewGuid()) // уникальное имя для каждого теста
                 .Options;

            __context = new ApplicationDbContext(options); // реальный контекст
            __controller = new SearchController(__context); // передаём его в контроллер
        }

        [TearDown]
        public void TearDown()
        {
            __context.Dispose();
            __controller.Dispose();
            __controller = null;
            _mockContext = null;
        }

        [Test]
        public async Task DeleteTitle()
        {
            var postRequest = new TitleDTO { TitleId = 5, TitleName = "New Anime" };
            await __controller.PostTitle(postRequest);

            await __controller.DeleteTitle(5);

            var count = __context.SearchData.Count(s => s.TitleId == 5);

            Assert.That(count, Is.Zero);
        }

        [Test]
        public async Task Post_GetTitleBySearchTag()
        {
            var postRequest = new TitleDTO { TitleId = 5, TitleName = "New Anime" };
            await __controller.PostTitle(postRequest);

            var request = new TagTitleDTO { TitleSearchTag = "New" };

            var result = await __controller.getTitleIdBySearchTag(request);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var okResult = (OkObjectResult)result.Result;
            var value = okResult.Value as int?;

            Assert.That(value, Is.EqualTo(5));
        }

        [Test]
        public async Task Post_Title()
        {
            if (__context.SearchData == null)
            {
                throw new System.Exception("SearchData is null");
            }

            var request = new TitleDTO { TitleId = 5, TitleName = "New Anime" };
            var result = await __controller.PostTitle(request);

            Assert.That(result, Is.Not.Null);

            var count = await __context.SearchData.CountAsync();

            Assert.That(count, Is.Not.EqualTo(0));
        }
    }
}