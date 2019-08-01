using SocialMediaAuthentication.Services;
using SocialMediaAuthentication.ViewModels;
using Xamarin.Forms;

namespace SocialMediaAuthentication.Views
{
    public partial class SocialLoginPage : ContentPage
    {
        public SocialLoginPage(IOAuth2Service oAuth2Service)
        {
            InitializeComponent();
            this.BindingContext = new SocialLoginPageViewModel(oAuth2Service);
        }
    }
}
