using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.DbModels;
[Table("useranime")]
public class UserAnime
{
    [Column("iduser")]
    public int IdUser { get; set; }
    
    [Column("idanime")]
    public int IdAnime { get; set; }

    [Column("rating")]
    public float? Rating { get; set; }

    [Column("status")]
    public string Status { get; set; }
}