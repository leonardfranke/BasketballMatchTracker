using DTO;
using LeafletForBlazor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Website.Services.Matches;

namespace Website.Pages
{
    public class HomeBase : ComponentBase
    {
        [Inject]
        public IMatchService MatchService { get; set; }
        public RealTimeMap Map { get; set; }
        public List<MatchDTO> Matches { get; set; }

        public bool IsLoading { get; set; }
        public int PostalCode { get; set; } = 49196;
        public int Radius { get; set; } = 30;
        public DateRange DateRange { get; set; }

        protected override void OnInitialized()
        {
            var differenceToFriday = DayOfWeek.Saturday - DateTime.Today.DayOfWeek;
            var dateFrom = DateTime.Today.AddDays(differenceToFriday);
            var dateTo = dateFrom.AddDays(8);
            DateRange = new DateRange(dateFrom, dateTo);
        }

        public async Task SearchMatches()
        {
            IsLoading = true;
            var matches = await MatchService.SearchMatches(PostalCode, Radius, DateRange.Start.Value, DateRange.End.Value);
            //foreach(var match in matches)
            //{
            //    await Map.Geometric.Points.([match.Latitude, match.Longitude], new RealTimeMap.PointTooltip
            //    {
            //        content = "Test"
            //    });
            //}
            var points = matches.Select(match =>
            {
                return new RealTimeMap.StreamPoint
                {
                    value = match,
                    latitude = match.Latitude,
                    longitude = match.Longitude,
                };
            }).ToList();
            await Map.Geometric.Points.upload(points, true);
            Map.Geometric.Points.Appearance().pattern = new RealTimeMap.PointTooltip
            {
                content = "Test",
                permanent = true
            };
            Matches = matches;
            IsLoading = false;
        }
    }
}
