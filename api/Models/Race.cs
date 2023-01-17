namespace api.Models
{
    public class Race
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public Season Season { get; set; }
        public Guid SeasonId { get; set; }
        public List<Result> Results { get; set; }
    }
}
