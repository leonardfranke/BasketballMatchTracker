using DTO;

namespace Api.Manager
{
    public interface IMatchManager
    {
        public Task<List<MatchDTO>> SearchMatches(string placeId, int radius, DateTime dateFrom, DateTime dateTo);
    }
}
