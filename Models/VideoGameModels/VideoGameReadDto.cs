namespace DistributedSystemAPI.Models.VideoGameModels
{
    public class VideoGameReadDto
    {
        public int GameId { get; set; }
        public string Name { get; set; } = null!;
        public string Publisher { get; set; } = null!;
        public int YearPublished { get; set; }
        public string System { get; set; } = null!;
        public string Condition { get; set; } = null!;
        public int? PreviousOwnersCount { get; set; }

        public Dictionary<string, string> Links { get; set; } = new();
    }
}
