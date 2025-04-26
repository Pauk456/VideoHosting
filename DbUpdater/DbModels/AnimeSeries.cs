using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbUpdater.DbModels;
[Table("anime_series")]
public class AnimeSeries
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("preview_path")]
    public string PreviewPath { get; set; }

    [Column("title")]
    public string Title { get; set; }

    public List<Season> Seasons { get; set; } = new();
}