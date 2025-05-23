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
        try
        {
            var item = _contex.SearchData.Find(1); // ���� �� idSearch

            if (item == null)
            {
                return "�������� �����: ������ � id=1 �� �������";
            }

            return item.TitleTag ?? "������ �������, �� TitleTag ������";
        }
        catch (Exception ex)
        {
            // �� ������ ������ ����������� � ��
            return $"������ ��� ��������: {ex.Message}";
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
            await _contex.Database.ExecuteSqlRawAsync(
                "DELETE FROM searchData WHERE title_id = {0}",
                TitleId
            );

            Console.WriteLine($"������ Title: {TitleId}");
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
        {
            return BadRequest("Title name and title id is required");
        }

        try
        {
            await _contex.Database.ExecuteSqlRawAsync(
                "INSERT INTO searchData (title_id, title_tag) VALUES ({0}, {1})",
                request.TitleId, request.TitleName.ToLower()
            );

            var title = request.TitleName.ToLower();
            var maxTokenLength = title.Length;

            for (int length = 1; length <= maxTokenLength; length++)
            {
                var token = title.Substring(0, length);

                var exists = await _contex.SearchData
                    .AnyAsync(s => s.TitleId == request.TitleId && s.TitleTag == token);

                if (!exists)
                {
                    await _contex.Database.ExecuteSqlRawAsync(
                        "INSERT INTO searchData (title_id, title_tag) VALUES ({0}, {1})",
                        request.TitleId, token
                    );
                }
            }
            Console.WriteLine($"������� ����� Title: {request.TitleName}");
            return Ok("Success post in search service");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return BadRequest("Error in post in search service");
        }
    }
}
