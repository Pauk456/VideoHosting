using System.Text.Json.Serialization;

namespace GatewayService.Models;
public class TitleDTO
{
    public string? TitleName { get; set; }
    public int? titleId { get; set; }
}