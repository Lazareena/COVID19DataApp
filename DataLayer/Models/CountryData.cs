using System;
namespace DataLayer.Models
{
    public class CountryData
    {
        public string CountryName { get; set; }
        public int TotalCases { get; set; }
        public int TotalRecovered { get; set; }
        public int TotalDeaths { get; set; }
        public int ActiveCases { get; set; }
        public int SeriousCriticalCases { get; set; }
        public int TotalCasesPerMillion { get; set; }
        public int TotalDeathsPerMillion { get; set; }
        public int TotalTest { get; set; }
        public int TotalTestPerMillion { get; set; }
        public string Continent { get; set; }

        public NewCasesInfo NewCasesToday { get; private set; } = new NewCasesInfo();
        public NewCasesInfo NewCasesYesterday { get; private set; } = new NewCasesInfo();
    }
}
