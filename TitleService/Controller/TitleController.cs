
using Microsoft.AspNetCore.Mvc;
using TitleService.Models;
using TitleService.DbModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TitleService.Models.DTO;
using System.Net.Http;

namespace TitleService.Controllers;

[ApiController]
[EnableCors("AllowAll")]
[Route("api/title")]
public class TitleController : Controller
{
    private readonly ApplicationDbContext _context;

    private readonly HttpClient _httpClient;

    public TitleController(ApplicationDbContext context)
    {
        _context = context;
        _httpClient = new HttpClient();
    }

    // получает всевозможные аниме
    [HttpGet("all")]
    public async Task<List<AnimeSeriesData>> GetAllAnimeSeries()
    {
        var response = new List<AnimeSeriesData>();
        foreach (var item in _context.AnimeSeries)
        {
            response.Add(new AnimeSeriesData(item.Id, item.Title));
        }
        return response;
    }

    //по id сериала получает инфу о сериях и Сезонах
    [HttpGet("getSeasonsAndEpisodes/{id}")]
    public async Task<List<SeasonsData>> GetSeasons(int id)
    {
        var response = new List<SeasonsData>();

        var seasons = new List<Season>();

        foreach (var item in _context.Seasons)
        {
            if (item.SeriesId == id)
            {
                seasons.Add(item);
            }
        }
        foreach (var season in seasons)
        {
            //response.Add(new AnimeSeriesData(item.Id, item.Title));
            var episodes = new List<EpisodeData>();

            foreach (var episode in _context.Episodes)
            {
                if (episode.SeasonId == season.Id)
                {
                    episodes.Add(new EpisodeData(episode.Id, episode.EpisodeNumber));
                }
            }
            response.Add(new SeasonsData(season.SeasonNumber, episodes));
        }
        return response;
    }

    //Описантие тайтла
    [HttpGet("getConfig/{id}")]
    public async Task<IActionResult> GetConfig(int id)
    {
        var series = await _context.AnimeSeries.FindAsync(id);

        var previewPath = series.PreviewPath;

        string directoryPath = previewPath.Substring(0, previewPath.LastIndexOf('/'));

        var absoluteUri = new Uri(new Uri("http://serverinteraction:4999"), 
            $"get-config?filePath={directoryPath}/titleConfig.json");
        Console.WriteLine(absoluteUri);
        var response = await _httpClient.GetAsync(
            absoluteUri,
            HttpCompletionOption.ResponseHeadersRead
        );

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode);
        }

        return File(await response.Content.ReadAsStreamAsync(), "application/json");
    }

    [HttpGet("getAnimeName/{id}")]
    public IActionResult GetAnimeName(int id)
    {
        foreach (var item in _context.AnimeSeries)
        {
            if (item.Id == id)
            {
                return Json(new { name = item.Title });
            }
        }
        return Json(new { name = "" });
    }

    [HttpGet("getRecentEpisodes")]
    public async Task<List<NewEpisodeData>> GetRecentEpisodes()
    {
        var episodes = await _context.Episodes
            .OrderByDescending(e => e.Id) 
            .Take(4)
            .ToListAsync();

        var response = new List<NewEpisodeData>();
        foreach (var item in episodes)
        {
            var season = await _context.Seasons.FindAsync(item.SeasonId);
            response.Add(new NewEpisodeData(item.EpisodeNumber, season?.SeriesId ?? 0));
        }

        return response;
    }

    [HttpPost("addSeries")]
    public async Task<ActionResult<int>> AddSeries(AnimeSeriesDto seriesDto)
    {
        try
        {
            var newSeries = new AnimeSeries
            {
                Title = seriesDto.Title,
                PreviewPath = seriesDto.PreviewPath ?? string.Empty,
                Seasons = new List<Season>()
            };

            await _context.AnimeSeries.AddAsync(newSeries);
            await _context.SaveChangesAsync();

            return Ok(newSeries.Id); // Возвращаем ID сразу после сохранения
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPost("addSeason")]
    public async Task<ActionResult<int>> AddSeason(AddedSeason seasonDto)
    {
        try
        {
            // Проверяем существование сериала
            var seriesExists = await _context.AnimeSeries.AnyAsync(s => s.Id == seasonDto.SeriesId);
            if (!seriesExists)
            {
                return NotFound($"Сериал с ID {seasonDto.SeriesId} не найден");
            }

            var newSeason = new Season
            {
                SeasonNumber = seasonDto.SeasonNumber,
                SeriesId = seasonDto.SeriesId,
                Episodes = new List<Episode>()
            };

            await _context.Seasons.AddAsync(newSeason);
            await _context.SaveChangesAsync();

            return Ok(newSeason.Id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPost("addEpisode")]
    public async Task<ActionResult<int>> AddEpisode(AddedEpisode episodeDto)
    {
        try
        {
            // Проверяем существование сезона
            var seasonExists = await _context.Seasons.AnyAsync(s => s.Id == episodeDto.SeasonId);
            if (!seasonExists)
            {
                return NotFound($"Сезон с ID {episodeDto.SeasonId} не найден");
            }

            var newEpisode = new Episode
            {
                EpisodeNumber = episodeDto.EpisodeNumber,
                FilePath = episodeDto.FilePath,
                SeasonId = episodeDto.SeasonId
            };

            await _context.Episodes.AddAsync(newEpisode);
            await _context.SaveChangesAsync();

            return Ok(newEpisode.Id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpDelete("deleteSeries/{seriesId}")]
    public async Task<IActionResult> DeleteSeries(int seriesId)
    {
        try
        {
            var series = await _context.AnimeSeries
                .Include(s => s.Seasons)
                .ThenInclude(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == seriesId);

            if (series == null)
            {
                return NotFound($"Сериал с ID {seriesId} не найден");
            }

            _context.Episodes.RemoveRange(series.Seasons.SelectMany(s => s.Episodes));
            _context.Seasons.RemoveRange(series.Seasons);
            _context.AnimeSeries.Remove(series);

            await _context.SaveChangesAsync();

            return Ok($"Сериал с ID {seriesId} и все связанные данные удалены");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpDelete("deleteSeason/{seasonId}")]
    public async Task<IActionResult> DeleteSeason(int seasonId)
    {
        try
        {
            var season = await _context.Seasons
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == seasonId);

            if (season == null)
            {
                return NotFound($"Сезон с ID {seasonId} не найден");
            }

            _context.Episodes.RemoveRange(season.Episodes);
            _context.Seasons.Remove(season);

            await _context.SaveChangesAsync();

            return Ok($"Сезон с ID {seasonId} и все эпизоды удалены");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpDelete("deleteEpisode/{episodeId}")]
    public async Task<IActionResult> DeleteEpisode(int episodeId)
    {
        try
        {
            var episode = await _context.Episodes.FindAsync(episodeId);

            if (episode == null)
            {
                return NotFound($"Эпизод с ID {episodeId} не найден");
            }

            _context.Episodes.Remove(episode);
            await _context.SaveChangesAsync();

            return Ok($"Эпизод с ID {episodeId} удален");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    //тут мб лучше titleid и seasonsid и seriesid передавать через FormBody
    //[HttpPost("seasons")] 
    //[HttpPost("series")]

    //[HttpPost("title")] // Чисто одним постом запостить и сезоны и серии
}