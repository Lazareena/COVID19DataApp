﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Models;
using DataLayer.ViewModel;
using Xamarin.Forms;

namespace COVID19_DataApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private COVIDDataRepository _dataRepository;
        private List<CovidCountryViewModel> _covidCountryData;
        /// <summary>
        /// Country Data grouped by Continent
        /// </summary>
        private List<IGrouping<string,CovidCountryViewModel>> _displayedCovidCountryGroupedList;
        private CovidCountryViewModel  _worldData;

        public MainPage(COVIDDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
            InitializeComponent();
        }

        //TODO: Setup UI for displaying COVID 19 data by country
        public void InitializeUI()
        {
            CountryList.IsVisible = true;
            _displayedCovidCountryGroupedList = _covidCountryData.GroupBy(c => c.Continent).ToList();
            WorldLayout.BindingContext = _worldData;
            CountryList.ItemsSource = _displayedCovidCountryGroupedList;
            SearchEntry.TextChanged += SearchEntry_TextChanged;
        }

        SemaphoreSlim _searchEntryLock = new SemaphoreSlim(1);
        private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_searchEntryLock.Wait(0))
                return;

            var searchTerm = SearchEntry.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchTerm))
                _displayedCovidCountryGroupedList = _covidCountryData.GroupBy(c => c.Continent).ToList(); 
            else
                _displayedCovidCountryGroupedList = _covidCountryData.Where(c => c.CountryName.Length >= searchTerm.Length &&
                                                           (string.Equals(c.CountryName, searchTerm, StringComparison.InvariantCultureIgnoreCase)
                                                            || c.CountryName.ToLower().Contains(searchTerm))).GroupBy(c => c.Continent).ToList(); 
            CountryList.ItemsSource = _displayedCovidCountryGroupedList;
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
                _worldData = _dataRepository.GetTotalWorldNoLoad();
                InitializeUI();
            }
        }

        private void Country_Tapped(System.Object sender, System.EventArgs e)
        {
            var collectionView = sender as CollectionView;
            var bindingContext = collectionView?.SelectedItem as CovidCountryViewModel;

            if (collectionView == null || collectionView.SelectionMode == SelectionMode.None || bindingContext == null)
                return;

            collectionView.SelectedItem = null;
            CountryList.SelectionMode = SelectionMode.None;

            var countryDetailsPage = new CountryDetailsPage();
            countryDetailsPage.BindingContext = bindingContext;

            countryDetailsPage.OnClose = () =>
            {
                MainLayout.Children.Remove(countryDetailsPage);
                CountryList.SelectionMode = SelectionMode.Single;
            };
            MainLayout.Children.Add(countryDetailsPage);
        }

        private void DaySwitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            _worldData.DisplayNewCasesToday = e.Value;
            if(_displayedCovidCountryGroupedList != null)
            {
                foreach (var continent in _displayedCovidCountryGroupedList)
                {
                    foreach (var c in continent)
                        c.DisplayNewCasesToday = e.Value;
                }
            }
        }
    }
}
