using System;
using System.Collections.Generic;
using System.Dynamic;
using funtown_metro_sample.Views;
using Msn;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace facebook_metro_sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MsnLoginPage : Page
    {
        private const string AppId = ""

        /// <summary>
        /// Extended permissions is a comma separated list of permissions to ask the user.
        /// </summary>
        /// <remarks>
        /// For extensive list of available extended permissions refer to 
        /// https://developers.facebook.com/docs/reference/api/permissions/
        /// </remarks>
        private const string ExtendedPermissions = ""

        private readonly MsnClient _msn = new MsnClient();

        public MsnLoginPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void WebView1_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            WebView1.LoadCompleted += WebView1_LoadCompleted;
            
            var loginUrl = GetMsnLoginUrl(AppId, ExtendedPermissions);
            WebView1.Navigate(loginUrl);
        }

        private Uri GetMsnLoginUrl(string appId, string extendedPermissions)
        {
            dynamic parameters = new ExpandoObject();
            parameters.client_id = appId;
            parameters.redirect_uri = "https://weblogin.funtown.com.tw/oauth/login_success.html";
            parameters.response_type = "token";

            // add the 'scope' parameter only if we have extendedPermissions.
            if (!string.IsNullOrWhiteSpace(extendedPermissions))
            {
                // A comma-delimited list of permissions
                parameters.scope = extendedPermissions;
            }

            return _msn.GetLoginUrl(parameters);
        }

        private void WebView1_LoadCompleted(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            MsnOAuthResult oauthResult;
            if (!_msn.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                return;
            }

            if (oauthResult.IsSuccess)
            {
                var error_code = oauthResult.ErrorCode;
                var code = oauthResult.Code;
                var accessToken = oauthResult.AccessToken;
                
                if (!string.IsNullOrEmpty(code))
                {
                    RequestToken(code);
                    return;
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    LoginSucceded(accessToken);
                    return;
                }
            }
            else
            {
                // user cancelled
            }
        }

        private void LoginSucceded(string accessToken)
        {
            dynamic parameters = new ExpandoObject();
            parameters.access_token = accessToken;

            Frame.Navigate(typeof(MsnInfoPage), (object)parameters);
        }

        private async void RequestToken(string code)
        {
            dynamic parameters = new ExpandoObject();
            parameters.client_id = AppId;
            parameters.client_secret = ""
            parameters.redirect_uri = "https://weblogin.funtown.com.tw/oauth/login_success.html";
            parameters.grant_type = "authorization_code";
            parameters.code = code;

            dynamic result = await _msn.GetTaskAsync("oauth20_token.srf", parameters);
            string accessToken = result.access_token;
            var errorCode = result.error_code;

            if (!string.IsNullOrEmpty(accessToken))
            {
                LoginSucceded(accessToken);
            }
        }
    }
}
