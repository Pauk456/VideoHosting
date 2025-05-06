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
        public async Task GetSeasonsAndEpisodes_ReturnsCorrectStructure()
        {
            // Arrange
            var seriesId = 1;

            var seasons = new List<Season>
            {
                new Season { Id = 1, SeasonNumber = 1, SeriesId = seriesId },
                new Season { Id = 2, SeasonNumber = 2, SeriesId = seriesId }
            }.AsQueryable();

            var episodes = new List<Episode>
            {
                new Episode { Id = 1, EpisodeNumber = 1, SeasonId = 1 },
                new Episode { Id = 2, EpisodeNumber = 2, SeasonId = 1 },
                new Episode { Id = 3, EpisodeNumber = 1, SeasonId = 2 }
            }.AsQueryable();

            var mockSeasonSet = new Mock<DbSet<Season>>();
            mockSeasonSet.As<IQueryable<Season>>().Setup(m => m.Provider).Returns(seasons.Provider);
            mockSeasonSet.As<IQueryable<Season>>().Setup(m => m.Expression).Returns(seasons.Expression);
            mockSeasonSet.As<IQueryable<Season>>().Setup(m => m.ElementType).Returns(seasons.ElementType);
            mockSeasonSet.As<IQueryable<Season>>().Setup(m => m.GetEnumerator()).Returns(seasons.GetEnumerator());

            var mockEpisodeSet = new Mock<DbSet<Episode>>();
            mockEpisodeSet.As<IQueryable<Episode>>().Setup(m => m.Provider).Returns(episodes.Provider);
            mockEpisodeSet.As<IQueryable<Episode>>().Setup(m => m.Expression).Returns(episodes.Expression);
            mockEpisodeSet.As<IQueryable<Episode>>().Setup(m => m.ElementType).Returns(episodes.ElementType);
            mockEpisodeSet.As<IQueryable<Episode>>().Setup(m => m.GetEnumerator()).Returns(episodes.GetEnumerator());

            _mockContext.Setup(c => c.Seasons).Returns(mockSeasonSet.Object);
            _mockContext.Setup(c => c.Episodes).Returns(mockEpisodeSet.Object);

            // Act
            var result = await _controller.GetSeasons(seriesId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].SeasonNumber);
            Assert.Equal(2, result[0].Episodes.Count);
            Assert.Equal(2, result[1].SeasonNumber);
            Assert.Single(result[1].Episodes);
        }

        [Fact]
        public void SetSeries_AddsNewSeriesAndReturnsId()
        {
            // Arrange
            var testSeries = new AnimeSeriesDto { Title = "New Series" };
            var mockSet = new Mock<DbSet<AnimeSeries>>();

            var seriesList = new List<AnimeSeries>();
            mockSet.Setup(m => m.Add(It.IsAny<AnimeSeries>())).Callback<AnimeSeries>(s => seriesList.Add(s));

            _mockContext.Setup(c => c.AnimeSeries).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

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
            var mockSet = new Mock<DbSet<Season>>();

            var seasonList = new List<Season>();
            mockSet.Setup(m => m.Add(It.IsAny<Season>())).Callback<Season>(s => seasonList.Add(s));

            _mockContext.Setup(c => c.Seasons).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            // Act
            var result = _controller.addSeason(testSeason);

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
            var mockSet = new Mock<DbSet<Episode>>();

            var episodeList = new List<Episode>();
            mockSet.Setup(m => m.Add(It.IsAny<Episode>())).Callback<Episode>(e => episodeList.Add(e));

            _mockContext.Setup(c => c.Episodes).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _controller.AddEpisode(testEpisode);

            // Assert
            Assert.Equal(0, result);
            Assert.Single(episodeList);
            Assert.Equal(1, episodeList[0].EpisodeNumber);
            Assert.Equal(1, episodeList[0].SeasonId);
            Assert.Equal("path/to/file", episodeList[0].FilePath);
        }

        [Fact]
        public async Task DeleteSeries_RemovesSeriesAndRelatedData()
        {
            // Arrange
            var seriesId = 1;
            var series = new AnimeSeries
            {
                Id = seriesId,
                Seasons = new List<Season>
                {
                    new Season
                    {
                        Id = 1,
                        Episodes = new List<Episode>
                        {
                            new Episode { Id = 1 }
                        }
                    }
                }
            };

            var mockSeriesSet = new Mock<DbSet<AnimeSeries>>();
            var mockSeasonSet = new Mock<DbSet<Season>>();
            var mockEpisodeSet = new Mock<DbSet<Episode>>();

            _mockContext.Setup(c => c.AnimeSeries).Returns(mockSeriesSet.Object);
            _mockContext.Setup(c => c.Seasons).Returns(mockSeasonSet.Object);
            _mockContext.Setup(c => c.Episodes).Returns(mockEpisodeSet.Object);

            _mockContext.Setup(c => c.AnimeSeries.FindAsync(seriesId)).ReturnsAsync(series);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteSeries(seriesId);

            // Assert
            mockSeriesSet.Verify(m => m.Remove(series), Times.Once);
            mockSeasonSet.Verify(m => m.RemoveRange(It.IsAny<IEnumerable<Season>>()), Times.Once);
            mockEpisodeSet.Verify(m => m.RemoveRange(It.IsAny<IEnumerable<Episode>>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteSeason_RemovesSeasonAndEpisodes()
        {
            // Arrange
            var seasonId = 1;
            var season = new Season
            {
                Id = seasonId,
                Episodes = new List<Episode> { new Episode { Id = 1 } }
            };

            var mockSeasonSet = new Mock<DbSet<Season>>();
            var mockEpisodeSet = new Mock<DbSet<Episode>>();

            _mockContext.Setup(c => c.Seasons).Returns(mockSeasonSet.Object);
            _mockContext.Setup(c => c.Episodes).Returns(mockEpisodeSet.Object);

            _mockContext.Setup(c => c.Seasons.FindAsync(seasonId)).ReturnsAsync(season);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteSeason(seasonId);

            // Assert
            mockSeasonSet.Verify(m => m.Remove(season), Times.Once);
            mockEpisodeSet.Verify(m => m.RemoveRange(It.IsAny<IEnumerable<Episode>>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteEpisode_RemovesEpisode()
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

        [Fact]
        public async Task GetSeasons_ReturnsEmptyList_WhenNoSeasonsExist()
        {
            // Arrange
            var seriesId = 1;
            var seasons = new List<Season>().AsQueryable();

            var mockSet = new Mock<DbSet<Season>>();
            mockSet.As<IQueryable<Season>>().Setup(m => m.Provider).Returns(seasons.Provider);
            mockSet.As<IQueryable<Season>>().Setup(m => m.Expression).Returns(seasons.Expression);
            mockSet.As<IQueryable<Season>>().Setup(m => m.ElementType).Returns(seasons.ElementType);
            mockSet.As<IQueryable<Season>>().Setup(m => m.GetEnumerator()).Returns(seasons.GetEnumerator());

            _mockContext.Setup(c => c.Seasons).Returns(mockSet.Object);

            // Act
            var result = await _controller.GetSeasons(seriesId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteSeries_ReturnsNotFound_WhenSeriesDoesNotExist()
        {
            // Arrange
            var seriesId = 999;
            _mockContext.Setup(c => c.AnimeSeries.FindAsync(seriesId)).ReturnsAsync((AnimeSeries)null);

            // Act
            var result = await _controller.DeleteSeries(seriesId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}