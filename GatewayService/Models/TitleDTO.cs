using System.Text.Json;
using System.Text.Json.Serialization;

namespace GatewayService.Models;
public class TitleDTO
{
    public string? name { get; set; }
    public int? id { get; set; }
    public JsonElement? description { get; set; }
    public JsonElement? seasons { get; set; }
    public JsonElement? rating { get; set; }
}