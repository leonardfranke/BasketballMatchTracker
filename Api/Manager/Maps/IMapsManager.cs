using DTO;

namespace Api.Manager.Maps
{
    public interface IMapsManager
    {
        public Task<IEnumerable<PlacesAutocompleteDTO>> Search(string query);
    }
}
