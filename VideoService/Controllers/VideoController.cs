using Microsoft.AspNetCore.Mvc;

namespace VideoService.Controllers;

using FluentFTP;
using FluentFTP.Exceptions;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using VideoService.DbModels;

using VideoService.Models;

[ApiController]
[Route("api/video")]
public class VideoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    private readonly HttpClient _httpClient;

    public VideoController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet("{id}")]
    [EnableCors("AllowAll")]
    public async Task<IActionResult> GetVideo(int id)
    {
        var episode = await _context.Episodes.FindAsync(id);

        if (episode == null)
        {
            return NotFound();
        }

        var absoluteUri = new Uri(new Uri("http://localhost:4999"), $"stream-video?filePath={episode.FilePath}");

        var response = await _httpClient.GetAsync(
            absoluteUri,
            HttpCompletionOption.ResponseHeadersRead
        );

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode);
        }

        // Получаем Content-Type из исходного ответа
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

        // Создаем ответ с поддержкой диапазонов
        var stream = await response.Content.ReadAsStreamAsync();
        return File(
            stream,
            contentType,
            enableRangeProcessing: true
        );
    }

    private static (long Start, long End, long FileSize)? ParseRange(string rangeHeader)
    {
        try
        {
            var parts = rangeHeader.Replace("bytes=", "").Split('-');
            var start = long.Parse(parts[0]);
            var end = long.Parse(parts[1]);
            return (start, end, end - start + 1);
        }
        catch
        {
            return null;
        }
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