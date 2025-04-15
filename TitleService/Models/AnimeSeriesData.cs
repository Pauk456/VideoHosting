using System.ComponentModel.DataAnnotations.Schema;

namespace TitleService.Models
{
    public class AnimeSeriesData
    {
        public AnimeSeriesData(int id, string name)
        {
            Name = name;
            SeriesId = id;
        }

        public int SeriesId { get; set; }
        public string Name { get; set; }
    }
}