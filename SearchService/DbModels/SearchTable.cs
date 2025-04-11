using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchService.DbModels;
[Table("searchtable")]
public class SearchTable
{
    [Key]
    [Column("idsearch")]
    public int IdSearch { get; set; }

    [Column("idanime")]
    public int IdAnime { get; set; }

    [Column("animetag")]
    public string AnimeTag { get; set; }
}