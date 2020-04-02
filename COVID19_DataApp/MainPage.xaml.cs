using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
                errMsg.Text = "Successfully loaded data from https://www.worldometers.info/coronavirus";
                InitializeUI();
            }
        }
    }
}
