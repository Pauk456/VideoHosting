using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using DbUpdater.DbModels;
using Microsoft.EntityFrameworkCore;


public class FileStructureUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly HttpClient _httpClient;

    public FileStructureUpdateService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _httpClient = new HttpClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var response = await _httpClient.GetAsync("http://localhost:4999/get-structure", stoppingToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(stoppingToken);

                var receivedSeries = JsonSerializer.Deserialize<List<AnimeSeriesDto>>(content);


                //две эти функции занимаются обновлением данных(одна лечи другая калечит)
                await AddToDatabase(dbContext, receivedSeries);
                await RemoveMissingData(dbContext, receivedSeries);
            }
            catch { }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    public async Task RemoveMissingData(ApplicationDbContext dbContext, List<AnimeSeriesDto> receivedSeries)
    {
        var existingSeries = await dbContext.AnimeSeries
            .Include(s => s.Seasons)
                .ThenInclude(season => season.Episodes)
            .ToListAsync();

        var receivedTitles = receivedSeries.Select(s => s.Title).ToHashSet();

        foreach (var series in existingSeries)
        {
            if (!receivedTitles.Contains(series.Title))
            {
                dbContext.AnimeSeries.Remove(series);
                continue;
            }

            var seriesDto = receivedSeries.First(s => s.Title == series.Title);
            var receivedSeasonNumbers = seriesDto.Seasons.Select(s => s.SeasonNumber).ToHashSet();

            foreach (var season in series.Seasons.ToList())
            {
                if (!receivedSeasonNumbers.Contains(season.SeasonNumber))
                {
                    dbContext.Seasons.Remove(season);
                    continue;
                }

                var seasonDto = seriesDto.Seasons.First(s => s.SeasonNumber == season.SeasonNumber);
                var receivedEpisodeNumbers = seasonDto.Episodes.Select(e => e.EpisodeNumber).ToHashSet();

                foreach (var episode in season.Episodes.ToList())
                {
                    if (!receivedEpisodeNumbers.Contains(episode.EpisodeNumber))
                    {
                        dbContext.Episodes.Remove(episode);
                    }
                }
            }
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task AddToDatabase(ApplicationDbContext dbContext, List<AnimeSeriesDto> receivedSeries)
    {
        foreach (var seriesDto in receivedSeries)
        {
            // Проверка, существует ли аниме сериал с таким названием
            var existingSeries = await dbContext.AnimeSeries
                .Include(s => s.Seasons)
                .ThenInclude(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Title == seriesDto.Title);

            if (existingSeries == null)
            {
                var newSeries = new AnimeSeries
                {
                    Title = seriesDto.Title,
                    PreviewPath = seriesDto.PreviewPath ?? string.Empty,
                    Seasons = new List<Season>()
                };

                await ProcessSeasons(dbContext, newSeries, seriesDto.Seasons);
                dbContext.AnimeSeries.Add(newSeries);
            }
            else
            {
                if (!string.IsNullOrEmpty(seriesDto.PreviewPath))
                {
                    existingSeries.PreviewPath = seriesDto.PreviewPath;
                }

                await ProcessSeasons(dbContext, existingSeries, seriesDto.Seasons);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task ProcessSeasons(ApplicationDbContext dbContext, AnimeSeries series, List<SeasonDto> seasonDtos)
    {
        foreach (var seasonDto in seasonDtos)
        {
            // Проверка, существует ли сезон с таким номером
            var existingSeason = series.Seasons
                .FirstOrDefault(s => s.SeasonNumber == seasonDto.SeasonNumber);

            if (existingSeason == null)
            {
                var newSeason = new Season
                {
                    SeasonNumber = seasonDto.SeasonNumber,
                    SeriesId = series.Id,
                    Episodes = new List<Episode>()
                };

                ProcessEpisodes(newSeason, seasonDto.Episodes);
                series.Seasons.Add(newSeason);
            }
            else
            {
                ProcessEpisodes(existingSeason, seasonDto.Episodes);
            }
        }
    }

    private void ProcessEpisodes(Season season, List<EpisodeDto> episodeDtos)
    {
        foreach (var episodeDto in episodeDtos)
        {
            // Проверка, существует ли эпизод с таким номером
            var existingEpisode = season.Episodes
                .FirstOrDefault(e => e.EpisodeNumber == episodeDto.EpisodeNumber);

            if (existingEpisode == null)
            {
                var newEpisode = new Episode
                {
                    EpisodeNumber = episodeDto.EpisodeNumber,
                    FilePath = episodeDto.FilePath,
                    SeasonId = season.Id
                };

                season.Episodes.Add(newEpisode);
            }
            else
            {
                existingEpisode.FilePath = episodeDto.FilePath;
            }
        }
    }

    public class AnimeSeriesDto
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("previewPath")]
        public string? PreviewPath { get; set; }

        [JsonPropertyName("seasons")]
        public List<SeasonDto> Seasons { get; set; } = new List<SeasonDto>();
    }

    public class SeasonDto
    {
        [JsonPropertyName("seasonNumber")]
        public int SeasonNumber { get; set; }

        [JsonPropertyName("episodes")]
        public List<EpisodeDto> Episodes { get; set; } = new List<EpisodeDto>();
    }

    public class EpisodeDto
    {
        [JsonPropertyName("episodeNumber")]
        public int EpisodeNumber { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; } = string.Empty;
    }
}