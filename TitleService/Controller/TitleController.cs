
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

        string directoryPath = previewPath.Substring(0, previewPath.LastIndexOf('\\'));

        var absoluteUri = new Uri(new Uri("http://host.docker.internal:4999"), 
            $"get-config?filePath={directoryPath}\\titleConfig.json");

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
    public int SetSeries(AnimeSeriesDto seriesDto)
    {
        var newSeries = new AnimeSeries
        {
            Title = seriesDto.Title,
            PreviewPath = seriesDto.PreviewPath ?? string.Empty,
            Seasons = new List<Season>()
        };

        _context.AnimeSeries.Add(newSeries);

        _context.SaveChangesAsync();

        int newId = 0;
        foreach (var item in _context.AnimeSeries)
        {
            if (item.Title == seriesDto.Title)
            {
                newId = item.Id;
                break;
            }
        }

        Console.WriteLine(newId);

        return newId;
    }

    [HttpPost("addSeason")]
    public int AddSeason(AddedSeason seasonDto)
    {
        var newSeason = new Season
        {
            SeasonNumber = seasonDto.SeasonNumber,
            SeriesId = seasonDto.SeriesId,
            Episodes = new List<Episode>()
        };


        _context.Seasons.Add(newSeason);

        _context.SaveChangesAsync();

        int newId = 0;
        foreach (var item in _context.Seasons)
        {
            if(item.SeasonNumber == seasonDto.SeasonNumber)
            {
                if (item.SeriesId == seasonDto.SeriesId)
                {
                    newId = item.Id;
                    break;
                }
            }
        }

        Console.WriteLine("id сезона" + newId);

        Console.WriteLine("номер сезона" + seasonDto.SeasonNumber);

        return newId;
    }

    [HttpPost("addEpisode")]
    public int AddEpisode(AddedEpisode episodeDto)
    {
        var newEpisode = new Episode
        {
            EpisodeNumber = episodeDto.EpisodeNumber,
            FilePath = episodeDto.FilePath,
            SeasonId = episodeDto.SeasonId
        };

        _context.Episodes.Add(newEpisode);
        _context.SaveChanges();

        int newId = 0;
        foreach (var item in _context.Episodes)
        {
            if (item.EpisodeNumber == episodeDto.EpisodeNumber && item.SeasonId == episodeDto.SeasonId)
            {
                newId = item.Id;
                break;
            }
        }

        Console.WriteLine($"ID эпизода: {newId}");
        Console.WriteLine($"Номер эпизода: {episodeDto.EpisodeNumber}");
        Console.WriteLine($"ID сезона: {episodeDto.SeasonId}");

        return newId;
    }

    [HttpDelete("deleteSeries/{seriesId}")]
    public async Task<IActionResult> DeleteSeries(int seriesId)
    {
        var series = await _context.AnimeSeries
            .Include(s => s.Seasons)
            .ThenInclude(s => s.Episodes)
            .FirstOrDefaultAsync(s => s.Id == seriesId);

        if (series == null)
        {
            return NotFound($"Series with ID {seriesId} not found");
        }

        // Удаляем все связанные эпизоды, затем сезоны, и наконец саму серию
        foreach (var season in series.Seasons)
        {
            _context.Episodes.RemoveRange(season.Episodes);
        }

        _context.Seasons.RemoveRange(series.Seasons);
        _context.AnimeSeries.Remove(series);

        await _context.SaveChangesAsync();

        Console.WriteLine($"Deleted series with ID: {seriesId}");
        return Ok($"Series with ID {seriesId} and all related seasons/episodes deleted successfully");
    }

    [HttpDelete("deleteSeason/{seasonId}")]
    public async Task<IActionResult> DeleteSeason(int seasonId)
    {
        var season = await _context.Seasons
            .Include(s => s.Episodes)
            .FirstOrDefaultAsync(s => s.Id == seasonId);

        if (season == null)
        {
            return NotFound($"Season with ID {seasonId} not found");
        }

        // Удаляем все связанные эпизоды, затем сам сезон
        _context.Episodes.RemoveRange(season.Episodes);
        _context.Seasons.Remove(season);

        await _context.SaveChangesAsync();

        Console.WriteLine($"Deleted season with ID: {seasonId}");
        return Ok($"Season with ID {seasonId} and all related episodes deleted successfully");
    }

    [HttpDelete("deleteEpisode/{episodeId}")]
    public async Task<IActionResult> DeleteEpisode(int episodeId)
    {
        var episode = await _context.Episodes.FindAsync(episodeId);

        if (episode == null)
        {
            return NotFound($"Episode with ID {episodeId} not found");
        }

        _context.Episodes.Remove(episode);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Deleted episode with ID: {episodeId}");
        return Ok($"Episode with ID {episodeId} deleted successfully");
    }

    //тут мб лучше titleid и seasonsid и seriesid передавать через FormBody
    //[HttpPost("seasons")] 
    //[HttpPost("series")]

    //[HttpPost("title")] // Чисто одним постом запостить и сезоны и серии
}