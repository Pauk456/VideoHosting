
using Microsoft.AspNetCore.Mvc;
using TitleService.Models;
using TitleService.DbModels;

[ApiController]
[Route("api/title")]
public class TitleController : Controller
{
    private readonly ApplicationDbContext _context;


    public TitleController(ApplicationDbContext context)
    {
        _context = context;
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

    //тут мб лучше titleid и seasonsid и seriesid передавать через FormBody
    //[HttpPost("photo")] 
    //[HttpPost("seasons")] 
    //[HttpPost("series")]

    //[HttpPost("title")] // Чисто одним постом запостить и сезоны и серии
}
