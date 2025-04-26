using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TitleRecommendationService.DbModels;
[Table("titlerating")]
public class TitleRating
{
	[Key]
	[Column("idtitle")]
	public int IdTitle { get; set; }

	[Column("rating")]
	public float? Rating { get; set; }

	[Column("countreviews")]
	public int CountReviews { get; set; }

}