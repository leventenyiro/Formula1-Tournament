using System.Drawing;

namespace car_racing_tournament_api.Models
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public Season Season { get; set; }
        public Guid SeasonId { get; set; }
        public List<Driver> Drivers { get; set; }
        public List<Result> Results { get; set; }
    }
}
