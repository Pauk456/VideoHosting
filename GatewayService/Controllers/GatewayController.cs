namespace GatewayService.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class GatewayController : Controller
{
    [HttpGet]
    public string Get()
    {
        return "Hello from VideoService!";
    }
}
