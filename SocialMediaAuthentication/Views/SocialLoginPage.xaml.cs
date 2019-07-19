using SocialMediaAuthentication.ViewModels;
using Xamarin.Forms;

namespace SocialMediaAuthentication.Views
{
    public partial class SocialLoginPage : ContentPage
    {
        public SocialLoginPage()
        {
            InitializeComponent();
            this.BindingContext = new SocialLoginPageViewModel();
        }
    }
}
