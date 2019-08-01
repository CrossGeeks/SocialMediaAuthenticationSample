using System;
using System.Collections.Generic;
using Plugin.FacebookClient;
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

        void OnLogout(object sender, System.EventArgs e)
        {
           if(BindingContext is NetworkAuthData data)
           {
                switch(data.Name)
                {
                    case "Facebook":
                        CrossFacebookClient.Current.Logout();
                        break;
                }

                Navigation.PopModalAsync();
            }
        }
    }
}
