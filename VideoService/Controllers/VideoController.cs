using Microsoft.AspNetCore.Mvc;

namespace VideoService.Controllers;

using VideoService.Models;

[ApiController]
[Route("api/video")]
public class VideoController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<VideoData>> GetVideo(int id)
    {
        throw new NotImplementedException();
    }

    // Вовращает int тот айдишник по которому видео загружено и http код возврата
    [HttpPost]
    public async Task<ActionResult<int>> UploadVideo([FromBody] VideoData request)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteVideo(int id)
    {
        throw new NotImplementedException();
    }
}