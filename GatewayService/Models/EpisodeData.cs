namespace GatewayService.Models
{
    public class EpisodeData
    {
        public int EpisodeNumber { get; set; }
        public int Id { get; set; }

        public EpisodeData(int id, int episodeNumber)
        {
            this.Id = id;
            this.EpisodeNumber = episodeNumber;
        }
    }
}
