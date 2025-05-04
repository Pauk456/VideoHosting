using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DbUpdater.DbModels;
using Microsoft.EntityFrameworkCore;


public class FileStructureUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly HttpClient _httpClient;
    private readonly string uriTitleService = "http://localhost:5006/api/title";
    private readonly string uriServerInteraction = "http://localhost:4999";
    private readonly string uriRecommendationService = "http://localhost:5007/controller";
    private readonly string uriSearchService = "http://localhost:5000";

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

                var response = await _httpClient.GetAsync(uriServerInteraction + "/get-structure", stoppingToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(stoppingToken);

                var receivedSeries = JsonSerializer.Deserialize<List<AnimeSeriesDto>>(content);


                //две эти функции занимаются обновлением данных(одна лечит другая калечит)
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
                // Удаление в микросервисе Димы
                await _httpClient.DeleteAsync($"{uriTitleService}/deleteSeries/{series.Id}");

                // Удаление в микросервисе Игоря
                var content = new StringContent(JsonSerializer.Serialize(series.Id), Encoding.UTF8, "application/json");
                await _httpClient.PostAsync($"{uriRecommendationService}/deleteTitle", content);

                // Удаление в микросервисе Вовы, пока нет,но есть шанс добавки
                await _httpClient.DeleteAsync($"{uriSearchService}/deleteTitle/{series.Id}");

                continue;
            }

            var seriesDto = receivedSeries.First(s => s.Title == series.Title);
            var receivedSeasonNumbers = seriesDto.Seasons.Select(s => s.SeasonNumber).ToHashSet();

            foreach (var season in series.Seasons.ToList())
            {
                if (!receivedSeasonNumbers.Contains(season.SeasonNumber))
                {
                    // Удаление сезона в микросервисе Димы
                    await _httpClient.DeleteAsync($"{uriTitleService}/deleteSeason/{season.Id}");

                    continue;
                }

                var seasonDto = seriesDto.Seasons.First(s => s.SeasonNumber == season.SeasonNumber);
                var receivedEpisodeNumbers = seasonDto.Episodes.Select(e => e.EpisodeNumber).ToHashSet();

                foreach (var episode in season.Episodes.ToList())
                {
                    if (!receivedEpisodeNumbers.Contains(episode.EpisodeNumber))
                    {
                        // Удаление эпизода в микросервисе Димы
                        await _httpClient.DeleteAsync($"{uriTitleService}/deleteEpisode/{episode.Id}");

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
                //отправляются данные на микросервис Димы
                string jsonString = JsonSerializer.Serialize<AnimeSeriesDto>(seriesDto);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var responseMessage = await _httpClient.PostAsync(uriTitleService + "/addSeries", content);
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                int seriesId = 0;
                // Парсим строку в int
                if (int.TryParse(responseString, out int result))
                {
                    seriesId = result;
                    Console.WriteLine($"Получено число: {result}");
                }


                //отправляются данные на микросервис Игоря
                var data = new { idTitle = seriesId };
                var json = JsonSerializer.Serialize(data);
                var strContent = new StringContent(json, Encoding.UTF8, "application/json");
                var recommendationServiceResponse = await _httpClient.PostAsync(uriRecommendationService + "/addNewTitle", strContent);
                if (recommendationServiceResponse.IsSuccessStatusCode)
                {
                    var responseBody = await recommendationServiceResponse.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                }


                //отправляются данные на микросервис Вовы
                var titleDto = new TitleDTO
                {
                    TitleName = seriesDto.Title,
                    TitleId = seriesId
                };
                string searchServiceJsonString = JsonSerializer.Serialize(titleDto);
                var searchServiceContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
                try
                {
                    var response = await _httpClient.PostAsync(uriSearchService + "/postTitle", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Успех: {responseBody}");
                    }
                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Ошибка: {response.StatusCode} - {errorResponse}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Ошибка HTTP: {ex.Message}");
                }

                //теперь смотрим нужно ли обновлять сезоны для этого аниме 
                var newSeries = dbContext.AnimeSeries.Find(seriesId);
                await ProcessSeasons(dbContext, newSeries, seriesDto.Seasons);
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
                var addedSeason = new AddedSeason
                {
                    SeasonNumber = seasonDto.SeasonNumber,
                    SeriesId = series.Id
                };

                string jsonString = JsonSerializer.Serialize<AddedSeason>(addedSeason);

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var responseMessage = await _httpClient.PostAsync(uriTitleService + "/addSeason", content);

                string responseString = await responseMessage.Content.ReadAsStringAsync();

                int seasonId = 0;

                // Парсим строку в int
                if (int.TryParse(responseString, out int result))
                {
                    seasonId = result;
                    Console.WriteLine($"Получено число: {result}");
                }

                var newSeason = dbContext.Seasons.Find(seasonId);

                ProcessEpisodes(newSeason, seasonDto.Episodes);
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
                var newEpisode = new AddedEpisode
                {
                    EpisodeNumber = episodeDto.EpisodeNumber,
                    FilePath = episodeDto.FilePath,
                    SeasonId = season.Id
                };

                string jsonString = JsonSerializer.Serialize<AddedEpisode>(newEpisode);

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var responseMessage = _httpClient.PostAsync(uriTitleService + "/addEpisode", content);
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

    public class TitleDTO
    {
        public string? TitleName { get; set; }
        public int? TitleId { get; set; }
    }

    public class AddedEpisode
    {
        [JsonPropertyName("episodeNumber")]
        public int EpisodeNumber { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; }

        [JsonPropertyName("seasonId")]
        public int SeasonId { get; set; }
    }

    public class AddedSeason
    {
        [JsonPropertyName("seasonNumber")]
        public int SeasonNumber { get; set; }

        [JsonPropertyName("seriesId")]
        public int SeriesId { get; set; }
    }
}