var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Urls.Add("http://localhost:4999");

app.MapGet("/stream-video", async (HttpContext context, string filePath) =>
{
    var basePath = @"D:\fileZilla";
    var fullPath = Path.GetFullPath(Path.Combine(basePath, filePath.Substring(1)));

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

// Парсинг заголовка Range
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