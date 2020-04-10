using System;
using System.ComponentModel;
using DataLayer.Models;

namespace DataLayer.ViewModel
{
    public class CovidCountryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string CountryName { get; set; }
        public string Continent { get; set; }
        public int TotalCases { get; set; }
        public int TotalDeaths { get; set; }
        public int NewCasesToday { get; set; }
        public int NewDeathsToday { get; set; }
        public int NewCasesYesterday { get; set; }
        public int NewDeathsYesterday { get; set; }

        private bool _displayNewCasesToday;
        public bool DisplayNewCasesToday
        {
            get => _displayNewCasesToday;
            set
            {
                if (_displayNewCasesToday != value)
                {
                    _displayNewCasesToday = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayNewCasesToday)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewCases)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewDeaths)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNewCasesMoreThanThousand)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNewDeathsMoreThanThousand)));
                }
            }
        }

        public int NewCases
        {
            get
            {
                if (_displayNewCasesToday)
                    return NewCasesToday;
                else
                    return NewCasesYesterday;
            }
        }

        public int NewDeaths
        {
            get
            {
                if (_displayNewCasesToday)
                    return NewDeathsToday;
                else
                    return NewDeathsYesterday;
            }
        }
   
        public static CovidCountryViewModel Create(CountryData countryData)
        {
            if (countryData == null)
                return null;
            return new CovidCountryViewModel
            {
                CountryName = countryData.CountryName,
                Continent = string.IsNullOrWhiteSpace(countryData.Continent) ? "Others" : countryData.Continent,
                TotalCases = countryData.TotalCases,
                TotalDeaths = countryData.TotalDeaths,
                NewCasesToday = countryData.NewCasesToday.NewCases,
                NewDeathsToday = countryData.NewCasesToday.NewDeaths,
                NewCasesYesterday = countryData.NewCasesYesterday.NewCases,
                NewDeathsYesterday = countryData.NewCasesYesterday.NewDeaths,
                DisplayNewCasesToday = true
            };
        }

        public bool IsNewCasesMoreThanThousand { get { return NewCases > 1000; } }

        public bool IsNewDeathsMoreThanThousand { get { return NewDeaths > 1000; } }
    }
}
