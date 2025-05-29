namespace GatewayService.Controllers;

using Microsoft.AspNetCore.Mvc;
using GatewayService.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Cors;
using static System.Net.WebRequestMethods;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using static GatewayService.Controllers.GatewayController;

[EnableCors("AllowAll")]
[ApiController]
[Route("api/")]
public class GatewayController : ControllerBase
{
    private readonly HttpClient _httpClient;
    public GatewayController() 
    {
        _httpClient = new HttpClient();
    }


    [HttpGet("id={id}&filter={filter}")]
    public async Task<ActionResult<TitleDTO>> GetTitle(string id, string filter)
    {
        if (!int.TryParse(id, out var titleId))
            return BadRequest("Invalid ID");

        var requestedFields = filter?.Split(',')
                             .Select(f => f.Trim())
                             .ToHashSet(StringComparer.OrdinalIgnoreCase)
                             ?? new HashSet<string>();

        var result = new TitleDTO();

        try
        {
            if (requestedFields.Contains("TitleId"))
                result.id = titleId;

            if (requestedFields.Contains("TitleName"))
                result.name = await GetTitleName(titleId);

            if (requestedFields.Contains("Seasons"))
                result.seasons = await GetSeasons(titleId);
             
            if (requestedFields.Contains("Description"))
                result.description = await GetTitleDescription(titleId);

            if (requestedFields.Contains("Raiting"))
                result.rating = await GetRating(titleId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при попытке получить title запрос: id={id}&filter={filter}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("titleTag={titleTag}&filter={filter}")]
    public async Task<ActionResult<TitleDTO>> GetVideosBySearch(string titleTag, string filter)
    {
        int titleId;
        try
        {
            var dto = new
            {
                TitleSearchTag = titleTag
            };

            var searchResponse = await _httpClient.PostAsJsonAsync($"{ServicesAddresses.uriSearchService}/getTitleBySearchTag", dto);

            searchResponse.EnsureSuccessStatusCode();

            var responseString = await searchResponse.Content.ReadAsStringAsync();
            if (!int.TryParse(responseString, out titleId))
            {
                return BadRequest("Некорректный формат ответа от SearchService");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при попытке получить  результат поиска");
            return BadRequest();
        }

        try
        {
            var tittleResponse = await GetTitle(titleId.ToString(), filter);

            return tittleResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при попытке получить ответ от gateway: {ex.Message}");
            return BadRequest();
        }
    }

    [HttpGet("getManyTitles/filter={filter}")]
    public async Task<ActionResult<ManyTitleDTO>> GetManyTitle(string filter)
    {
        var requestedFields = filter?.Split(',')
                            .Select(f => f.Trim())
                            .ToHashSet(StringComparer.OrdinalIgnoreCase)
                            ?? new HashSet<string>();

        var result = new ManyTitleDTO();

        try
        {
            if (requestedFields.Contains("recommends"))
                result.reccomends = await GetReccomends();

            if (requestedFields.Contains("all"))
                result.all = await GetAll();

            if (requestedFields.Contains("recent"))
                result.recent = await GetRecent();

            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка {ex.Message} про обработке запроса: getManyTitle/filter={filter}");
            return StatusCode(500, "Internal server error:");
        }
    }

    [HttpGet("img/{id}")]
    public async Task<IActionResult> GetImgById(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ServicesAddresses.uriVideoAndImageService}/{id}");

            response.EnsureSuccessStatusCode();

            return File(await response.Content.ReadAsStreamAsync(), "image/jpeg");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка при попытке получить изображение из VideoAndImgService");
            Console.ResetColor();
            return BadRequest();
        }
    }

    [HttpGet("video/{id}")]
    public async Task<IActionResult> GetVideo(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ServicesAddresses.uriVideoService}/{id}");

            response.EnsureSuccessStatusCode();

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

            return File(
                fileStream: await response.Content.ReadAsStreamAsync(),
                contentType: contentType,
                enableRangeProcessing: true
            );
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка при попытке получить видео из VideoAndImgService");
            Console.ResetColor();
            return BadRequest();
        }
    }

    [HttpPost("auth/login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO request)
    {
        throw new NotImplementedException();
    }

    [HttpPost("auth/register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO request)
    {
        throw new NotImplementedException();
    }

    [HttpGet("Test")]
    public string Test()
    {
        return "Hello from Gataway!";
    }

    private async Task<string?> GetTitleName(int titleId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ServicesAddresses.uriTitleService}/getAnimeName/{titleId}");
            
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var resultObj = JsonSerializer.Deserialize<AnimeNameResponse>(json);
            var name = resultObj?.name ?? null;

            return name;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении description: {ex.Message}");
            return null;
        }
    }
    private async Task<JsonElement?> GetSeasons(int titleId)
    {
        try
        {
            var configResponse = await _httpClient.GetAsync(
                $"{ServicesAddresses.uriTitleService}/getSeasonsAndEpisodes/{titleId}",
                HttpCompletionOption.ResponseHeadersRead);

            configResponse.EnsureSuccessStatusCode();

            using var stream = await configResponse.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            return doc.RootElement.Clone();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении Seasons: {ex.Message}");
            return null;
        }
    }
    private async Task<JsonElement?> GetTitleDescription(int titleId)
    {
        try
        {
            var configResponse = await _httpClient.GetAsync(
                $"{ServicesAddresses.uriTitleService}/getConfig/{titleId}",
                HttpCompletionOption.ResponseHeadersRead);

            configResponse.EnsureSuccessStatusCode();

            using var stream = await configResponse.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            return doc.RootElement.Clone();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении description: {ex.Message}");
            return null;
        }
    }
    private async Task<JsonElement?> GetRating(int titleId)
    {
        try
        {
            var content = JsonContent.Create(new { idTitle = titleId });

            var response = await _httpClient.GetAsync($"{ServicesAddresses.uriRecommendationService}/getReview/{titleId}");

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            return doc.RootElement.Clone();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении рейтинга: {ex.Message}");
            return null;
        }
    }
    private record DTOgetAll(int seriesId, string name);
    private record AnimeDto(int idTitle, string titleName, float? rating, JsonElement description);
    private class TitleRatingData__
    {
        public int IdTitle { get; set; }
        public float? Rating { get; set; }
        public int CountReviews { get; set; }
    }
    private async Task<JsonElement?> GetAll()
    {
        List<DTOgetAll> titles;
        try
        {
            var response = await _httpClient.GetAsync($"{ServicesAddresses.uriTitleService}/all");

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            titles = await JsonSerializer.DeserializeAsync<List<DTOgetAll>>(stream);

            if (titles == null)
            {
                Console.WriteLine("Не получено данных или пустой список");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении всех аниме: {ex.Message}");
            return null;
        }

        var TitleDTOsend = new List<AnimeDto>();

        foreach (var title in titles)
        { 
            var ratingJson = await GetRating(title.seriesId);

            var animeDto = JsonSerializer.Deserialize<TitleRatingData__>(ratingJson.Value.GetRawText());

            var desc = await GetTitleDescription(title.seriesId) ?? new JsonElement();

            TitleDTOsend.Add(new(
                idTitle: title.seriesId,
                titleName: title.name,
                rating: animeDto.Rating,
            description: desc
            ));
        }

        var jsonElement = JsonSerializer.SerializeToElement(TitleDTOsend);

        return jsonElement;
    }
    private async Task<JsonElement?> GetReccomends()
    {
        List<TitleRatingDto> ratings;

        try
        {
            var response = await _httpClient.GetAsync($"{ServicesAddresses.uriRecommendationService}/top");

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            ratings = await JsonSerializer.DeserializeAsync<List<TitleRatingDto> >(stream);

            if (ratings == null)
            {
                Console.WriteLine("Не получено данных или пустой список");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении рекомендованных аниме: {ex.Message}");
            return null;
        }

        foreach(var raitingDTO in ratings)
        {
            raitingDTO.titleName = await GetTitleName(raitingDTO.idTitle);
        }

        var jsonString = JsonSerializer.Serialize(ratings);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonString);

        return jsonElement;
    }
    private record recentDTO(int idTitle, string titleName, int episodeNumber, JsonElement seasons);
    private record NewEpisodeData____(int episodeNumber, int titleId);

    private async Task<JsonElement?> GetRecent()
    {
        List<NewEpisodeData____> titles;
        try
        {
            var response = await _httpClient.GetAsync($"{ServicesAddresses.uriTitleService}/getRecentEpisodes");

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            titles = await JsonSerializer.DeserializeAsync<List<NewEpisodeData____>>(stream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении недавних аниме: {ex.Message}");
            return null;
        }

        var sendDTO = new List<recentDTO>();

        foreach (var title in titles)
        {
            sendDTO.Add(new (
                idTitle: title.titleId,
                titleName: await GetTitleName(title.titleId),
                episodeNumber: title.episodeNumber,
                seasons: await GetSeasons(title.titleId) ?? new JsonElement()
            )
            );
        }

        var jsonElement = JsonSerializer.SerializeToElement(sendDTO);

        return jsonElement;
    }

    private class AnimeNameResponse
    {
        public string name { get; set; }
    }

    private class TitleRatingDto
    {
        public int idTitle { get; set; }

        public string? titleName { get; set; }

        public float? rating { get; set; }

        public int countReviews { get; set; }
    }

    public class TitleDTOs
    {
        public int idTitle { get; set; }

        public string? titleName { get; set; }

        public float? rating { get; set; }


    }
}
