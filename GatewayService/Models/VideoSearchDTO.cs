using System.Text.Json.Serialization;

namespace GatewayService.Models
{
    public class VideoSearchDTO
    {
        [JsonPropertyName("SearchTag")]
        public string SearchTag { get; set; }
    }
}
