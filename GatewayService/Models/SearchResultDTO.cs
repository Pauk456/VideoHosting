using System.Text.Json.Serialization;

namespace GatewayService.Models
{
    public class SearchResultDTO
    {
        [JsonPropertyName("TitleName")]
        public string TitleName { get; set; }

        //[JsonPropertyName("TitleName")]
        //public  TitleName { get; set; }
    }
}
