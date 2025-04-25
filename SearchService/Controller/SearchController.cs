using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SearchService.Models;

namespace SearchService.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController : Controller
{
    private readonly ApplicationDbContext _contex;
    public SearchController(ApplicationDbContext context)
    {
        _contex = context;
    }
    [HttpGet("test")]
    public string Test()
    {
        var item = _contex.SearchData.Find(1); // »щем по idSearch
        return item?.TitleTag ?? "таблица не найдена или запись пуста€";
    }

    [HttpPost("getTitleBySearchTag")]
    public async Task<ActionResult<int>> getTitleIdBySearchTag([FromBody] TagTitleDTO request)
    {
        if (string.IsNullOrEmpty(request.TitleSearchTag))
        {
            return BadRequest("Title search tag is required");
        }

        var query = $"SELECT * FROM searchData WHERE title_tag = @p0 LIMIT 1";
        var searchItem = await _contex.SearchData
            .FromSqlRaw(query, request.TitleSearchTag)
            .FirstOrDefaultAsync();

        if (searchItem == null)
        {
            return NotFound("No title found with the given search tag");
        }

        return Ok(searchItem.TitleId);
    }

    [HttpPost("postTitle")]
    public async Task<ActionResult> PostTitle([FromBody] TitleDTO request)
    {
        if (string.IsNullOrEmpty(request.TitleName) || request.TitleId == null)
        {
            return BadRequest("Title name and title id is required");
        }

        try
        {
            await _contex.Database.ExecuteSqlRawAsync(
                "INSERT INTO searchData (title_id, title_tag) VALUES ({0}, {1})",
                request.TitleId, request.TitleName
            );
            return Ok("Succes post in search service");
        }
        catch
        {
            return BadRequest("Error in post int search service");
        }
    }
}
