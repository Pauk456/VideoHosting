using Microsoft.AspNetCore.Mvc;

namespace VideoService.Controllers;

using FluentFTP;
using FluentFTP.Exceptions;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using System.Net;
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

    public VideoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVideo(int id)
    {
        var episode = await _context.Episodes.FindAsync(id);
        if (episode == null) return NotFound();

        using (var ftp = new FtpClient("localhost", FtpUser, FtpPass, 21))
        {
            // Настройки подключения
            ftp.Config.EncryptionMode = FtpEncryptionMode.Explicit;
            ftp.Config.ValidateAnyCertificate = true; // Принимаем любой сертификат
            ftp.Config.DataConnectionEncryption = true; // Шифрование данных
            ftp.Config.DataConnectionType = FtpDataConnectionType.PASV; // Явно указываем PASV

            try
            {
                ftp.Connect();
                if (!ftp.FileExists(episode.FilePath))
                    return NotFound("Файл не найден");

                // Создаем MemoryStream
                var stream = new MemoryStream();

                // Базовая версия DownloadStream без FtpDownloadOptions
                ftp.DownloadStream(stream, episode.FilePath);

                // Перемещаем позицию в начало
                stream.Seek(0, SeekOrigin.Begin);

                // Возвращаем файл (поток закроется автоматически)
                return File(stream, "video/mp4");
            }
            catch (FtpException ex)
            {
                return StatusCode(500, $"FTP error: {ex.Message}");
            }
        }
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