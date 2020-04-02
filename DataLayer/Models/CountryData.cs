using System;
namespace DataLayer.Models
{
    public class CountryData
    {
        public string CountryName { get; set; }
        public int TotalCases { get; set; }
        public int NewCases { get; set; }
        public int TotalDeaths { get; set; }
        public int NewDeaths { get; set; }
        public int TotalRecovered { get; set; }
        public int ActiveCases { get; set; }
        public int SeriousCriticalCases { get; set; }
        public int TotalCasesPerMillion { get; set; }
        public int TotalDeathsPerMillion { get; set; }
        public DateTime ReportedFirstCase { get; set; }
    }
}
