using formula1_tournament_api.Data;
using formula1_tournament_api.DTO;
using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Services
{
    public class DriverService : IDriver
    {
        private readonly FormulaDbContext _formulaDbContext;

        public DriverService(FormulaDbContext formulaDbContext)
        {
            _formulaDbContext = formulaDbContext;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddDriver(DriverDto driverDto)
        {
            if (driverDto != null)
            {
                _formulaDbContext.Add(new Driver
                {
                    Id = Guid.NewGuid(),
                    Name = driverDto.Name,
                    RealName = driverDto.RealName,
                    Number = driverDto.Number,
                    ActualTeamId = driverDto.ActualTeamId
                });
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
                //driverObj.TeamId = driver.TeamId; just when point is null, every race
                _formulaDbContext.Drivers.Update(driverObj);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Driver not found");
        }
    }
}
