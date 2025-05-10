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
    private readonly string uriServerInteraction = "http://host.docker.internal:4999";
    private readonly string uriRecommendationService = "http://recommendationservice:5007/controller";
    private readonly string uriSearchService = "http://searchservice:5000";

    public FileStructureUpdateService(
        IServiceProvider serviceProvider,
        ILogger<FileStructureUpdateService> logger) // Внедрение логгера
    {
        _serviceProvider = serviceProvider;
        _httpClient = new HttpClient();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Сервис обновления структуры файлов запущен.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Начало нового цикла обновления...");

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Логируем запрос к серверу
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

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
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
                _logger.LogInformation($"Удаление сериала: {series.Title} (ID: {series.Id})");

                // Удаление в микросервисе Димы
                var deleteTitleResponse = await _httpClient.DeleteAsync($"{uriTitleService}/deleteSeries/{series.Id}");
                _logger.LogInformation($"Микросервис TitleService. Статус удаления: {deleteTitleResponse.StatusCode}");

                // Удаление в микросервисе Игоря
                var content = new StringContent(JsonSerializer.Serialize(series.Id), Encoding.UTF8, "application/json");
                var deleteRecommendationResponse = await _httpClient.PostAsync($"{uriRecommendationService}/deleteTitle", content);
                _logger.LogInformation($"Микросервис RecommendationService. Статус удаления: {deleteRecommendationResponse.StatusCode}");

                // Удаление в микросервисе Вовы
                var deleteSearchResponse = await _httpClient.DeleteAsync($"{uriSearchService}/deleteTitle/{series.Id}");
                _logger.LogInformation($"Микросервис SearchService. Статус удаления: {deleteSearchResponse.StatusCode}");

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
            _logger.LogInformation($"Обработка сериала: {seriesDto.Title}");

            var existingSeries = await dbContext.AnimeSeries
                .Include(s => s.Seasons)
                .ThenInclude(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Title == seriesDto.Title);

            if (existingSeries == null)
            {
                _logger.LogInformation($"Добавление нового сериала: {seriesDto.Title}");

                // Отправка в TitleService
                string jsonString = JsonSerializer.Serialize<AnimeSeriesDto>(seriesDto);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var responseMessage = await _httpClient.PostAsync(uriTitleService + "/addSeries", content);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError($"Ошибка при добавлении сериала. StatusCode: {responseMessage.StatusCode}");
                    continue;
                }

                string responseString = await responseMessage.Content.ReadAsStringAsync();
                _logger.LogInformation($"Ответ от TitleService: {responseString}");

                int seriesId = 0;
                if (int.TryParse(responseString, out int result))
                {
                    seriesId = result;
                    _logger.LogInformation($"Сериалу присвоен ID: {seriesId}");
                }
                else
                {
                    _logger.LogError("Не удалось распарсить ID сериала.");
                    continue;
                }

                // Отправка в RecommendationService
                var data = new { idTitle = seriesId };
                var json = JsonSerializer.Serialize(data);
                var strContent = new StringContent(json, Encoding.UTF8, "application/json");
                var recommendationServiceResponse = await _httpClient.PostAsync(uriRecommendationService + "/addNewTitle", strContent);

                _logger.LogInformation($"Ответ от RecommendationService: {recommendationServiceResponse.StatusCode}");
                if (recommendationServiceResponse.IsSuccessStatusCode)
                {
                    var responseBody = await recommendationServiceResponse.Content.ReadAsStringAsync();
                    _logger.LogInformation(responseBody);
                }

                // Отправка в SearchService
                var titleDto = new TitleDTO
                {
                    TitleName = seriesDto.Title,
                    TitleId = seriesId
                };
                string searchServiceJsonString = JsonSerializer.Serialize(titleDto);
                var searchServiceContent = new StringContent(searchServiceJsonString, Encoding.UTF8, "application/json");

                try
                {
                    var response = await _httpClient.PostAsync(uriSearchService + "/postTitle", searchServiceContent);
                    _logger.LogInformation($"Ответ от SearchService: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation($"Успех: {responseBody}");
                    }
                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        _logger.LogError($"Ошибка: {response.StatusCode} - {errorResponse}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Ошибка HTTP при обращении к SearchService");
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

            var existingSeason = series.Seasons
                .FirstOrDefault(s => s.SeasonNumber == seasonDto.SeasonNumber);

            if (existingSeason == null)
            {
                _logger.LogInformation($"Добавление нового сезона {seasonDto.SeasonNumber}");

                var addedSeason = new AddedSeason
                {
                    SeasonNumber = seasonDto.SeasonNumber,
                    SeriesId = series.Id
                };

                string jsonString = JsonSerializer.Serialize<AddedSeason>(addedSeason);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var responseMessage = await _httpClient.PostAsync(uriTitleService + "/addSeason", content);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError($"Ошибка при добавлении сезона. StatusCode: {responseMessage.StatusCode}");
                    continue;
                }

                string responseString = await responseMessage.Content.ReadAsStringAsync();
                _logger.LogInformation($"Ответ от TitleService: {responseString}");

                int seasonId = 0;
                if (int.TryParse(responseString, out int result))
                {
                    seasonId = result;
                    _logger.LogInformation($"Сезону присвоен ID: {seasonId}");
                }
                else
                {
                    _logger.LogError("Не удалось распарсить ID сезона.");
                    continue;
                }

                var newSeason = dbContext.Seasons.Find(seasonId);
                ProcessEpisodes(newSeason, seasonDto.Episodes);
            }
            else
            {
                _logger.LogInformation($"Сезон {seasonDto.SeasonNumber} уже существует. Обновление эпизодов...");
                ProcessEpisodes(existingSeason, seasonDto.Episodes);
            }
        }
    }

    private void ProcessEpisodes(Season season, List<EpisodeDto> episodeDtos)
    {
        _logger.LogInformation($"Обработка эпизодов для сезона {season.SeasonNumber} (ID: {season.Id})");

        foreach (var episodeDto in episodeDtos)
        {
            _logger.LogInformation($"Проверка эпизода {episodeDto.EpisodeNumber}");

            var existingEpisode = season.Episodes
                .FirstOrDefault(e => e.EpisodeNumber == episodeDto.EpisodeNumber);

            if (existingEpisode == null)
            {
                _logger.LogInformation($"Добавление нового эпизода {episodeDto.EpisodeNumber}");

                var newEpisode = new AddedEpisode
                {
                    EpisodeNumber = episodeDto.EpisodeNumber,
                    FilePath = episodeDto.FilePath,
                    SeasonId = season.Id
                };

                string jsonString = JsonSerializer.Serialize<AddedEpisode>(newEpisode);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                try
                {
                    var responseMessage = _httpClient.PostAsync(uriTitleService + "/addEpisode", content).Result;
                    _logger.LogInformation($"Ответ от TitleService: {responseMessage.StatusCode}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при добавлении эпизода");
                }
            }
            else
            {
                _logger.LogInformation($"Эпизод {episodeDto.EpisodeNumber} уже существует. Обновление FilePath...");
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