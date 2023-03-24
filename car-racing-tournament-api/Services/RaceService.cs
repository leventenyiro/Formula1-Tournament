using AutoMapper;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class RaceService : IRace
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public RaceService(CarRacingTournamentDbContext carRacingTournamentDbContext, IMapper mapper, IConfiguration configuration)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, Race? Race, string? ErrorMessage)> GetRaceById(Guid id)
        {
            var race = await _carRacingTournamentDbContext.Races
                .Where(e => e.Id == id)
                .Include(x => x.Results!).ThenInclude(x => x.Driver)
                .Include(x => x.Results!).ThenInclude(x => x.Team)
                .Select(x => new Race
                {
                    Id = x.Id,
                    Name = x.Name,
                    DateTime = x.DateTime,
                    SeasonId = x.SeasonId,
                    Results = x.Results!.Select(x => new Result
                    {
                        Id = x.Id,
                        Position = x.Position,
                        Point = x.Point,
                        Driver = new Driver
                        {
                            Id = x.Driver.Id,
                            Name = x.Driver.Name,
                            RealName = x.Driver.RealName,
                            Number = x.Driver.Number
                        },
                        Team = new Team
                        {
                            Id = x.Team.Id,
                            Name = x.Team.Name,
                            Color = x.Team.Color
                        }
                    }).ToList(),
                }).FirstOrDefaultAsync();

            if (race == null)
                return (false, null, _configuration["ErrorMessages:RaceNotFound"]);

            return (true, race, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddRace(Season season, RaceDto raceDto)
        {
            raceDto.Name = raceDto.Name.Trim();
            if (string.IsNullOrEmpty(raceDto.Name))
                return (false, _configuration["ErrorMessages:RaceNameCannotBeEmpty"]);

            if (await _carRacingTournamentDbContext.Races.CountAsync(
                x => x.Name == raceDto.Name && x.SeasonId == season.Id) != 0)
                return (false, _configuration["ErrorMessages:RaceNameExists"]);

            raceDto.Name = raceDto.Name;
            var race = _mapper.Map<Race>(raceDto);
            race.Id = Guid.NewGuid();
            race.SeasonId = season.Id;
            await _carRacingTournamentDbContext.Races.AddAsync(race);
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateRace(Race race, RaceDto raceDto)
        {
            raceDto.Name = raceDto.Name.Trim();
            if (string.IsNullOrEmpty(raceDto.Name))
                return (false, _configuration["ErrorMessages:RaceNameCannotBeEmpty"]);

            if (race.Name != raceDto.Name && 
                await _carRacingTournamentDbContext.Races.CountAsync(
                    x => x.Name == raceDto.Name && x.SeasonId == race.SeasonId) != 0)
                return (false, _configuration["ErrorMessages:RaceNameExists"]);

            race.Name = raceDto.Name;
            race.DateTime = raceDto.DateTime;
            _carRacingTournamentDbContext.Entry(race).State = EntityState.Modified;
            await _carRacingTournamentDbContext.SaveChangesAsync();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteRace(Race race)
        {
            _carRacingTournamentDbContext.Races.Remove(race);
            await _carRacingTournamentDbContext.SaveChangesAsync();
            
            return (true, null);
        }
    }
}
