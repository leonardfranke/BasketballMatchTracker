namespace DTO
{
    public class SearchMatchesDTO
    {
        public int PostalCode { get; set; }
        public int Radius { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
