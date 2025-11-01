using DTO;
using Microsoft.AspNetCore.Components;
using Website.Services.Maps;

namespace Web.Views
{
    public class LocationAutocompleteBase : ComponentBase
    {
        [Inject]
        private IMapsService _mapsService { get; set; }
        
        [Parameter]
        public Action<PlacesAutocompleteDTO> LocationChangedFunc { get; set; }

        public async Task<IEnumerable<object>> Search(string searchText, CancellationToken cancellationToken) 
        {
            if (string.IsNullOrEmpty(searchText))
                return [];

            var locations = await _mapsService.SearchLocation(searchText);
            return locations.Cast<object>();
        }

        public string ItemToString(object item)
        {
            if (item is PlacesAutocompleteDTO place)
                return place.Text;
            else
                return item.ToString();
        }

        public async Task ValueChanged(object value)
        {
            LocationChangedFunc.Invoke(value as PlacesAutocompleteDTO);
        }
    }
}
