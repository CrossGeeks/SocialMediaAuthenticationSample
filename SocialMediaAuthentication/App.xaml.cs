using System;
using SocialMediaAuthentication.Services;
using SocialMediaAuthentication.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SocialMediaAuthentication
{
    public partial class App : Application
    {
      
        public App(IOAuth2Service oAuth2Service)
        {
            InitializeComponent();
           
            MainPage = new NavigationPage(new SocialLoginPage(oAuth2Service));
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
