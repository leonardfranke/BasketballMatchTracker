using Api.Models;
using DTO;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Geocoding.Address.Request;
using System.Text;
using System.Text.Json;

namespace Api.Manager
{
    public class MatchManager : IMatchManager
    {
        private HttpClient _httpClient;
        private string _geocodingKey;
        private Dictionary<int, Task<(double, double)>> _locationQueries = [];

        public MatchManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _geocodingKey = Environment.GetEnvironmentVariable("MATCHTRACKER_MAPSKEY");
        }

        public async Task<List<MatchDTO>> SearchMatches(int postalCode, int radius, DateTime dateFrom, DateTime dateTo)
        {
            var payload = new
            {
                akGruppeIdList = new List<int> { },
                fromDate = dateFrom.ToString("yyyy-MM-dd"),
                gIdList = new List<int> { },
                spielfeldPlz = postalCode.ToString(),
                spielfeldUmkreis = radius*1000,
                startAtIndex = 0,
                toDate = dateTo.AddDays(1).ToString("yyyy-MM-dd")
            };
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await _httpClient.PostAsync(new Uri("match/search", UriKind.Relative), content);

            if (httpResponse.IsSuccessStatusCode)
            {
                var matchResponse = await httpResponse.Content.ReadFromJsonAsync<MatchSearchResponse>();                
                var matches = await Task.WhenAll(matchResponse.Data.Matches.Select(async match =>
                {
                    var locationQuery = $"{match.MatchInfo.Spielfeld.Strasse} {match.MatchInfo.Spielfeld.Ort} {match.MatchInfo.Spielfeld.Plz}";
                    var (latitude, longitude) = await SearchPlace(locationQuery);
                    var matchDTO = new MatchDTO
                    {
                        Liganame = match.LigaData.Liganame,
                        Heimmannschaft = match.HomeTeam.TeamName,
                        Gastmannschaft = match.GuestTeam.TeamName,
                        Tipoff = DateTime.Parse($"{match.KickoffDate} {match.KickoffTime}"),
                        Latitude = latitude,
                        Longitude = longitude,
                        Adresse = locationQuery
                    };
                    return matchDTO;
                }));
                return matches.ToList();
            }

            return new();
        }

        public Task<(double, double)> SearchPlace(string query)
        {
            var queryKey = query.GetHashCode();
            if(_locationQueries.ContainsKey(queryKey))
            {
                return _locationQueries[queryKey];
            }

            var request = new AddressGeocodeRequest
            {
                Address = query,
                Region = "DE",                
                Language = Language.German,
                Key = _geocodingKey                
            };

            var queryTask = GoogleApi.GoogleMaps.Geocode.AddressGeocode.QueryAsync(request).ContinueWith(async task =>
            {
                var response = await task;
                var location = response.Results.First().Geometry.Location;
                return (location.Latitude, location.Longitude);
            }).Unwrap();
            _locationQueries.Add(queryKey, queryTask);
            return queryTask;
        }
    }
}
