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

        private const string RACE_NOT_FOUND = "Race not found";

        public RaceService(CarRacingTournamentDbContext carRacingTournamentDbContext, IMapper mapper)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _mapper = mapper;
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
                    Results = x.Results!.Select(x => new Result
                    {
                        Id = x.Id,
                        Position = x.Position,
                        Points = x.Points,
                        Driver = new Driver
                        {
                            Id = x.Driver.Id,
                            Name = x.Driver.Name,
                            RealName = x.Driver.RealName,

                        },
                        Team = new Team
                        {
                            Id = x.Team.Id,
                            Name = x.Team.Name,
                            Color = x.Team.Color
                        }
                    }).ToList()
                }).FirstOrDefaultAsync();
            if (race == null)
                return (false, null, RACE_NOT_FOUND);

            return (true, race, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateRace(Guid id, RaceDto raceDto)
        {
            var raceObj = await _carRacingTournamentDbContext.Races.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (raceObj == null)
                return (false, RACE_NOT_FOUND);

            raceObj.Name = raceDto.Name;
            raceObj.DateTime = raceDto.DateTime;
            _carRacingTournamentDbContext.Races.Update(raceObj);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteRace(Guid id)
        {
            var race = await _carRacingTournamentDbContext.Races.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (race == null)
                return (false, RACE_NOT_FOUND);

            _carRacingTournamentDbContext.Races.Remove(race);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<Result>? Results, string? ErrorMessage)> GetResultsByRaceId(Guid raceId)
        {
            var results = await _carRacingTournamentDbContext.Results
                .Where(x => x.RaceId == raceId)
                .Include(x => x.Driver)
                .Include(x => x.Team)
                .Select(x => new Result {
                    Id = x.Id,
                    Position = x.Position,
                    Points = x.Points,
                    Driver = new Driver
                    {
                        Id = x.Driver.Id,
                        Name = x.Driver.Name,
                        RealName = x.Driver.RealName,

                    },
                    Team = new Team
                    {
                        Id = x.Team.Id,
                        Name = x.Team.Name,
                        Color = x.Team.Color
                    }
                }).ToListAsync();
            if (results == null)
                return (false, null, "No results found");

            return (true, results, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddResult(Guid raceId, ResultDto resultDto)
        {
            if (resultDto == null)
                return (false, "Please provide the result data");

            Race? raceObj = await _carRacingTournamentDbContext.Races.Where(e => e.Id == raceId).FirstOrDefaultAsync();
            Team? teamObj = await _carRacingTournamentDbContext.Teams.Where(e => e.Id == resultDto.TeamId).FirstOrDefaultAsync();
            if (raceObj?.SeasonId != teamObj?.SeasonId)
                return (false, "Race and team aren't in the same season");

            Driver? driverObj = await _carRacingTournamentDbContext.Drivers.Where(e => e.Id == resultDto.DriverId).FirstOrDefaultAsync();
            if (raceObj?.SeasonId == driverObj?.SeasonId)
                return (false, "Race and driver aren't in the same season");

            var result = _mapper.Map<Result>(resultDto);
            result.Id = Guid.NewGuid();
            result.RaceId = raceId;
            await _carRacingTournamentDbContext.AddAsync(result);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }
    }
}
