using System.Text.Json;

namespace GatewayService.Models;
public class ManyTitleDTO
{
    public JsonElement? reccomends { get; set; }
    public JsonElement? all { get; set; }
    public JsonElement? recent { get; set; }
}