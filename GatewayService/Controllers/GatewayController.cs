namespace GatewayService.Controllers;

using Microsoft.AspNetCore.Mvc;
using GatewayService.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Linq.Expressions;

[ApiController]
[Route("api/")]
public class GatewayController : Controller
{
    private readonly HttpClient _httpClient;
    public GatewayController() 
    {
        _httpClient = new HttpClient();
    }

    [HttpPost("videos/search")]
    public async Task<ActionResult<VideoData>> GetVideosBySearch([FromBody] VideoSearchDTO filter)
    {
        throw new NotImplementedException();
    }
    [HttpPost("auth/register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO request)
    {
        throw new NotImplementedException();
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка при попытке удаления из TitleService");
            Console.ResetColor();
        }

        //var content = new StringContent(JsonSerializer.Serialize(seriesId), Encoding.UTF8, "application/json");
        //var deleteRecommendationResponse = await _httpClient.PostAsync($"{ServicesAddresses.uriRecommendationService}/deleteTitle", content);

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
        } catch(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка при попытке добавления в TitleService");
            Console.ResetColor();
        }

        // Отправка в RecommendationService
        //var data = new { idTitle = seriesId };
        //var json = JsonSerializer.Serialize(data);
        //var strContent = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var titleDto = new TitleDTO
            {
                TitleName = request.Title,
                titleId = seriesId
            };
            var searchServiceJsonString = JsonSerializer.Serialize(titleDto);
            var contentSearch = new StringContent(searchServiceJsonString, Encoding.UTF8, "application/json");
            var responseSearch = await _httpClient.PostAsync(ServicesAddresses.uriSearchService + "/postTitle", contentSearch);
            responseSearch.EnsureSuccessStatusCode();
        } catch(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка при попытке добавления в SearchService" + ex.Message);
            Console.ResetColor();
        }

        Console.WriteLine($"Добавился новый Title: {request.Title}");

        return Ok(seriesId);
    }
   
    [HttpPost("auth/login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO request)
    {
        throw new NotImplementedException();
    }

    [HttpGet("Test")]
    public string Test()
    {
        return "Hello from Gataway!";
    }
}


