using formula1_tournament_api.Models;

namespace formula1_tournament_api.Interfaces
{
    public interface IDriver
    {
        Task<(bool IsSuccess, List<Driver> Driver, string ErrorMessage)> GetAllDrivers();
        Task<(bool IsSuccess, Driver Driver, string ErrorMessage)> GetDriverById(Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> AddDriver(Driver driver, Guid adminId);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateDriver(Guid id, Driver driver);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteDriver(Guid id);
    }
}
