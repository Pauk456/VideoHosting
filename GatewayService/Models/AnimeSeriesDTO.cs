using System.Text.Json.Serialization;

namespace GatewayService.Models;
public class AnimeSeriesDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("previewPath")]
    public string? PreviewPath { get; set; }

    [JsonPropertyName("seasons")]
    public List<SeasonDto> Seasons { get; set; } = new List<SeasonDto>();
}