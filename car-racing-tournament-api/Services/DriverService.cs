using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Services
{
    public class DriverService : IDriver
    {
        private readonly FormulaDbContext _formulaDbContext;

        private const string DRIVER_NOT_FOUND = "Driver not found";

        public DriverService(FormulaDbContext formulaDbContext)
        {
            _formulaDbContext = formulaDbContext;
        }

        public async Task<(bool IsSuccess, Driver? Driver, string? ErrorMessage)> GetDriverById(Guid id)
        {
            var driver = _formulaDbContext.Drivers.Where(e => e.Id == id).FirstOrDefault();
            if (driver == null)
                return (false, null, DRIVER_NOT_FOUND);
            
            return (true, driver, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateDriver(Guid id, DriverDto driverDto)
        {
            var driverObj = _formulaDbContext.Drivers.Where(e => e.Id == id).FirstOrDefault();
            if (driverObj == null)
                return (false, DRIVER_NOT_FOUND);

            driverObj.Name = driverDto.Name;
            driverObj.RealName = driverDto.RealName;
            driverObj.Number = driverDto.Number;
            driverObj.ActualTeam = driverObj.ActualTeam;
            _formulaDbContext.Drivers.Update(driverObj);
            _formulaDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateDriverTeam(Guid id, Guid teamId)
        {
            var driverObj = _formulaDbContext.Drivers.Where(e => e.Id == id).FirstOrDefault();
            if (driverObj == null)
                return (false, DRIVER_NOT_FOUND);
            
            driverObj.ActualTeamId = teamId;
            _formulaDbContext.Drivers.Update(driverObj);
            _formulaDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteDriver(Guid id)
        {
            var driver = _formulaDbContext.Drivers.Where(e => e.Id == id).First();
            if (driver == null)
                return (false, DRIVER_NOT_FOUND);
            
            _formulaDbContext.Drivers.Remove(driver);
            _formulaDbContext.SaveChanges();

            return (true, null);
        }
    }
}
