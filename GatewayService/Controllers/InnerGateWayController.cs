namespace GatewayService.Controllers;

using Microsoft.AspNetCore.Mvc;
using GatewayService.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;

[ApiController]
[Route("api/inner")]
public class InnerGateWayController : ControllerBase
{
    private readonly HttpClient _httpClient;
    public InnerGateWayController()
    {
        _httpClient = new HttpClient();
    }

    [HttpDelete("deleteSeries/{seriesId}")]
    public async Task<ActionResult> DeleteTitle(int seriesId)
    {
        Console.WriteLine($"Начало удаление Тайтла с id: {seriesId}");

        try
        {
            var deleteTitleResponse = await _httpClient.DeleteAsync($"{ServicesAddresses.uriTitleService}/deleteSeries/{seriesId}");
            deleteTitleResponse.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при попытке удаления из TitleService");
            return BadRequest();
        }

        try
        {
            var deleteRecommendationResponse = await _httpClient.DeleteAsync($"{ServicesAddresses.uriRecommendationService}/deleteTitle/{seriesId}");

            deleteRecommendationResponse.EnsureSuccessStatusCode();
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Ошибка при попытке удаления из TitleRecommendationService: {ex.Message}");
            return BadRequest();
        }

        try
        {
            var deleteSearchResponse = await _httpClient.DeleteAsync($"{ServicesAddresses.uriSearchService}/deleteTitle/{seriesId}");
            deleteSearchResponse.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка при попытке удаления из SearchService");
            Console.ResetColor();
            return BadRequest();
        }

        Console.WriteLine($"Удаление Тайтла с id: {seriesId}, прошло успешно");

        return Ok();
    }

    [HttpPost("addTitle")]
    public async Task<ActionResult<int>> AddTitle([FromBody] AnimeSeriesDto request)
    {
        Console.WriteLine($"Начало добовление нового Title: {request.Title}");

        int seriesId = 0;
        try
        {
            string jsonString = JsonSerializer.Serialize<AnimeSeriesDto>(request);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var responseMessage = await _httpClient.PostAsync(ServicesAddresses.uriTitleService + "/addSeries", content);
            string responseString = await responseMessage.Content.ReadAsStringAsync();

            if (int.TryParse(responseString, out int result))
            {
                seriesId = result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при попытке добавления в TitleService");
            return BadRequest();
        }

        try
        {
            var responseMessage = await _httpClient.GetAsync(ServicesAddresses.uriRecommendationService + $"/addNewTitle/{seriesId}");

            responseMessage.EnsureSuccessStatusCode();
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Ошибка при попытке добавления в RecommendationService: {ex.Message}");
            return BadRequest();
        }

        try
        {
            var titleDto = new
            {
                TitleName = request.Title,
                titleId = seriesId
            };
            var searchServiceJsonString = JsonSerializer.Serialize(titleDto);
            var contentSearch = new StringContent(searchServiceJsonString, Encoding.UTF8, "application/json");
            var responseSearch = await _httpClient.PostAsync(ServicesAddresses.uriSearchService + "/postTitle", contentSearch);
            responseSearch.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка при попытке добавления в SearchService" + ex.Message);
            Console.ResetColor();
            return BadRequest();
        }

        Console.WriteLine($"Добавился новый Title: {request.Title}");

        return Ok(seriesId);
    }
}
