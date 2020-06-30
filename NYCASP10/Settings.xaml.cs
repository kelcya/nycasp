using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NYCASP10
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object userid = localSettings.Values["userid"];
            if (userid == null)
            {
                localSettings.Values["userid"] = RandomString(20);
                Debug.WriteLine(localSettings.Values["userid"]);
            }
            Debug.WriteLine(localSettings.Values["userid"]);
            //random.Text = localSettings.Values["userid"].ToString();
            //random.Text = checkuser.ToString();
            object livetiles = localSettings.Values["livetiles"];
            object pushnotifications = localSettings.Values["notifications"];
            if (livetiles != null)
            {
                if (livetiles.Equals(true))
                {
                    tiles.IsOn = true;
                }
                else
                {
                    tiles.IsOn = false;
                }
            }
            if (pushnotifications != null)
            {
                if (pushnotifications.Equals(true))
                {
                    notifications.IsOn = true;
                }
                else
                {
                    notifications.IsOn = false;
                }
            }

        }
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void tiles_Toggled(object sender, RoutedEventArgs e)
        {
            HttpClient httpClient = new HttpClient();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object userid = localSettings.Values["userid"];
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    localSettings.Values["livetiles"] = true;
                    Uri tileuser = new Uri("https://www.kelcya.com/nycasp/10push.php?userid=" + userid + "&tile=1" + "&uri=" + localSettings.Values["channeluri"]);
                    // Create the web request.
                    
                    //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                    var response = await httpClient.GetAsync(tileuser);
                    push_tile();
                }
                else
                {
                    localSettings.Values["livetiles"] = false;
                    Uri tileuser = new Uri("https://www.kelcya.com/nycasp/10push.php?userid=" + userid + "&tile=0" + "&uri=" + localSettings.Values["channeluri"]);
                    // Create the web request.

                    //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                    var response = await httpClient.GetAsync(tileuser);
                    TileUpdateManager.CreateTileUpdaterForApplication().Clear();
                }
            }
           
        }

        private async void push_tile()
        {

            string xml = string.Empty;
            Uri url = new Uri("http://www.kelcya.com/nycasp/asp.xml");
            HttpClient httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            var response = await httpClient.GetAsync(url);
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            using (var streamReader = new StreamReader(responseStream))
            {
                xml = streamReader.ReadToEnd();
            }
            XDocument doc = XDocument.Parse(xml);
            string[] twitter;
            //string[] date;
            twitter = doc.Descendants("twitter").Select(o => o.Value).ToArray();
            //date = doc.Descendants("date2").Select(o => o.Value).ToArray();
            //twittertext.Text = twitter[0];


            // Construct the tile content
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {

                    TileMedium = new TileBinding()
                    {
                        //DisplayName = "NYCASP",
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            PeekImage = new TilePeekImage()
                            {
                                Source = new TileImageSource("Assets/Square150x150Logo.scale-200.png")
                            },
                            Children =
                            {
                                new TileText()
                                {

                                    Text = twitter[0],
                                    Style = TileTextStyle.BaseSubtle,
                                    Wrap = true
                                },
                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        //DisplayName = "NYCASP",
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            PeekImage = new TilePeekImage()
                            {
                                Source = new TileImageSource("Assets/Wide310x150Logo.scale-200.png")
                            },
                            Children =
                            {
                                new TileText()
                                {
                                    Text = twitter[0],
                                    Style = TileTextStyle.Subtitle,
                                    Wrap = true
                                },
                            }
                        }
                    }
                }
            };

            var notification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private async void notifications_Toggled(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object userid = localSettings.Values["userid"];
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            // Create the web request.
            HttpClient httpClient = new HttpClient();
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    localSettings.Values["notifications"] = true;
                    Uri tileuser = new Uri("https://www.kelcya.com/nycasp/10push.php?userid=" + userid + "&toast=1" + "&uri=" + localSettings.Values["channeluri"]);

                    //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                    var response = await httpClient.GetAsync(tileuser);
                }
                else
                {
                    localSettings.Values["notifications"] = false;
                    Uri tileuser = new Uri("https://www.kelcya.com/nycasp/10push.php?userid=" + userid + "&toast=0" + "&uri=" + localSettings.Values["channeluri"]);
                    var response = await httpClient.GetAsync(tileuser);
                }
            }
        }
    }
}
