using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using Xamarin.Forms;

namespace COVID19_DataApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private COVIDDataRepository _dataRepository;
        public MainPage(COVIDDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
            InitializeComponent();
        }

        private async void LoadCOVIDCases_Clicked(System.Object sender, System.EventArgs e)
        {
            var covidCases = await _dataRepository.LoadCovidCases();
        }
    }
}
