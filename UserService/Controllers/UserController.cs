using Microsoft.AspNetCore.Mvc;

namespace VideoService.Controllers;

using UserService.Models;

[ApiController]
[Route("api/user")]
public class UserController : Controller
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserData>> GetUser(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost("create")]
    public async Task<ActionResult> Register([FromBody] UserData userData)
    {
        throw new NotImplementedException();
    }
}