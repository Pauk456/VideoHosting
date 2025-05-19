using System.Text.Json.Serialization;

namespace GatewayService.Models;
public class SeasonDto
{
    [JsonPropertyName("seasonNumber")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("episodes")]
    public List<EpisodeDto> Episodes { get; set; } = new List<EpisodeDto>();
}
