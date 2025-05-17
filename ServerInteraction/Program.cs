
//ServerInteraction будет отвечать за взаимодействие с сервером.
//“о есть дл€ подгрузки картинок и видео с сервера может ещЄ дл€ какой-нибудь инфы с сервера
using ServerInteraction.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


//app.UseHttpsRedirection();

app.Urls.Add("http://localhost:4999");

//вынесу в контроллер если не будет лень
app.MapGet("/get-structure", (HttpContext context) =>
{
    var basePath = @"D:\Anime\Videos";
    var animeFiles = Directory.GetDirectories(basePath);

    var animeList = new List<AnimeSeriesDto>();

    foreach (var animePath in animeFiles)
    {
        var animeInf = new DirectoryInfo(animePath);
        var animeSeries = new AnimeSeriesDto();

        animeSeries.Title = animeInf.Name;

        var seasonsPath = Directory.GetDirectories(animePath);

        var allFilesInDirectory = Directory.GetFiles(animePath);

        if (allFilesInDirectory.Length != 0)
        {
            foreach (var file in allFilesInDirectory)
            {
                if (Path.GetExtension(file) == ".jpg")
                {
                    animeSeries.PreviewPath = file.Split("D:\\Anime\\")[1];
                }
            }
        }
        else
        {
            animeSeries.PreviewPath = "";
        }

        var seasons = new List<SeasonDto>();

        Console.WriteLine(animeSeries.Title);

        int seasonCount = 1;
        foreach (var seasonPath in seasonsPath)
        {

            var season = new SeasonDto();
            season.SeasonNumber = seasonCount++;

            Console.WriteLine("    " + season.SeasonNumber);

            var seasonInf = new DirectoryInfo(seasonPath);

            var episodesPath = Directory.GetFiles(seasonPath);

            var episodes = new List<EpisodeDto>();

            int episodeCount = 1;

            foreach (var episodePath in episodesPath)
            {
                Console.WriteLine("          " + episodePath);
                var episode = new EpisodeDto();

                episode.EpisodeNumber = episodeCount++;
                episode.FilePath = episodePath.Split("D:\\Anime\\")[1];

                episodes.Add(episode);
            }

            season.Episodes = episodes;
            seasons.Add(season);
        }

        animeSeries.Seasons = seasons;

        animeList.Add(animeSeries);
    }
    return animeList;
});

//вынесу в контроллер если не будет лень
app.MapGet("/get-img", async (HttpContext context, string filePath) =>
{
    var basePath = @"D:\Anime";
    var fullPath = Path.GetFullPath(Path.Combine(basePath, filePath));

    if (!fullPath.StartsWith(basePath))
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Access denied");
        return;
    }
    if (!File.Exists(fullPath))
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("File not found");
        return;
    }

    await using var fullFileStream = File.OpenRead(fullPath);
    await fullFileStream.CopyToAsync(context.Response.Body);

});

app.MapGet("/get-config", async (HttpContext context, string filePath) =>
{
    var basePath = @"D:\Anime";
    var fullPath = Path.GetFullPath(Path.Combine(basePath, filePath));

    if (!fullPath.StartsWith(basePath))
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Access denied");
        return;
    }
    if (!File.Exists(fullPath))
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("File not found");
        return;
    }

    await using var fullFileStream = File.OpenRead(fullPath);
    await fullFileStream.CopyToAsync(context.Response.Body);

});

//вынесу в контроллер если не будет лень
app.MapGet("/stream-video", async (HttpContext context, string filePath) =>
{
    var basePath = @"D:\Anime";
    var fullPath = Path.GetFullPath(Path.Combine(basePath, filePath));

    if (!fullPath.StartsWith(basePath))
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Access denied");
        return;
    }

    if (!File.Exists(fullPath))
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("File not found");
        return;
    }

    var fileInfo = new FileInfo(fullPath);
    long fileLength = fileInfo.Length;

    context.Response.Headers.Add("Accept-Ranges", "bytes");

    if (context.Request.Headers.TryGetValue("Range", out var rangeHeader))
    {
        var range = ParseRange(rangeHeader, fileLength);
        if (range != null)
        {
            context.Response.StatusCode = 206;
            context.Response.Headers.Add("Content-Range", $"bytes {range.Value.Start}-{range.Value.End}/{fileLength}");

            await using var fileStream = File.OpenRead(fullPath);
            fileStream.Seek(range.Value.Start, SeekOrigin.Begin);

            var buffer = new byte[81920];
            long bytesRemaining = range.Value.End - range.Value.Start + 1;

            while (bytesRemaining > 0)
            {
                int bytesRead = await fileStream.ReadAsync(buffer, 0, (int)Math.Min(buffer.Length, bytesRemaining));
                if (bytesRead == 0) break;

                await context.Response.Body.WriteAsync(buffer, 0, bytesRead);
                bytesRemaining -= bytesRead;
            }
            return;
        }
    }
    context.Response.ContentType = "video/mp4";
    context.Response.ContentLength = fileLength;
    await using var fullFileStream = File.OpenRead(fullPath);
    await fullFileStream.CopyToAsync(context.Response.Body);
});

// ѕарсинг заголовка Range
static (long Start, long End)? ParseRange(string rangeHeader, long fileLength)
{
    try
    {
        var range = rangeHeader.Replace("bytes=", "").Split('-');
        long start = long.Parse(range[0]);
        long end = range[1] == "" ? fileLength - 1 : long.Parse(range[1]);
        return (start, end);
    }
    catch
    {
        return null;
    }
}

app.Run();