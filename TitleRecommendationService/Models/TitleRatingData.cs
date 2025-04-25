using System.ComponentModel.DataAnnotations.Schema;

namespace TitleRecommendationService.Models
{
    public class TitleRatingData
    {
        public int IdTitle { get; set; }
        public float? Rating { get; set; }
        public int CountReviews { get; set; }
    }
}