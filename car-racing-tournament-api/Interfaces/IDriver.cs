using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface IDriver
    {
        Task<(bool IsSuccess, List<Driver>? Drivers, string? ErrorMessage)> GetDriversBySeason(Season season);
        Task<(bool IsSuccess, Driver? Driver, string? ErrorMessage)> GetDriverById(Guid id);
        Task<(bool IsSuccess, string? ErrorMessage)> AddDriver(Season season, DriverDto driverDto, Team team);
        Task<(bool IsSuccess, string? ErrorMessage)> UpdateDriver(Driver driver, DriverDto driverDto, Team team);
        Task<(bool IsSuccess, string? ErrorMessage)> UpdateDriverTeam(Driver driver, Team team);
        Task<(bool IsSuccess, string? ErrorMessage)> DeleteDriver(Driver driver);
    }
}
