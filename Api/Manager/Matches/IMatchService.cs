using DTO;

namespace Api.Manager
{
    public interface IMatchManager
    {
        public Task<List<MatchDTO>> SearchMatches(int postalCode, int radius, DateTime dateFrom, DateTime dateTo);
    }
}
