using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SearchService.Models;
using SearchService.DbModels;

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
        try
        {
            var item = _contex.SearchData.Find(1); // Ищем по idSearch

            if (item == null)
            {
                return "Тестовый ответ: запись с id=1 не найдена";
            }

            return item.TitleTag ?? "Запись найдена, но TitleTag пустой";
        }
        catch (Exception ex)
        {
            // На случай ошибок подключения к БД
            return $"Ошибка при проверке: {ex.Message}";
        }
    }

    [HttpPost("getTitleBySearchTag")]
    public async Task<ActionResult<int>> getTitleIdBySearchTag([FromBody] TagTitleDTO request)
    {
        if (string.IsNullOrEmpty(request.TitleSearchTag))
        {
            return BadRequest("Title search tag is required");
        }

        var titleSearchTag = request.TitleSearchTag.ToLower();

        var exactMatch = await _contex.SearchData
            .Where(s => s.TitleTag == titleSearchTag)
            .FirstOrDefaultAsync();

        if (exactMatch != null)
        {
            return Ok(exactMatch.TitleId);
        }

        var partialMatches = await _contex.SearchData
            .Where(s => s.TitleTag.Contains(titleSearchTag))
            .OrderBy(s => s.TitleTag.Length)
            .ToListAsync();

        if (partialMatches.Any())
        {
            return Ok(partialMatches.First().TitleId);
        }

        return NotFound("No title found with the given search tag");
    }

    [HttpDelete("deleteTitle/{TitleId}")]
    public async Task<ActionResult> DeleteTitle(int TitleId)
    {
        try
        {
            var items = _contex.SearchData
                .Where(s => s.TitleId == TitleId)
                .ToList();

            _contex.SearchData.RemoveRange(items);
            await _contex.SaveChangesAsync();

            Console.WriteLine($"Удалил Title: {TitleId}");
            return Ok("Succes post in search service");
        }
        catch
        {
            return BadRequest("Error in post int search service");
        }
    }

    [HttpPost("postTitle")]
    public async Task<ActionResult> PostTitle([FromBody] TitleDTO request)
    {
        if (string.IsNullOrEmpty(request.TitleName) || request.TitleId == null)
            return BadRequest("...");

        try
        {
            var tokenEntry = new SearchData
            {
                TitleId = request.TitleId.Value,
                TitleTag = request.TitleName.ToLower()
            };

            await _contex.SearchData.AddAsync(tokenEntry);

            var title = request.TitleName.ToLower();
            for (int length = 1; length <= title.Length; length++)
            {
                var token = title.Substring(0, length);
                bool exists = await _contex.SearchData
                    .AnyAsync(s => s.TitleId == request.TitleId && s.TitleTag == token);

                if (!exists)
                {
                    tokenEntry = new SearchData
                    {
                        TitleId = request.TitleId.Value,
                        TitleTag = token
                    };
                    await _contex.SearchData.AddAsync(tokenEntry);
                }
            }

            await _contex.SaveChangesAsync();

            Console.WriteLine($"Добавил новый Title: {request.TitleName}");
            return Ok("Success post in search service");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return BadRequest("Error in post in search service");
        }
    }
}
