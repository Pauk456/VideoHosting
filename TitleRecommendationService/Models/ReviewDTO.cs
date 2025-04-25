using System.ComponentModel.DataAnnotations.Schema;

namespace TitleRecommendationService.Models
{
	public class ReviewDTO
	{
		public int IdTitle { get; set; }
		public float? Rating { get; set; }
	}
}