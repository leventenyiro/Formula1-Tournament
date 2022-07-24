namespace formula1_tournament_api.Models
{
    public class Race
    {
        public Guid Id { get; set; }
        public int Position { get; set; }
        public int Points { get; set; }
        public Guid RacerId { get; set; }
        public Guid SeasonId { get; set; }
    }
}
