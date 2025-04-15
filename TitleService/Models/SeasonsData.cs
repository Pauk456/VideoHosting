namespace TitleService.Models
{
    public class SeasonsData
    {
        public List<EpisodeData> Episodes { get; set; }
        public int SeasonNumber { get; set; }

        public SeasonsData(int seasonNumber, List<EpisodeData> episodes)
        {
            this.SeasonNumber = seasonNumber;
            this.Episodes = episodes;
        }
    }
}
