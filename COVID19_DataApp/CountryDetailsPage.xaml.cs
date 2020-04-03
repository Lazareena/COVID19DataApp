using System;
using System.Collections.Generic;
using DataLayer.Models;
using Xamarin.Forms;

namespace COVID19_DataApp
{
    public partial class CountryDetailsPage : Frame
    {
        private CountryData _countryData;
        public Action OnClose { get; set; }

        public CountryDetailsPage(CountryData countryData)
        {
            _countryData = countryData;
            InitializeComponent();
            this.BindingContext = countryData;
        }

        private void ButtonClose_Clicked(System.Object sender, System.EventArgs e)
        {
            OnClose?.Invoke();
        }
    }
}
