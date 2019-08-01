using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Newtonsoft.Json;
using Plugin.FacebookClient;
using Plugin.FacebookClient.Abstractions;
using Refit;
using SocialMediaAuthentication.Models;
using SocialMediaAuthentication.Services;
using SocialMediaAuthentication.Views;
using Xamarin.Forms;

namespace SocialMediaAuthentication.ViewModels
{
    public class SocialLoginPageViewModel
    {
        const string InstagramApiUrl = "https://api.instagram.com";
        const string InstagramScope = "basic";
        const string InstagramAuthorizationUrl = "https://api.instagram.com/oauth/authorize/";
        const string InstagramRedirectUrl = "https://xamboy.com/default.html";
        const string InstagramClientId = "77567512de424a528ba61715d389a644";

        public ICommand OnLoginCommand { get; set; }

        IFacebookClient _facebookService = CrossFacebookClient.Current;
        IOAuth2Service _oAuth2Service;

        public ObservableCollection<AuthNetwork> AuthenticationNetworks { get; set; } = new ObservableCollection<AuthNetwork>()
        {
            new AuthNetwork()
            {
                Name = "Facebook",
                Icon = "ic_fb",
                Background = "#4768AD"
            },
             new AuthNetwork()
            {
                Name = "Instagram",
                Icon = "ic_ig",
                Background = "#DD2A7B"
            }
        };


        public SocialLoginPageViewModel(IOAuth2Service oAuth2Service)
        {
            _oAuth2Service = oAuth2Service;
            
            OnLoginCommand = new Command<AuthNetwork>(async (data) => await LoginAsync(data));
        }
        async Task LoginAsync(AuthNetwork authNetwork)
        {
            switch(authNetwork.Name)
            {
                case "Facebook":
                    await LoginFacebookAsync(authNetwork);
                    break;
                case "Instagram":
                    await LoginInstagramAsync(authNetwork);
                    break;
            }
        }
        async Task LoginInstagramAsync(AuthNetwork authNetwork)
        {
            EventHandler<string> onSuccessDelegate = null;
            EventHandler<string> onErrorDelegate = null;
            EventHandler onCancelDelegate = null;

            onSuccessDelegate = async (s, a) =>
            {

               UserDialogs.Instance.ShowLoading("Loading");

                var userResponse = await RestService.For<IInstagramApi>(InstagramApiUrl).GetUser(a);

                if (userResponse.IsSuccessStatusCode)
                {
                    var userDataString = await userResponse.Content.ReadAsStringAsync();
                    //Handling Encoding
                    var userDataStringFixed = System.Text.RegularExpressions.Regex.Unescape(userDataString);

                    var instagramUser = JsonConvert.DeserializeObject<InstagramUser>(userDataStringFixed);
                    var socialLoginData = new NetworkAuthData
                    {
                        Logo = authNetwork.Icon,
                        Picture = instagramUser.Data.ProfilePicture,
                        Background = authNetwork.Background,
                        Name = instagramUser.Data.FullName,
                        Id = instagramUser.Data.Id
                    };

                    UserDialogs.Instance.HideLoading();
                    await App.Current.MainPage.Navigation.PushModalAsync(new HomePage(socialLoginData));
                }
                else
                {
                    //TODO: Handle instagram user info error
                   UserDialogs.Instance.HideLoading();

                   await UserDialogs.Instance.AlertAsync("Error","Houston we have a problem" , "Ok");
                }

                _oAuth2Service.OnSuccess -= onSuccessDelegate;
                _oAuth2Service.OnCancel -= onCancelDelegate;
                _oAuth2Service.OnError -= onErrorDelegate;
            };
            onErrorDelegate = (s, a) =>
            {
                _oAuth2Service.OnSuccess -= onSuccessDelegate;
                _oAuth2Service.OnCancel -= onCancelDelegate;
                _oAuth2Service.OnError -= onErrorDelegate;
                Debug.WriteLine($"ERROR: Instagram, MESSAGE: {a}");
            };
            onCancelDelegate = (s, a) =>
            {
                _oAuth2Service.OnSuccess -= onSuccessDelegate;
                _oAuth2Service.OnCancel -= onCancelDelegate;
                _oAuth2Service.OnError -= onErrorDelegate;
            };
           
            _oAuth2Service.OnSuccess += onSuccessDelegate;
            _oAuth2Service.OnCancel += onCancelDelegate;
            _oAuth2Service.OnError += onErrorDelegate;
            _oAuth2Service.Authenticate(InstagramClientId, InstagramScope, new Uri(InstagramAuthorizationUrl), new Uri(InstagramRedirectUrl));


        }

        async Task LoginFacebookAsync(AuthNetwork authNetwork)
        {
            try
            {

                if (_facebookService.IsLoggedIn)
                {
                    _facebookService.Logout();
                }

                EventHandler<FBEventArgs<string>> userDataDelegate = null;

                userDataDelegate = async (object sender, FBEventArgs<string> e) =>
                {
                    if (e == null) return;

                    switch (e.Status)
                    {
                        case FacebookActionStatus.Completed:
                            var facebookProfile = await Task.Run(() => JsonConvert.DeserializeObject<FacebookProfile>(e.Data));
                            var socialLoginData = new NetworkAuthData
                            {
                                Id = facebookProfile.Id,
                                Logo = authNetwork.Icon,
                                Background = authNetwork.Background,
                                Picture = facebookProfile.Picture.Data.Url,
                                Name = $"{facebookProfile.FirstName} {facebookProfile.LastName}",
                            };
                            await App.Current.MainPage.Navigation.PushModalAsync(new HomePage(socialLoginData));
                            break;
                        case FacebookActionStatus.Canceled:
                            break;
                    }

                    _facebookService.OnUserData -= userDataDelegate;
                };

                _facebookService.OnUserData += userDataDelegate;

                string[] fbRequestFields = { "email", "first_name", "picture", "gender", "last_name" };
                string[] fbPermisions = { "email" };
                await _facebookService.RequestUserDataAsync(fbRequestFields, fbPermisions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
