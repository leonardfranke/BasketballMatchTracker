using DTO;
namespace Website.Services.Matches
{
    public interface IMatchService
    {
        public Task<List<MatchDTO>> SearchMatches(int postalCode, int radius, DateTime dateFrom, DateTime dateTo);
    }
}
