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

        public async Task<NetworkStatus<List<CountryData>>> LoadCountryCovidCases()
        {
            _covidCountryData.Clear();
            var result = new NetworkStatus<List<CountryData>>();

            var pageSourceResult = await _httpClientMgr.GetAsync(_covidCasesNowUrl);
            if(!pageSourceResult.Succeeded)
            {
                result.ErrorMessage = "Error loading content from " + _covidCasesNowUrl;
                result.ErrorCode = pageSourceResult.ErrorCode;
                return result;
            }

            var parseSuccess = TryParseWorldometersSiteData(pageSourceResult.Result);
            if(!parseSuccess)
            {
                result.ErrorMessage = "Error parsing content from " + _covidCasesNowUrl;
            }
            else
            {
                result.Result = _covidCountryData;
                result.Succeeded = true;
            }
            return result;
        }

        private bool TryParseWorldometersSiteData(string pageSource)
        {
            try
            {
                var pageHtmlDoc = new HtmlDocument();
                pageHtmlDoc.LoadHtml(pageSource);

                var covidTableByCountry = pageHtmlDoc.GetElementbyId(_covidCountriesTableId);

                var tableNodes = covidTableByCountry.ChildNodes;
                var tableBodyNode = tableNodes.FirstOrDefault(n => n.Name == _tableBodyName);

                foreach (var row in tableBodyNode.ChildNodes)
                {
                    if (row.Name == _tableRowName)
                    {
                        var newCountryData = new CountryData();

                        int indexColumn = 0;
                        foreach (var col in row.ChildNodes)
                        {
                            if (col.Name == _tableDataName)
                            {
                                int intValue = 0;
                                DateTime dateTimeValue = DateTime.MinValue;
                                string stringValue = "";

                                if (indexColumn == 0)
                                {
                                    stringValue = col.InnerText.Trim();
                                }
                                else if (indexColumn == 10)
                                {
                                    if (!string.IsNullOrWhiteSpace(col.InnerText))
                                        DateTime.TryParse(col.InnerText.Trim(), null, DateTimeStyles.AllowWhiteSpaces, out dateTimeValue);
                                }
                                else if (!string.IsNullOrWhiteSpace(col.InnerText))
                                {
                                    Int32.TryParse(col.InnerText.TrimStart('+').Trim(), NumberStyles.AllowThousands, null, out intValue);
                                }

                                switch (indexColumn)
                                {
                                    case 0:
                                        newCountryData.CountryName = stringValue;
                                        break;
                                    case 1:
                                        newCountryData.TotalCases = intValue;
                                        break;
                                    case 2:
                                        newCountryData.NewCases = intValue;
                                        break;
                                    case 3:
                                        newCountryData.TotalDeaths = intValue;
                                        break;
                                    case 4:
                                        newCountryData.NewDeaths = intValue;
                                        break;
                                    case 5:
                                        newCountryData.TotalRecovered = intValue;
                                        break;
                                    case 6:
                                        newCountryData.ActiveCases = intValue;
                                        break;
                                    case 7:
                                        newCountryData.SeriousCriticalCases = intValue;
                                        break;
                                    case 8:
                                        newCountryData.TotalCasesPerMillion = intValue;
                                        break;
                                    case 9:
                                        newCountryData.TotalDeathsPerMillion = intValue;
                                        break;
                                    case 10:
                                        newCountryData.ReportedFirstCase = dateTimeValue;
                                        break;
                                }
                                indexColumn++;
                            }
                        }
                        _covidCountryData.Add(newCountryData);
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
