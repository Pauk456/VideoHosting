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
    private readonly ILogger<FileStructureUpdateService> _logger; // Добавлен логгер

    private readonly string uriTitleService = "http://titleservice:5006/api/title";
    private readonly string uriGateWayService = "http://gatewayservice:5004/api/inner";
    private readonly string uriServerInteraction = "http://host.docker.internal:4999";

    public FileStructureUpdateService(
        IServiceProvider serviceProvider,
        ILogger<FileStructureUpdateService> logger) //логгерование чтобы понять почему эта фигня не работает
    {
        _serviceProvider = serviceProvider;
        _httpClient = new HttpClient();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Начало нового цикла обновления...");

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                _logger.LogInformation($"Запрос структуры файлов: {uriServerInteraction}/get-structure");
                var response = await _httpClient.GetAsync(uriServerInteraction + "/get-structure", stoppingToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Ошибка при запросе структуры. StatusCode: {response.StatusCode}");
                    continue;
                }

                var content = await response.Content.ReadAsStringAsync(stoppingToken);
                _logger.LogInformation("Структура файлов получена. Десериализация...");

                var receivedSeries = JsonSerializer.Deserialize<List<AnimeSeriesDto>>(content);
                _logger.LogInformation($"Получено {receivedSeries?.Count ?? 0} сериалов.");

                await AddToDatabase(dbContext, receivedSeries);
                await RemoveMissingData(dbContext, receivedSeries);
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP ошибка при обновлении структуры.");
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Ошибка десериализации JSON.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неизвестная ошибка в основном цикле.");
            }

            await Task.Delay(30000, stoppingToken);
        }
    }

    public async Task RemoveMissingData(ApplicationDbContext dbContext, List<AnimeSeriesDto> receivedSeries)
    {
        _logger.LogInformation("Начало удаления отсутствующих данных...");

        var existingSeries = await dbContext.AnimeSeries
            .Include(s => s.Seasons)
                .ThenInclude(season => season.Episodes)
            .ToListAsync();

        var receivedTitles = receivedSeries.Select(s => s.Title).ToHashSet();

        foreach (var series in existingSeries)
        {
            if (!receivedTitles.Contains(series.Title))
            {

                var deleteResponse = await _httpClient.DeleteAsync($"{uriGateWayService}/deleteSeries/{series.Id}");
                deleteResponse.EnsureSuccessStatusCode();

                _logger.LogInformation($"Удаление сериала: {series.Title} (ID: {series.Id})");

                continue;
            }

            var seriesDto = receivedSeries.First(s => s.Title == series.Title);
            var receivedSeasonNumbers = seriesDto.Seasons.Select(s => s.SeasonNumber).ToHashSet();

            foreach (var season in series.Seasons.ToList())
            {
                if (!receivedSeasonNumbers.Contains(season.SeasonNumber))
                {
                    _logger.LogInformation($"Удаление сезона: {season.SeasonNumber} (ID: {season.Id})");
                    var deleteSeasonResponse = await _httpClient.DeleteAsync($"{uriTitleService}/deleteSeason/{season.Id}");
                    _logger.LogInformation($"Микросервис TitleService. Статус удаления сезона: {deleteSeasonResponse.StatusCode}");
                    continue;
                }

                var seasonDto = seriesDto.Seasons.First(s => s.SeasonNumber == season.SeasonNumber);
                var receivedEpisodeNumbers = seasonDto.Episodes.Select(e => e.EpisodeNumber).ToHashSet();

                foreach (var episode in season.Episodes.ToList())
                {
                    if (!receivedEpisodeNumbers.Contains(episode.EpisodeNumber))
                    {
                        _logger.LogInformation($"Удаление эпизода: {episode.EpisodeNumber} (ID: {episode.Id})");
                        var deleteEpisodeResponse = await _httpClient.DeleteAsync($"{uriTitleService}/deleteEpisode/{episode.Id}");
                        _logger.LogInformation($"Микросервис TitleService. Статус удаления эпизода: {deleteEpisodeResponse.StatusCode}");
                    }
                }
            }
        }

        await dbContext.SaveChangesAsync();
        _logger.LogInformation("Удаление отсутствующих данных завершено.");
    }

    public async Task AddToDatabase(ApplicationDbContext dbContext, List<AnimeSeriesDto> receivedSeries)
    {
        _logger.LogInformation("Начало добавления новых данных...");

        foreach (var seriesDto in receivedSeries)
        {
            _logger.LogInformation($"\x1b[36mОбработка сериала: {seriesDto.Title}\x1b[0m");

            var existingSeries = await dbContext.AnimeSeries
                .Include(s => s.Seasons)
                .ThenInclude(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Title == seriesDto.Title);

            if (existingSeries == null)
            {
                _logger.LogInformation($"Добавление нового сериала: {seriesDto.Title}");

                string jsonString = JsonSerializer.Serialize<AnimeSeriesDto>(seriesDto);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var responseMessage = await _httpClient.PostAsync(uriGateWayService + "/addTitle", content);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError($"Ошибка при добавлении сериала. StatusCode: {responseMessage.StatusCode}");
                    continue;
                }


                string responseString = await responseMessage.Content.ReadAsStringAsync();

                int seriesId = 0;
                if (int.TryParse(responseString, out int result))
                {
                    seriesId = result;
                }

                var newSeries = dbContext.AnimeSeries.Find(seriesId);
                await ProcessSeasons(dbContext, newSeries, seriesDto.Seasons);
            }
            else
            {
                _logger.LogInformation($"Сериал {seriesDto.Title} уже существует. Обновление данных...");

                if (!string.IsNullOrEmpty(seriesDto.PreviewPath))
                {
                    existingSeries.PreviewPath = seriesDto.PreviewPath;
                    _logger.LogInformation($"Обновлен PreviewPath для {seriesDto.Title}");
                }

                await ProcessSeasons(dbContext, existingSeries, seriesDto.Seasons);
            }
        }
        _logger.LogInformation("Добавление новых данных завершено.");
    }

    private async Task ProcessSeasons(ApplicationDbContext dbContext, AnimeSeries series, List<SeasonDto> seasonDtos)
    {
        _logger.LogInformation($"Обработка сезонов для сериала {series.Title} (ID: {series.Id})");

        foreach (var seasonDto in seasonDtos)
        {
            _logger.LogInformation($"Проверка сезона {seasonDto.SeasonNumber}");

            var existingSeason = await dbContext.Seasons
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s =>
                    s.SeasonNumber == seasonDto.SeasonNumber &&
                    s.SeriesId == series.Id);

            if (existingSeason == null)
            {
                _logger.LogInformation($"Добавление нового сезона {seasonDto.SeasonNumber}");

                try
                {
                    var addedSeason = new AddedSeason
                    {
                        SeasonNumber = seasonDto.SeasonNumber,
                        SeriesId = series.Id
                    };

                    var content = new StringContent(JsonSerializer.Serialize(addedSeason),
                        Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync($"{uriTitleService}/addSeason", content);
                    response.EnsureSuccessStatusCode();

                    var seasonId = int.Parse(await response.Content.ReadAsStringAsync());
                    _logger.LogInformation($"Добавлен сезон ID: {seasonId}");

                    var newSeason = await dbContext.Seasons
                        .Include(s => s.Episodes)
                        .FirstOrDefaultAsync(s => s.Id == seasonId);

                    await ProcessEpisodes(dbContext, newSeason, seasonDto.Episodes);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при добавлении сезона {seasonDto.SeasonNumber}");
                    continue;
                }
            }
            else
            {
                await ProcessEpisodes(dbContext, existingSeason, seasonDto.Episodes);
            }
        }
    }

    private async Task ProcessEpisodes(ApplicationDbContext dbContext, Season season, List<EpisodeDto> episodeDtos)
    {
        _logger.LogInformation($"Обработка эпизодов для сезона {season.SeasonNumber}");

        foreach (var episodeDto in episodeDtos)
        {
            var existingEpisode = await dbContext.Episodes
                .FirstOrDefaultAsync(e =>
                    e.EpisodeNumber == episodeDto.EpisodeNumber &&
                    e.SeasonId == season.Id);

            if (existingEpisode == null)
            {
                _logger.LogInformation($"Добавление эпизода {episodeDto.EpisodeNumber}");

                try
                {
                    var newEpisode = new AddedEpisode
                    {
                        EpisodeNumber = episodeDto.EpisodeNumber,
                        FilePath = episodeDto.FilePath,
                        SeasonId = season.Id
                    };

                    var content = new StringContent(JsonSerializer.Serialize(newEpisode),
                        Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync($"{uriTitleService}/addEpisode", content);
                    response.EnsureSuccessStatusCode();

                    var episodeId = int.Parse(await response.Content.ReadAsStringAsync());
                    _logger.LogInformation($"Добавлен эпизод ID: {episodeId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при добавлении эпизода {episodeDto.EpisodeNumber}");
                }
            }
            else if (existingEpisode.FilePath != episodeDto.FilePath)
            {
                existingEpisode.FilePath = episodeDto.FilePath;
                await dbContext.SaveChangesAsync();
                _logger.LogInformation($"Обновлен путь к файлу для эпизода {episodeDto.EpisodeNumber}");
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