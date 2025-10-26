namespace Api.Models
{
    public class MatchSearchResponse
    {
        public MatchData Data { get; set; }
    }

    public class MatchData
    {
        public List<Match> Matches { get; set; }
    }

    public class Match
    {
        public LigaData LigaData { get; set; }
        public string KickoffDate { get; set; }
        public string KickoffTime { get; set; }
        public Team HomeTeam { get; set; }
        public Team GuestTeam { get; set; }
        public MatchInfo MatchInfo { get; set; }
    }

    public class MatchInfo
    {
        public Spielfeld Spielfeld { get; set; }
    }

    public class Spielfeld
    {
        public string Strasse { get; set; }
        public string Plz { get; set; }
        public string Ort { get; set; }
    }

    public class Team
    {
        public string TeamName { get; set; }
    }

    public class LigaData
    {
        public string Liganame { get; set; }
    }

}
