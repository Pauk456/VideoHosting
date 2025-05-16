using System.ComponentModel.DataAnnotations.Schema;

namespace TitleService.Models
{
    public class NewEpisodeData
    {
        public NewEpisodeData(int episodeNumber, int id)
        {
            EpisodeNumber = episodeNumber;
            TitleId = id;
        }

        public int EpisodeNumber { get; set; }
        public int TitleId { get; set; }
    }
}