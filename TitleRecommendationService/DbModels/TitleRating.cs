using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TitleRecommendationService.DbModels;
[Table("title_rating")]
public class TitleRating
{
	[Column("idtitle")]
	public int IdTitle { get; set; }

	[Column("rating")]
	public float? Rating { get; set; }

	[Column("countreviews")]
	public int CountReviews { get; set; }

}