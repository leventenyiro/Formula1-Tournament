namespace formula1_tournament_api.Interfaces
{
    public interface ISeason
    {
        Task<(bool IsSuccess, List<SeasonDTO> Season, string ErrorMessage)> GetAllSeasons();
        Task<(bool IsSuccess, SeasonDTO Season, string ErrorMessage)> GetSeasonById(Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> AddSeason(SeasonDTO season);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateSeason(Guid id, SeasonDTO season);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteSeason(Guid id);
    }
}
