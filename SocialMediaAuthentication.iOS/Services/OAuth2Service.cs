using System;
using System.Collections.Generic;
using SocialMediaAuthentication.Services;
using UIKit;
using Xamarin.Auth;

namespace SocialMediaAuthentication.iOS.Services
{
    public class iOSOAuth2Authenticator : OAuth2Authenticator
    {
        public iOSOAuth2Authenticator(string clientId, string scope, Uri authorizeUrl, Uri redirectUrl) : base(clientId, scope, authorizeUrl, redirectUrl)
        {

        }

        protected override void OnPageEncountered(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
        {
            if (UrlMatchesRedirect(url))

                base.OnPageEncountered(url, query, fragment);
        }

    }

    public class OAuth2Service : IOAuth2Service
    {
        public event EventHandler<string> OnSuccess = delegate { };
        public event EventHandler OnCancel = delegate { };
        public event EventHandler<string> OnError = delegate { };

        public void Authenticate(string clientId, string scope, Uri authorizeUrl, Uri redirectUrl)
        {

            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            var auth = new iOSOAuth2Authenticator(
                clientId: clientId, // your OAuth2 client id
                scope: scope, // the scopes for the particular API you're accessing, delimited by "+" symbols
                authorizeUrl: authorizeUrl, // the auth URL for the service
                redirectUrl: redirectUrl); // the redirect URL for the service

            auth.AllowCancel = true;
            auth.ShowErrors = false;
            EventHandler<AuthenticatorErrorEventArgs> errorDelegate = null;
            EventHandler<AuthenticatorCompletedEventArgs> completedDelegate = null;

            errorDelegate = (sender, eventArgs) =>
            {
                OnError?.Invoke(this, eventArgs.Message);

                auth.Error -= errorDelegate;
                auth.Completed -= completedDelegate;
            };

            completedDelegate = (sender, eventArgs) => {



                // UI presented, so it's up to us to dimiss it on iOS
                // dismiss ViewController with UIWebView or SFSafariViewController
                vc.DismissViewController(true, null);


                if (eventArgs.IsAuthenticated)
                {

                    OnSuccess?.Invoke(this, eventArgs.Account.Properties["access_token"]);

                }
                else
                {
                    // The user cancelled

                    OnCancel?.Invoke(this, EventArgs.Empty);
                }
                auth.Error -= errorDelegate;
                auth.Completed -= completedDelegate;

            };

            auth.Error += errorDelegate;
            auth.Completed += completedDelegate;

            vc.PresentViewController(auth.GetUI(), true, null);
        }

    }
}
