using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;

namespace View.Controllers;

[ApiController]
[Route("[controller]")]
public class ViewController : Controller
{
    private readonly HttpClient _httpClient;

    public ViewController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet("mainPage")]
    public IActionResult MainPage()
    {
        return View();
    }

    [HttpGet("test")]
    public async Task<HttpContent> GetTest()
    {
        var response = await _httpClient.GetAsync("http://localhost:5001/api/video/12"); //  videoservice
        return response.Content;
    }
}
