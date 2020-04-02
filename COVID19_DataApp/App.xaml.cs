using System;
using DataLayer;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace COVID19_DataApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage(new COVIDDataRepository(new HttpClientMgr()));
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
