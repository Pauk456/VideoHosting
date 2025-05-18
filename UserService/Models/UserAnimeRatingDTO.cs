namespace UserService.Models
{
    public class UserAnimeRatingDTO
    {
        public int IdUser { get; set; }
        public int IdAnime { get; set; }
        public float? Rating { get; set; }
    }
}
