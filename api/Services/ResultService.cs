using AutoMapper;
using api.Data;
using api.DTO;
using api.Interfaces;
using api.Models;

namespace api.Services
{
    public class ResultService : Interfaces.IResult
    {
        private readonly FormulaDbContext _formulaDbContext;
        private IMapper _mapper;

        public ResultService(FormulaDbContext formulaDbContext, IMapper mapper)
        {
            _formulaDbContext = formulaDbContext;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddResult(ResultDto resultDto)
        {
            if (resultDto != null)
            {
                var result = _mapper.Map<Result>(resultDto);
                result.Id = Guid.NewGuid();
                _formulaDbContext.Add(result);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Please provide the result data");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteResult(Guid id)
        {
            var result = _formulaDbContext.Results.Where(e => e.Id == id).FirstOrDefault();
            if (result != null)
            {
                _formulaDbContext.Results.Remove(result);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Result not found");
        }

        public async Task<(bool IsSuccess, List<Result> Results, string ErrorMessage)> GetAllResultsByRaceId(Guid raceId)
        {
            var results = _formulaDbContext.Results.Where(x => x.RaceId == raceId).ToList();
            if (results != null)
            {
                return (true, results, null);
            }
            return (false, null, "No results found");
        }

        public async Task<(bool IsSuccess, Result Result, string ErrorMessage)> GetResultById(Guid id)
        {
            var result = _formulaDbContext.Results.Where(e => e.Id == id).FirstOrDefault();
            if (result != null)
            {
                return (true, result, null);
            }
            return (false, null, "Result not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateResult(Guid id, ResultDto resultDto)
        {
            var resultObj = _formulaDbContext.Results.Where(e => e.Id == id).FirstOrDefault();
            if (resultObj != null)
            {
                resultObj.Position = resultDto.Position;
                resultObj.Points = resultDto.Points;
                resultObj.DriverId = resultDto.DriverId;
                resultObj.TeamId = resultDto.TeamId;
                _formulaDbContext.Results.Update(resultObj);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Result not found");
        }
    }
}
