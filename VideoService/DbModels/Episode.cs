using System.ComponentModel.DataAnnotations.Schema;

namespace VideoService.DbModels;
[Table("episodes")]
public class Episode
{
    [Column("id")]
    public int Id { get; set; }

    [Column("file_path")]
    public string FilePath { get; set; }

    [Column("episode_number")]
    public int EpisodeNumber { get; set; }

    [Column("season_id")]
    public int SeasonId { get; set; }

    public Season Season { get; set; }
}
