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
[EnableCors("AllowAll")] // Явно применяем политику CORS
[Route("api/img")]
public class ImgController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    private readonly HttpClient _httpClient;

    public ImgController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
    }

    //посылается id аниме сериала чтобы получить картинку
    [HttpGet("{id}")]
    public async Task<IActionResult> GetImg(int id)
    {
        var series = await _context.AnimeSeries.FindAsync(id);

        var absoluteUri = new Uri(new Uri("http://host.docker.internal:4999"), $"get-img?filePath={series.PreviewPath}");

        // Используем относительный путь!
        var response = await _httpClient.GetAsync(
            absoluteUri,
            HttpCompletionOption.ResponseHeadersRead
        );

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode);
        }

        return File(await response.Content.ReadAsStreamAsync(), "image/jpeg");
    }
}