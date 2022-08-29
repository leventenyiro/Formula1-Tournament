using formula1_tournament_api.Data;
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

        public async Task<(bool IsSuccess, string ErrorMessage)> AddDriver(Driver driver, Guid adminId)
        {
            // is the adminId the real admin? driver.Season.UserId
            bool isAdmin = _formulaDbContext.Season.Where(x => x.Id == driver.SeasonId).FirstOrDefault().UserId.Equals(adminId);
            if (driver != null || isAdmin)
            {
                driver.Id = Guid.NewGuid();
                _formulaDbContext.Add(driver);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Please provide the driver data");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteDriver(Guid id)
        {
            var driver = _formulaDbContext.Driver.Where(e => e.Id == id).FirstOrDefault();
            if (driver != null)
            {
                _formulaDbContext.Driver.Remove(driver);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Driver not found");
        }

        public async Task<(bool IsSuccess, List<Driver> Driver, string ErrorMessage)> GetAllDrivers()
        {
            var drivers = _formulaDbContext.Driver.ToList();
            if (drivers != null)
            {
                return (true, drivers, null);
            }
            return (false, null, "No drivers found");
        }

        public async Task<(bool IsSuccess, Driver Driver, string ErrorMessage)> GetDriverById(Guid id)
        {
            var driver = _formulaDbContext.Driver.Where(e => e.Id == id).FirstOrDefault();
            if (driver != null)
            {
                return (true, driver, null);
            }
            return (false, null, "Driver not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateDriver(Guid id, Driver driver)
        {
            var driverObj = _formulaDbContext.Driver.Where(e => e.Id == id).FirstOrDefault();
            if (driverObj != null)
            {
                driverObj.Name = driver.Name;
                //driverObj.TeamId = driver.TeamId; just when point is null, every race
                _formulaDbContext.Driver.Update(driverObj);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Driver not found");
        }
    }
}
