namespace formula1_tournament_api.Models
{
    public class Racer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid teamId { get; set; }
    }
}
