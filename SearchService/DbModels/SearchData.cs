using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchService.DbModels;
[Table("searchdata")]
public class SearchData
{
    [Key]
    [Column("search_id")]
    public int SearchId { get; set; }

    [Column("title_id")]
    public int TitleId { get; set; }

    [Column("title_tag")]
    public string TitleTag { get; set; }
}