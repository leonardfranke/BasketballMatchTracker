using Api.Manager;
using DTO;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : Controller
    {
        private IMatchManager _matchManager;

        public MatchController(IMatchManager matchManager) 
        {
            _matchManager = matchManager;
        }

        [HttpPost]
        public Task<List<MatchDTO>> SearchMatches([FromBody] SearchMatchesDTO searchMatchesDTO)
        {
            return _matchManager.SearchMatches(searchMatchesDTO.PostalCode, searchMatchesDTO.Radius, searchMatchesDTO.DateFrom, searchMatchesDTO.DateTo);
        }
    }
}
