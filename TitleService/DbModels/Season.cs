﻿using System.ComponentModel.DataAnnotations.Schema;

namespace TitleService.DbModels;
[Table("seasons")]
public class Season
{
    [Column("id")]
    public int Id { get; set; }

    [Column("season_number")]
    public int SeasonNumber { get; set; }

    [Column("series_id")]
    public int SeriesId { get; set; }

    public AnimeSeries Series { get; set; }

    public List<Episode> Episodes { get; set; } = new();
}
