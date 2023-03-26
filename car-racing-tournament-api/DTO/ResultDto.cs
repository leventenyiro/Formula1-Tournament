using System.Text.Json.Serialization;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.DTO
{
    public class ResultDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResultType Type { get; set; }
        public int? Position { get; set; }
        public int Point { get; set; }
        public Guid DriverId { get; set; }
        public Guid TeamId { get; set; }
    }
}
