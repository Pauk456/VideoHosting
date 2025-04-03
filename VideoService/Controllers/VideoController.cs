using Microsoft.AspNetCore.Mvc;

namespace VideoService.Controllers;

using Microsoft.EntityFrameworkCore;
using VideoService.DbModels;

using VideoService.Models;

[ApiController]
[Route("api/video")]
public class VideoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public VideoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<String>> GetVideo(int id)
    {
        var video = await _context.Episodes.FindAsync(id);

        if (video == null)
        {
            return NotFound();
        }

        return Ok(video.FilePath);
    }

    // Вовращает int тот айдишник по которому видео загружено и http код возврата
    [HttpPost]
    public async Task<ActionResult<int>> UploadVideo([FromBody] VideoInfo request)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteVideo(int id)
    {
        throw new NotImplementedException();
    }
}