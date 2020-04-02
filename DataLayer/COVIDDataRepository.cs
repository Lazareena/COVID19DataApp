using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models;
using HtmlAgilityPack;

namespace DataLayer
{
    public class COVIDDataRepository
    {
        private HttpClientMgr _httpClientMgr;

        private const string _covidCasesNowUrl = "https://www.worldometers.info/coronavirus";
        private const string _covidCountriesTableId = "main_table_countries_today";
        private const string _tableBodyName = "tbody";
        private const string _tableRowName = "tr";
        private const string _tableDataName = "td";

        private List<CountryData> _covidCountryData = new List<CountryData>();
            
        public COVIDDataRepository(HttpClientMgr httpClient)
        {
            _httpClientMgr = httpClient;
        }

        public async Task<string> LoadCovidCases()
        {
            var result = await _httpClientMgr.GetAsync(_covidCasesNowUrl);
            ParseWorldometersSiteData(result.Result);
            return result.Result;
        }

        private void ParseWorldometersSiteData(string pageSource)
        {
            var pageHtmlDoc = new HtmlDocument();
            pageHtmlDoc.LoadHtml(pageSource);

            var covidTableByCountry = pageHtmlDoc.GetElementbyId(_covidCountriesTableId);

            var tableNodes = covidTableByCountry.ChildNodes;
            var tableBodyNode = tableNodes.FirstOrDefault(n => n.Name == _tableBodyName);

            foreach(var row in tableBodyNode.ChildNodes)
            {
                if(row.Name == _tableRowName)
                {
                    var newCountryData = new CountryData();

                    int indexColumn = 0;
                    foreach(var col in row.ChildNodes)
                    {
                        if(col.Name == _tableDataName)
                        {
                            if (!Int32.TryParse(col.InnerText.TrimStart('+').Trim(), NumberStyles.AllowThousands, null, out var res))
                            {
                                ;
                            }
                            switch (indexColumn)
                            {
                                
                                case 0:
                                    newCountryData.CountryName = col.InnerText;
                                    break;
                                case 1:
                                    newCountryData.TotalCases = Int32.Parse(col.InnerText.Trim(), NumberStyles.AllowThousands);
                                    break;
                                case 2:
                                    newCountryData.NewCases = Int32.Parse(col.InnerText.TrimStart('+').Trim(), NumberStyles.AllowThousands);
                                    break;
                                case 3:
                                    newCountryData.TotalDeaths = Int32.Parse(col.InnerText.Trim(), NumberStyles.AllowThousands);
                                    break;
                                case 4:
                                    newCountryData.NewDeaths = Int32.Parse(col.InnerText.TrimStart('+').Trim(), NumberStyles.AllowThousands);
                                    break;
                                case 5:
                                    newCountryData.TotalRecovered = Int32.Parse(col.InnerText.Trim(), NumberStyles.AllowThousands);
                                    break;
                                case 6:
                                    newCountryData.ActiveCases = Int32.Parse(col.InnerText.Trim(), NumberStyles.AllowThousands);
                                    break;
                                case 7:
                                    newCountryData.SeriousCriticalCases = Int32.Parse(col.InnerText.Trim(), NumberStyles.AllowThousands);
                                    break;
                                case 8:
                                    newCountryData.TotalCasesPerMillion = Int32.Parse(col.InnerText.Trim(), NumberStyles.AllowThousands);
                                    break;
                                case 9:
                                    newCountryData.TotalDeathsPerMillion = Int32.Parse(col.InnerText.Trim(), NumberStyles.AllowThousands);
                                    break;
                                case 10:
                                    newCountryData.ReportedFirstCase = DateTime.Parse(col.InnerText.Trim(), null, DateTimeStyles.AllowWhiteSpaces);
                                    break;
                            }
                            indexColumn++;
                        }
                    }
                    _covidCountryData.Add(newCountryData);
                }
            }
        }
    }
}
