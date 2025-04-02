
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/title")]
public class TitleController : Controller
{
    [HttpGet("specification")]
    public async Task<ActionResult<Specification>> GetSpecification(/* Body */)
    {
        throw new NotImplementedException();
    }

    //тут мб лучше titleid и seasonsid и seriesid передавать через FormBody
    //[HttpPost("photo")] 
    //[HttpPost("seasons")] 
    //[HttpPost("series")]

    //[HttpPost("title")] // Чисто одним постом запостить и сезоны и серии
}
