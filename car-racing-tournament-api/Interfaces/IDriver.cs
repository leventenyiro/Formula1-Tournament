using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface IDriver
    {
        Task<(bool IsSuccess, List<Driver> Drivers, string ErrorMessage)> GetAllDriversBySeasonId(Guid seasonId);
        Task<(bool IsSuccess, Driver Driver, string ErrorMessage)> GetDriverById(Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> AddDriver(DriverDto driverDto);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateDriver(Guid id, DriverDto driverDto);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateDriverTeam(Guid id, Guid teamId);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteDriver(Guid id);
    }
}
