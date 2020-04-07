using System;
using System.Collections.Generic;
using DataLayer.Models;
using Xamarin.Forms;

namespace COVID19_DataApp
{
    public partial class CountryDetailsPage : Frame
    {
        public Action OnClose { get; set; }

        public CountryDetailsPage()
        {
            InitializeComponent();
        }

        private void ButtonClose_Clicked(System.Object sender, System.EventArgs e)
        {
            OnClose?.Invoke();
        }
    }
}
