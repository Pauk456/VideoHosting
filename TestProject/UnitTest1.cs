using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitleService.Controllers;
using TitleService.DbModels;
using TitleService;
using TitleService.Models.DTO;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using TestProject;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TitleService.Tests
{
    public class TitleControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly TitleController _controller;

        public TitleControllerTests()
        {
            _mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            _controller = new TitleController(_mockContext.Object);
        }

        [Fact]
        public async Task GetAllAnimeSeries_ReturnsAllSeries()
        {
            // Arrange
            var testData = new List<AnimeSeries>
            {
                new AnimeSeries { Id = 1, Title = "Series 1" },
                new AnimeSeries { Id = 2, Title = "Series 2" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<AnimeSeries>>();
            mockSet.As<IQueryable<AnimeSeries>>().Setup(m => m.Provider).Returns(testData.Provider);
            mockSet.As<IQueryable<AnimeSeries>>().Setup(m => m.Expression).Returns(testData.Expression);
            mockSet.As<IQueryable<AnimeSeries>>().Setup(m => m.ElementType).Returns(testData.ElementType);
            mockSet.As<IQueryable<AnimeSeries>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());

            _mockContext.Setup(c => c.AnimeSeries).Returns(mockSet.Object);


            // Act
            var result = await _controller.GetAllAnimeSeries();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Series 1", result[0].Name);
            Assert.Equal("Series 2", result[1].Name);
        }

        [Fact]
        public void SetSeries_AddsNewSeriesAndReturnsId()
        {
            // Arrange
            var testSeries = new AnimeSeriesDto { Title = "New Series" };
            var seriesList = new List<AnimeSeries>();

            var mockSet = new Mock<DbSet<AnimeSeries>>();
            mockSet.Setup(m => m.Add(It.IsAny<AnimeSeries>()))
                   .Callback<AnimeSeries>(s => {
                       s.Id = 0; 
                       seriesList.Add(s);
                   })
                   .Returns((AnimeSeries s) =>
                   {
                       return null;
                   });

            var queryable = seriesList.AsQueryable();
            mockSet.As<IQueryable<AnimeSeries>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<AnimeSeries>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<AnimeSeries>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<AnimeSeries>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            _mockContext.Setup(c => c.AnimeSeries).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _controller.SetSeries(testSeries);

            // Assert
            Assert.Equal(0, result);
            Assert.Single(seriesList);
            Assert.Equal("New Series", seriesList[0].Title);
        }

        [Fact]
        public void AddSeason_AddsNewSeasonAndReturnsId()
        {
            // Arrange
            var testSeason = new AddedSeason { SeasonNumber = 1, SeriesId = 1 };
            var seasonList = new List<Season>();

            var mockSet = new Mock<DbSet<Season>>();
            mockSet.Setup(m => m.Add(It.IsAny<Season>()))
                   .Callback<Season>(seasonList.Add)
                   .Returns((Season s) =>
                   {
                       s.Id = 0; 
                       return null; 
                   });

            var queryable = seasonList.AsQueryable();
            mockSet.As<IQueryable<Season>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<Season>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Season>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Season>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            _mockContext.Setup(c => c.Seasons).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _controller.AddSeason(testSeason);

            // Assert
            Assert.Equal(0, result);
            Assert.Single(seasonList);
            Assert.Equal(1, seasonList[0].SeasonNumber);
            Assert.Equal(1, seasonList[0].SeriesId);
        }

        [Fact]
        public void AddEpisode_AddsNewEpisodeAndReturnsId()
        {
            // Arrange
            var testEpisode = new AddedEpisode { EpisodeNumber = 1, SeasonId = 1, FilePath = "path/to/file" };
            var episodeList = new List<Episode>();

            var mockSet = new Mock<DbSet<Episode>>();

            mockSet.Setup(m => m.Add(It.IsAny<Episode>())).Callback<Episode>(e =>
            {
                e.Id = episodeList.Count + 1;
                episodeList.Add(e);
            });
            mockSet.As<IQueryable<Episode>>().Setup(m => m.Provider).Returns(episodeList.AsQueryable().Provider);
            mockSet.As<IQueryable<Episode>>().Setup(m => m.Expression).Returns(episodeList.AsQueryable().Expression);
            mockSet.As<IQueryable<Episode>>().Setup(m => m.ElementType).Returns(episodeList.AsQueryable().ElementType);
            mockSet.As<IQueryable<Episode>>().Setup(m => m.GetEnumerator()).Returns(() => episodeList.GetEnumerator());

            _mockContext.Setup(c => c.Episodes).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _controller.AddEpisode(testEpisode);

            // Assert
            Assert.Equal(1, result);
            Assert.Single(episodeList);
            Assert.Equal(1, episodeList[0].Id);
            Assert.Equal(1, episodeList[0].EpisodeNumber);
            Assert.Equal(1, episodeList[0].SeasonId);
            Assert.Equal("path/to/file", episodeList[0].FilePath);
        }

        [Fact]
        public async Task DeleteSeries()
        {
            // Arrange
            var seriesId = 1;
            var episode = new Episode { Id = 1 };
            var season = new Season
            {
                Id = 1,
                Episodes = new List<Episode> { episode }
            };
            var series = new AnimeSeries
            {
                Id = seriesId,
                Seasons = new List<Season> { season }
            };

            var mockSeriesSet = new Mock<DbSet<AnimeSeries>>();
            var seriesData = new List<AnimeSeries> { series }.AsQueryable();

            mockSeriesSet.As<IQueryable<AnimeSeries>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<AnimeSeries>(seriesData.Provider));
            mockSeriesSet.As<IQueryable<AnimeSeries>>().Setup(m => m.Expression)
                .Returns(seriesData.Expression);
            mockSeriesSet.As<IQueryable<AnimeSeries>>().Setup(m => m.ElementType)
                .Returns(seriesData.ElementType);
            mockSeriesSet.As<IQueryable<AnimeSeries>>().Setup(m => m.GetEnumerator())
                .Returns(seriesData.GetEnumerator());

            mockSeriesSet.As<IAsyncEnumerable<AnimeSeries>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<AnimeSeries>(seriesData.GetEnumerator()));

            var mockSeasonSet = new Mock<DbSet<Season>>();
            var seasonsData = series.Seasons.AsQueryable();

            mockSeasonSet.As<IQueryable<Season>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Season>(seasonsData.Provider));

            var mockEpisodeSet = new Mock<DbSet<Episode>>();
            var episodesData = season.Episodes.AsQueryable();

            mockEpisodeSet.As<IQueryable<Episode>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Episode>(episodesData.Provider));

            _mockContext.Setup(c => c.AnimeSeries).Returns(mockSeriesSet.Object);
            _mockContext.Setup(c => c.Seasons).Returns(mockSeasonSet.Object);
            _mockContext.Setup(c => c.Episodes).Returns(mockEpisodeSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteSeries(seriesId);

            // Assert
            mockSeriesSet.Verify(m => m.Remove(series), Times.Once);
            mockSeasonSet.Verify(m => m.RemoveRange(series.Seasons), Times.Once);
            mockEpisodeSet.Verify(m => m.RemoveRange(season.Episodes), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteSeason()
        {
            // Arrange
            var seasonId = 1;
            var season = new Season
            {
                Id = seasonId,
                Episodes = new List<Episode>()
            };

            var mockSeasons = new Mock<DbSet<Season>>();
            var seasonsData = new List<Season> { season }.AsQueryable();

            mockSeasons.As<IQueryable<Season>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Season>(seasonsData.Provider));
            mockSeasons.As<IQueryable<Season>>().Setup(m => m.Expression)
                .Returns(seasonsData.Expression);
            mockSeasons.As<IQueryable<Season>>().Setup(m => m.ElementType)
                .Returns(seasonsData.ElementType);
            mockSeasons.As<IQueryable<Season>>().Setup(m => m.GetEnumerator())
                .Returns(seasonsData.GetEnumerator());

            mockSeasons.As<IAsyncEnumerable<Season>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Season>(seasonsData.GetEnumerator()));

            var mockEpisodes = new Mock<DbSet<Episode>>();
            var episodesData = season.Episodes.AsQueryable();

            mockEpisodes.As<IQueryable<Episode>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Episode>(episodesData.Provider));

            _mockContext.Setup(c => c.Seasons).Returns(mockSeasons.Object);
            _mockContext.Setup(c => c.Episodes).Returns(mockEpisodes.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteSeason(seasonId);

            // Assert
            mockSeasons.Verify(m => m.Remove(season), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteEpisode()
        {
            // Arrange
            var episodeId = 1;
            var episode = new Episode { Id = episodeId };

            var mockSet = new Mock<DbSet<Episode>>();
            _mockContext.Setup(c => c.Episodes).Returns(mockSet.Object);
            _mockContext.Setup(c => c.Episodes.FindAsync(episodeId)).ReturnsAsync(episode);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteEpisode(episodeId);

            // Assert
            mockSet.Verify(m => m.Remove(episode), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }


    }
}