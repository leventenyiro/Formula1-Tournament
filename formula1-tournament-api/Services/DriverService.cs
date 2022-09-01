using formula1_tournament_api.Data;
using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Services
{
    public class DriverService : IDriver
    {
        private readonly FormulaDbContext _formulaDbContext;
        private readonly UserSeasonService _userSeasonService;

        public DriverService(FormulaDbContext formulaDbContext, UserSeasonService userSeasonService)
        {
            _formulaDbContext = formulaDbContext;
            _userSeasonService = userSeasonService;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddDriver(Driver driver, User user)
        {
            // is the adminId the real admin? driver.Season.UserId
            // csak akkor lehessen hozzáadni, ha ő admin, vagy moderátor
            //bool isAdmin = _formulaDbContext.Seasons.Where(x => x == driver.Season).First().UserId.Equals(adminId);
            //bool isAdmin = _formulaDbContext.Seasons.Where(x => x == driver.Season).First().Users.Where(x => x.Id == adminId).Any();
            UserSeasonPermission permission = _userSeasonService.GetPermission(user, driver.Season);
            bool hasPermission = permission == UserSeasonPermission.Admin || permission == UserSeasonPermission.Moderator;
            //bool isAdmin = true; // temporary
            /*if (driver != null && isAdmin)
            {
                driver.Id = Guid.NewGuid();
                _formulaDbContext.Add(driver);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Please provide the driver data");*/
            if (driver == null)
                return (false, "Please provide the driver data"); 
            else if (!hasPermission)
                return (false, "You don't have permission for it");
            else
            {
                driver.Id = Guid.NewGuid();
                _formulaDbContext.Add(driver);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteDriver(Guid id)
        {
            var driver = _formulaDbContext.Drivers.Where(e => e.Id == id).FirstOrDefault();
            if (driver != null)
            {
                _formulaDbContext.Drivers.Remove(driver);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Driver not found");
        }

        public async Task<(bool IsSuccess, List<Driver> Drivers, string ErrorMessage)> GetAllDrivers()
        {
            var drivers = _formulaDbContext.Drivers.ToList();
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

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateDriver(Guid id, Driver driver)
        {
            var driverObj = _formulaDbContext.Drivers.Where(e => e.Id == id).FirstOrDefault();
            if (driverObj != null)
            {
                driverObj.Name = driver.Name;
                //driverObj.TeamId = driver.TeamId; just when point is null, every race
                _formulaDbContext.Drivers.Update(driverObj);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Driver not found");
        }
    }
}
