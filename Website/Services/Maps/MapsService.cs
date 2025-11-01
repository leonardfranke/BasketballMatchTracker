using DTO;
using System.Net.Http.Json;

namespace Website.Services.Maps
{
    public class MapsService : IMapsService
    {
        public HttpClient _httpClient;

        public MapsService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BACKEND");
        }

        public async Task<List<PlacesAutocompleteDTO>> SearchLocation(string searchText)
        {
            var response = await _httpClient.GetAsync(new Uri($"/api/maps?query={searchText}", UriKind.Relative));
            return await response.Content.ReadFromJsonAsync<List<PlacesAutocompleteDTO>>();
        }
    }
}
