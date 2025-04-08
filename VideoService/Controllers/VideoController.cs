using Microsoft.AspNetCore.Mvc;

namespace VideoService.Controllers;

using FluentFTP;
using FluentFTP.Exceptions;
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

    private const string FtpBaseUrl = "ftp://localhost";
    private const string FtpUser = "test";
    private const string FtpPass = "1234";
    private readonly HttpClient _httpClient;

    public VideoController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVideo(int id)
    {
        var episode = await _context.Episodes.FindAsync(id);

        if (episode == null)
        {
            return NotFound();
        }

        var absoluteUri = new Uri(new Uri("http://localhost:4999"), $"stream-video?filePath={episode.FilePath}");

        // Используем относительный путь!
        var response = await _httpClient.GetAsync(
            absoluteUri,
            HttpCompletionOption.ResponseHeadersRead
        );

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode);
        }

        return File(
            await response.Content.ReadAsStreamAsync(),
            response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream",
            enableRangeProcessing: true
        );
        //var episode = await _context.Episodes.FindAsync(id);
        //if (episode == null) return NotFound();

        //using (var ftp = new FtpClient("localhost", FtpUser, FtpPass, 21))
        //{
        //    // Настройки подключения
        //    ftp.Config.EncryptionMode = FtpEncryptionMode.Explicit;
        //    ftp.Config.ValidateAnyCertificate = true; // Принимаем любой сертификат
        //    ftp.Config.DataConnectionEncryption = true; // Шифрование данных
        //    ftp.Config.DataConnectionType = FtpDataConnectionType.PASV; // Явно указываем PASV

        //    try
        //    {
        //        ftp.Connect();
        //        if (!ftp.FileExists(episode.FilePath))
        //            return NotFound("Файл не найден");

        //        var stream = new MemoryStream();

        //        ftp.DownloadStream(stream, episode.FilePath);

        //        stream.Seek(0, SeekOrigin.Begin);

        //        return File(stream, "video/mp4");
        //    }
        //    catch (FtpException ex)
        //    {
        //        return StatusCode(500, $"FTP error: {ex.Message}");
        //    }
        //}
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