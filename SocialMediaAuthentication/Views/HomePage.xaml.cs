using System;
using System.Collections.Generic;
using Plugin.FacebookClient;
using Plugin.GoogleClient;
using SocialMediaAuthentication.Models;
using Xamarin.Forms;

namespace SocialMediaAuthentication.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage(NetworkAuthData networkAuthData)
        {
            BindingContext = networkAuthData;
            InitializeComponent();
        }

       async void OnLogout(object sender, System.EventArgs e)
        {
           if(BindingContext is NetworkAuthData data)
           {
                switch(data.Name)
                {
                    case "Facebook":
                        CrossFacebookClient.Current.Logout();
                        break;
                    case "Google":
                        CrossGoogleClient.Current.Logout();
                        break;
                }

               await Navigation.PopModalAsync();
            }
        }
    }
}
