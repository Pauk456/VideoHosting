namespace ServerInteraction.Models { 
public class AnimeSeriesDto
{
    public string Title { get; set; }
    public string PreviewPath { get; set; }
    public List<SeasonDto> Seasons { get; set; }
}
}

