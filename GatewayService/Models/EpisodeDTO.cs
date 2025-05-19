using System.Text.Json.Serialization;

namespace GatewayService.Models;
public class EpisodeDto
{
    [JsonPropertyName("episodeNumber")]
    public int EpisodeNumber { get; set; }

    [JsonPropertyName("filePath")]
    public string FilePath { get; set; } = string.Empty;
}