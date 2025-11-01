using Api.Manager.Maps;
using DTO;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapsController : ControllerBase
    {
        private IMapsManager _mapsManager;

        public MapsController(IMapsManager mapsManager) 
        {
            _mapsManager = mapsManager;
        }

        [HttpGet]
        public Task<IEnumerable<PlacesAutocompleteDTO>> SearchPlaces([FromQuery] string query)
        {
            return _mapsManager.Search(query);
        }
    }
}
