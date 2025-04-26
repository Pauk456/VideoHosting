namespace ServerInteraction.Models
{
    public class SeasonDto
    {
        public int SeasonNumber { get; set; }
        public List<EpisodeDto> Episodes { get; set; }
    }
}