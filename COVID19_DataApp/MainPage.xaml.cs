using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Models;
using Xamarin.Forms;

namespace COVID19_DataApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private COVIDDataRepository _dataRepository;
        private List<CountryData> _covidCountryData;

        public MainPage(COVIDDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
            InitializeComponent();
        }

        //TODO: Setup UI for displaying COVID 19 data by country
        public void InitializeUI()
        {
            var displayCovidCountryList = _covidCountryData;
            CountryList.ItemsSource = displayCovidCountryList;

            SearchEntry.TextChanged += SearchEntry_TextChanged;
        }

        SemaphoreSlim _searchEntryLock = new SemaphoreSlim(1);
        private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_searchEntryLock.Wait(0))
                return;

            var searchTerm = SearchEntry.Text.ToLower();

            List<CountryData> updatedList = null;
            if (string.IsNullOrWhiteSpace(searchTerm))
                updatedList = _covidCountryData;
            else
                updatedList = _covidCountryData.Where(c => c.CountryName.Length >= searchTerm.Length &&
                                                           (string.Equals(c.CountryName, searchTerm, StringComparison.InvariantCultureIgnoreCase)
                                                            || c.CountryName.ToLower().Contains(searchTerm))).ToList();
            CountryList.ItemsSource = updatedList;
            _searchEntryLock.Release();
        }

        protected async override void OnAppearing()
        {
            var covidDataResult = await _dataRepository.LoadCountryCovidCases();
            activityIndicator.IsRunning = false;

            errMsg.IsVisible = true;
            if (!covidDataResult.Succeeded)
                errMsg.Text = covidDataResult.ErrorMessage;
            else
            {
                _covidCountryData = covidDataResult.Result;
                InitializeUI();
            }
        }

        private void Country_Tapped(System.Object sender, System.EventArgs e)
        {
            var frame = sender as Frame;
            var bindingContext = frame?.BindingContext as CountryData;

            if (bindingContext == null)
                return;

            var countryDetailsPage = new CountryDetailsPage(bindingContext);

            countryDetailsPage.OnClose = () =>
            {
                MainLayout.Children.Remove(countryDetailsPage);
            };
            MainLayout.Children.Add(countryDetailsPage);
        }
    }
}
