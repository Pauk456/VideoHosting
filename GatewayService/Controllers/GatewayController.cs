namespace GatewayService.Controllers;

using Microsoft.AspNetCore.Mvc;
using GatewayService.Models;

[ApiController]
[Route("api/gateway")]
public class GatewayController : Controller
{
    // Как загружать файлы на сервер https://metanit.com/sharp/aspnet6/2.12.php (возможно поможет)
    [HttpPost("videos/search")]
    public async Task<ActionResult<VideoData>> GetVideosBySearch([FromBody] VideoSearchDTO filter)
    {
        throw new NotImplementedException();
    }

    [HttpPost("videos/delete/{id}")]
    public async Task<IActionResult> DeleteVideo(int videoId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("videos/upload")]
    public async Task<IActionResult> UploadVideo([FromBody] VideoDTO video)
    {
        throw new NotImplementedException();
    }

    [HttpPost("auth/register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO request)
    {
        throw new NotImplementedException();
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
