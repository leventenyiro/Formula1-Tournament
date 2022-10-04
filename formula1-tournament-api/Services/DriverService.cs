using AutoMapper;
using formula1_tournament_api.Data;
using formula1_tournament_api.DTO;
using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Services
{
    public class DriverService : IDriver
    {
        private readonly FormulaDbContext _formulaDbContext;
        private IMapper _mapper;

        public DriverService(FormulaDbContext formulaDbContext, IMapper mapper)
        {
            _formulaDbContext = formulaDbContext;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddDriver(DriverDto driverDto)
        {
            if (driverDto != null)
            {
                var driver = _mapper.Map<Driver>(driverDto);
                driver.Id = Guid.NewGuid();
                _formulaDbContext.Add(driver);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Please provide the driver data");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteDriver(Guid id)
        {
            var driver = _formulaDbContext.Drivers.Where(e => e.Id == id).First();
            if (driver != null)
            {
                _formulaDbContext.Drivers.Remove(driver);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Driver not found");
        }

        public async Task<(bool IsSuccess, List<Driver> Drivers, string ErrorMessage)> GetAllDriversBySeasonId(Guid seasonId)
        {
            var drivers = _formulaDbContext.Drivers.Where(x => x.SeasonId == seasonId).ToList();
            if (drivers != null)
            {
                return (true, drivers, null);
            }
            return (false, null, "No drivers found");
        }

        public async Task<(bool IsSuccess, Driver Driver, string ErrorMessage)> GetDriverById(Guid id)
        {
            var driver = _formulaDbContext.Drivers.Where(e => e.Id == id).FirstOrDefault();
            if (driver != null)
            {
                return (true, driver, null);
            }
            return (false, null, "Driver not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateDriver(Guid id, DriverDto driverDto)
        {
            var driverObj = _formulaDbContext.Drivers.Where(e => e.Id == id).FirstOrDefault();
            if (driverObj != null)
            {
                driverObj.Name = driverDto.Name;
                driverObj.RealName = driverDto.RealName;
                driverObj.Number = driverDto.Number;
                //driverObj.ActualTeamId = driverDto.ActualTeamId; just when point is null, every race
                _formulaDbContext.Drivers.Update(driverObj);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Driver not found");
        }
    }
}
