using System;
using Plugin.CurrentActivity;
using SocialMediaAuthentication.Services;
using Xamarin.Auth;

namespace SocialMediaAuthentication.Droid.Services
{
    public class DroidOAuth2Authenticator : OAuth2Authenticator
    {
        public DroidOAuth2Authenticator(string clientId, string scope, Uri authorizeUrl, Uri redirectUrl) : base(clientId, scope, authorizeUrl, redirectUrl)
        {

        }

        protected override void OnPageEncountered(Uri url, System.Collections.Generic.IDictionary<string, string> query, System.Collections.Generic.IDictionary<string, string> fragment)
        {
            // Remove state from dictionaries. 
            // We are ignoring request state forgery status 
            // as we're hitting an ASP.NET service which forwards 
            // to a third-party OAuth service itself
            if (query.ContainsKey("state"))
            {
                query.Remove("state");
            }

            if (fragment.ContainsKey("state"))
            {
                fragment.Remove("state");
            }

            base.OnPageEncountered(url, query, fragment);
        }
    }

    public class OAuth2Service: IOAuth2Service
    {
        public event EventHandler<string> OnSuccess = delegate { };
        public event EventHandler OnCancel = delegate { };
        public event EventHandler<string> OnError = delegate { };

        public void Authenticate(string clientId, string scope, Uri authorizeUrl, Uri redirectUrl)
        {
            var activity = CrossCurrentActivity.Current.Activity;

            var auth = new DroidOAuth2Authenticator(
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

                // UI presented, so it's up to us to dimiss it on Android
                // dismiss Activity with WebView or CustomTabs
                CrossCurrentActivity.Current.Activity.Finish();

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

            activity.StartActivity(auth.GetUI(activity));
        }

    }
}
