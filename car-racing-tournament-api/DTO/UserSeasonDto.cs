using System;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.DTO
{
	public class UserSeasonDto
	{
		public Guid UserId { get; set; }
		public Guid SeasonId { get; set; }
		public UserSeasonPermission Permission { get; set; }
	}
}

