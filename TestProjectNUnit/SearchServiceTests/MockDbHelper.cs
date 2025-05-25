using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using SearchService.Models;

public static class MockDbHelper
{
    public static ApplicationDbContext GetMockDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        var context = new ApplicationDbContext(options);

        // Очистка перед каждым тестом
        context.SearchData.RemoveRange(context.SearchData);
        context.SaveChanges();

        return context;
    }

    public static Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        return mockSet;
    }
}