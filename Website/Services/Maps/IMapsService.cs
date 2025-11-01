using DTO;

namespace Website.Services.Maps
{
    public interface IMapsService
    {
        public Task<List<PlacesAutocompleteDTO>> SearchLocation(string searchText);
    }
}
