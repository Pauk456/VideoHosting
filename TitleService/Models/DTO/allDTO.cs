using System.Text.Json.Serialization;

namespace TitleService.Models.DTO
{
    public class AnimeSeriesDto
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("previewPath")]
        public string? PreviewPath { get; set; }

        [JsonPropertyName("seasons")]
        public List<SeasonDto> Seasons { get; set; } = new List<SeasonDto>();
    }

    public class SeasonDto
    {
        [JsonPropertyName("seasonNumber")]
        public int SeasonNumber { get; set; }

        [JsonPropertyName("episodes")]
        public List<EpisodeDto> Episodes { get; set; } = new List<EpisodeDto>();
    }

    public class AddedSeason
    {
        [JsonPropertyName("seasonNumber")]
        public int SeasonNumber { get; set; }

        [JsonPropertyName("seriesId")]
        public int SeriesId { get; set; }
    }

    public class EpisodeDto
    {
        [JsonPropertyName("episodeNumber")]
        public int EpisodeNumber { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; } = string.Empty;
    }
    public class AddedEpisode
    {
        [JsonPropertyName("episodeNumber")]
        public int EpisodeNumber { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; }

        [JsonPropertyName("seasonId")]
        public int SeasonId { get; set; }
    }
}
