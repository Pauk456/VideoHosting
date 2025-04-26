using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbUpdater.DbModels;
[Table("episodes")]
public class Episode
{
    [Key]
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
