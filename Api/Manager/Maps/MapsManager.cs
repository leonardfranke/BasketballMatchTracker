using DTO;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.PlacesNew.AutoComplete.Request;

namespace Api.Manager.Maps
{
    public class MapsManager : IMapsManager
    {
        private string _placesKey;

        public MapsManager() 
        {
            _placesKey = Environment.GetEnvironmentVariable("MATCHTRACKER_MAPS_KEY");
        }

        public async Task<IEnumerable<PlacesAutocompleteDTO>> Search(string query)
        {
            var request = new PlacesNewAutoCompleteRequest
            {
                Input = query,
                Language = Language.German,
                Key = _placesKey,
                FieldMask = "suggestions.placePrediction.text.text,suggestions.placePrediction.placeId"
            };
            var response = await GoogleApi.GooglePlacesNew.AutoComplete.QueryAsync(request);
            var data = response.Suggestions.Select(result =>
            {
                var placePrediction = result.PlacePrediction;
                return new PlacesAutocompleteDTO
                {
                    PlaceId = placePrediction.PlaceId,
                    Text = placePrediction.Text.Text
                };
            });
            return data;
        }
    }
}
