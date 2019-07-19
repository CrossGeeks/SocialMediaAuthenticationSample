using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Plugin.FacebookClient;
using Plugin.FacebookClient.Abstractions;
using SocialMediaAuthentication.Models;
using SocialMediaAuthentication.Views;
using Xamarin.Forms;

namespace SocialMediaAuthentication.ViewModels
{
    public class SocialLoginPageViewModel
    {
        public ICommand OnLoginWithFacebookCommand { get; set; }

        IFacebookClient _facebookService = CrossFacebookClient.Current;
        public SocialLoginPageViewModel()
        {
            OnLoginWithFacebookCommand = new Command(async () => await LoginFacebookAsync());
        }

        async Task LoginFacebookAsync()
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
                                Email = facebookProfile.Email,
                                Name = $"{facebookProfile.FirstName} {facebookProfile.LastName}",
                                Id = facebookProfile.UserId
                            };
                            await App.Current.MainPage.Navigation.PushModalAsync(new HomePage());
                            break;
                        case FacebookActionStatus.Canceled:
                            break;
                    }

                    _facebookService.OnUserData -= userDataDelegate;
                };

                _facebookService.OnUserData += userDataDelegate;

                string[] fbRequestFields = { "email", "first_name", "gender", "last_name" };
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
