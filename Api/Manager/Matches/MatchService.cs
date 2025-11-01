using Api.Models;
using DTO;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Geocoding.Address.Request;
using GoogleApi.Entities.PlacesNew.Details.Request;
using System.Text;
using System.Text.Json;

namespace Api.Manager
{
    public class MatchManager : IMatchManager
    {
        private HttpClient _httpClient;
        private string _mapsKey;
        private Dictionary<int, Task<(double, double)>> _locationQueries = [];

        public MatchManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _mapsKey = Environment.GetEnvironmentVariable("MATCHTRACKER_MAPS_KEY");
        }

        public async Task<List<MatchDTO>> SearchMatches(string placeId, int radius, DateTime dateFrom, DateTime dateTo)
        {
            var postalCode = await GetPostalCode(placeId);
            var payload = new
            {
                akGruppeIdList = new List<int> { },
                fromDate = dateFrom.ToString("yyyy-MM-dd"),
                gIdList = new List<int> { },
                spielfeldPlz = postalCode,
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
                Key = _mapsKey                
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

        public async Task<string> GetPostalCode(string placeId)
        {
            var request = new PlacesNewDetailsRequest
            {
                Language = Language.German,
                Key = _mapsKey,
                PlaceId = placeId,
                FieldMask = "addressComponents"
            };
            var response = await GoogleApi.GooglePlacesNew.Details.QueryAsync(request);
            var postalCode = response.Place.AddressComponents
                .First(component => component.Types.Any(type => type == AddressComponentType.Postal_Code))
                .LongText;
            return postalCode;
        }
    }
}
