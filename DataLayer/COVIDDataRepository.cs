using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.ViewModel;
using HtmlAgilityPack;

namespace DataLayer
{
    public class COVIDDataRepository
    {
        private HttpClientMgr _httpClientMgr;

        private const string _covidCasesNowUrl = "https://www.worldometers.info/coronavirus";

        private const string _covidCountriesTableId = "main_table_countries_today";
        private const string _covidCountriesTableYesterdayId = "main_table_countries_yesterday";
        private const string _tableRowContinentClassId = "total_row_world row_continent";

        private const string _worldClassName = "total_row_world";
        private const string _tableBodyName = "tbody";
        private const string _tableRowName = "tr";
        private const string _tableDataName = "td";

        private const int _indexColumnCountryName = 0;
        private const int _indexColumnTotalCases = 1;
        private const int _indexColumnNewCases = 2;
        private const int _indexColumnTotalDeaths = 3;
        private const int _indexColumnNewDeaths = 4;
        private const int _indexColumnTotalRecovered = 5;
        private const int _indexColumnActiveCases = 6;
        private const int _indexColumnCriticalCases = 7;
        private const int _indexColumnCasesPerMillion = 8;
        private const int _indexColumnDeathsPerMillion = 9;
        private const int _indexColumnTotalTest = 10;
        private const int _indexColumnTotalTestPerMillion = 11;
        private const int _indexColumnContinent= 12;

        private List<CountryData> _covidCountryData = new List<CountryData>();
        private CountryData _totalWorldCovidData;
            
        public COVIDDataRepository(HttpClientMgr httpClient)
        {
            _httpClientMgr = httpClient;
        }

        public async Task<NetworkStatus<List<CovidCountryViewModel>>> LoadCountryCovidCases()
        {
            _covidCountryData.Clear();
            var result = new NetworkStatus<List<CovidCountryViewModel>>();

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
                result.Result = new List<CovidCountryViewModel>();
                foreach (var c in _covidCountryData)
                    result.Result.Add(CovidCountryViewModel.Create(c));
                result.Succeeded = true;
            }
            return result;
        }

        public CovidCountryViewModel GetTotalWorldNoLoad()
        {
            return CovidCountryViewModel.Create(_totalWorldCovidData);
        }

        private bool TryParseWorldometersSiteData(string pageSource)
        {
            try
            {
                var pageHtmlDoc = new HtmlDocument();
                pageHtmlDoc.LoadHtml(pageSource);

                var covidTableByCountry = pageHtmlDoc.GetElementbyId(_covidCountriesTableId);
                var covidTableByCountryYesterday = pageHtmlDoc.GetElementbyId(_covidCountriesTableYesterdayId);

                var relevantTableRows = covidTableByCountry.ChildNodes.FirstOrDefault(n => n.Name == _tableBodyName).ChildNodes
                                                                       .Where(c => c.Name == _tableRowName && !c.Attributes.Any(a => a.Name == "class" && a.Value == _tableRowContinentClassId));

                var relevantTableRowsYesterday = covidTableByCountryYesterday.ChildNodes.FirstOrDefault(n => n.Name == _tableBodyName).ChildNodes.
                                                                                         Where(c => c.Name == _tableRowName && !c.Attributes.Any(a => a.Name == "class" && a.Value == _tableRowContinentClassId));

                foreach (var row in relevantTableRows)
                {
                    var isTotalWorldRow = row.Attributes.Any(a => a.Name == "class" && a.Value == _worldClassName);
                    var newCountryData = new CountryData();

                    var relevantColumns = row.ChildNodes.Where(c => c.Name == _tableDataName).ToList();

                    newCountryData.CountryName = relevantColumns[_indexColumnCountryName].InnerText.Trim();
                    newCountryData.TotalCases = ParseIntValue(relevantColumns[_indexColumnTotalCases].InnerText);
                    newCountryData.NewCasesToday.NewCases = ParseIntValue(relevantColumns[_indexColumnNewCases].InnerText);
                    newCountryData.TotalDeaths = ParseIntValue(relevantColumns[_indexColumnTotalDeaths].InnerText);
                    newCountryData.NewCasesToday.NewDeaths = ParseIntValue(relevantColumns[_indexColumnNewDeaths].InnerText);
                    newCountryData.TotalRecovered = ParseIntValue(relevantColumns[_indexColumnTotalRecovered].InnerText);
                    newCountryData.ActiveCases = ParseIntValue(relevantColumns[_indexColumnActiveCases].InnerText);
                    newCountryData.SeriousCriticalCases = ParseIntValue(relevantColumns[_indexColumnCriticalCases].InnerText);
                    newCountryData.TotalCasesPerMillion = ParseIntValue(relevantColumns[_indexColumnCasesPerMillion].InnerText);
                    newCountryData.TotalDeathsPerMillion = ParseIntValue(relevantColumns[_indexColumnDeathsPerMillion].InnerText);
                    newCountryData.TotalTest = ParseIntValue(relevantColumns[_indexColumnTotalTest].InnerText);
                    newCountryData.TotalTestPerMillion = ParseIntValue(relevantColumns[_indexColumnTotalTestPerMillion].InnerText);
                    newCountryData.Continent = relevantColumns[_indexColumnContinent].InnerText.Trim();

                    var yesterdaysData = relevantTableRowsYesterday.FirstOrDefault(r => r.ChildNodes.Any(c => c.Name == _tableDataName && c.InnerText == newCountryData.CountryName));
                    if(yesterdaysData != null)
                    {
                        var relevantColumnsYesterday = yesterdaysData.ChildNodes.Where(c => c.Name == _tableDataName).ToList();
                        newCountryData.NewCasesYesterday.NewCases = ParseIntValue(relevantColumnsYesterday[_indexColumnNewCases].InnerText);
                        newCountryData.NewCasesYesterday.NewDeaths = ParseIntValue(relevantColumnsYesterday[_indexColumnNewDeaths].InnerText);
                    }

                    if (!isTotalWorldRow)
                        _covidCountryData.Add(newCountryData);
                    else
                        _totalWorldCovidData = newCountryData;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private int ParseIntValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;
            Int32.TryParse(value.TrimStart('+').Trim(), NumberStyles.AllowThousands, null, out var intValue);
            return intValue;
        }
    }
}
