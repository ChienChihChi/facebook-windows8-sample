using System;
using System.Collections.Generic;
using System.Dynamic;
using facebook_metro_sample.Views;
using Msn;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace funtown_metro_sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MsnInfoPage : Page
    {
        private readonly MsnClient _msn = new MsnClient();

        public MsnInfoPage()
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
            dynamic parameter = e.Parameter;
            _msn.AccessToken = parameter.access_token;

            LoadFuntownData();
        }

        private void LoadFuntownData()
        {
            GetUserProfilenName();
        }

        private async void GetUserProfilenName()
        {
            try
            {
                dynamic result = await _msn.GetTaskAsync("v5.0/me");
                string id = result.id;
                ProfileName.Text = "Hi " + id;
            }
            catch (MsnApiException ex)
            {
                // handel error message
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            dynamic parameters = new ExpandoObject();
            WebView1.Navigate(_msn.GetLogoutUrl(parameters));
        }

        private void WebView1_LoadCompleted(object sender, NavigationEventArgs e)
        {
            Frame.Navigate(typeof(HomePage));
        }
    }
}
