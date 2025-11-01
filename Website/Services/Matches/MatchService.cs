using DTO;
using System.Net.Http.Json;

namespace Website.Services.Matches
{
    public class MatchService : IMatchService
    {
        public HttpClient _httpClient;

        public MatchService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BACKEND");
        }

        public async Task<List<MatchDTO>> SearchMatches(string placeId, int radius, DateTime dateFrom, DateTime dateTo)
        {
            var query = new SearchMatchesDTO
            {
                PlaceId = placeId,
                Radius = radius,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
            var response = await _httpClient.PostAsJsonAsync(new Uri("api/match", UriKind.Relative), query);
            return await response.Content.ReadFromJsonAsync<List<MatchDTO>>();
        }
    }
}
