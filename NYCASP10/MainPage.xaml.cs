using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.Networking.PushNotifications;
using System.Net;
using System.Diagnostics;
using NotificationsExtensions.Tiles;
using System.Net.NetworkInformation;
using Windows.UI.Popups;
using Windows.System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NYCASP10
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public LicenseInformation licenseInformation { get; set; }

        public class Holidays
        {
            public Holidays(string holidayname, DateTime holidate)
            {
                holiday = holidayname;
                date = holidate.ToString("dddd, MMMM d");
            }
            public string holiday { get; set; }
            public string date { get; set; }
            public Holidays()
            {
                holiday = "";
                date = "";
            }
        }

        public List<Holidays> pubholidays = new List<Holidays>();

        public MainPage()
        {
            this.InitializeComponent();
            Page_Loaded();
            bool isNetwork = NetworkInterface.GetIsNetworkAvailable();
            if (!isNetwork)
            {
                NoNetwork();
            }
            else
            {
                load();
                liveupdates();
                //pusher();
                createpushchannel();
                //livetiles();
            }

            pubholidays.Add(new Holidays("New Year's Day", new DateTime(2016, 1, 1)));
            pubholidays.Add(new Holidays("Martin Luther King, Jr.'s Birthday", new DateTime(2016, 1, 18)));
            pubholidays.Add(new Holidays("Asian Lunar New Year", new DateTime(2016, 2, 8)));
            pubholidays.Add(new Holidays("Ash Wednesday", new DateTime(2016, 2, 10)));
            pubholidays.Add(new Holidays("Lincoln's Birthday", new DateTime(2016, 2, 12)));
            pubholidays.Add(new Holidays("Washington's Birthday (Pres. Day)", new DateTime(2016, 2, 15)));
            pubholidays.Add(new Holidays("Purim and Holy Thursday", new DateTime(2016, 3, 24)));
            pubholidays.Add(new Holidays("Good Friday", new DateTime(2016, 3, 25)));
            pubholidays.Add(new Holidays("Passover 1st Day", new DateTime(2016, 4, 23)));
            pubholidays.Add(new Holidays("Passover 2nd Day", new DateTime(2016, 4, 24)));
            pubholidays.Add(new Holidays("Holy Thursday (Orthodox)", new DateTime(2016, 4, 28)));
            pubholidays.Add(new Holidays("Passover 7th Day and Good Friday (Orthodox)", new DateTime(2016, 4, 29)));
            pubholidays.Add(new Holidays("Passover 8th Day", new DateTime(2016, 4, 30)));
            pubholidays.Add(new Holidays("Solemnity of Ascension", new DateTime(2016, 5, 5)));
            pubholidays.Add(new Holidays("Memorial Day", new DateTime(2016, 5, 30)));
            pubholidays.Add(new Holidays("Shavuot", new DateTime(2016, 6, 12)));
            pubholidays.Add(new Holidays("Shavuot", new DateTime(2016, 6, 13)));
            pubholidays.Add(new Holidays("Independence Day", new DateTime(2016, 7, 4)));
            pubholidays.Add(new Holidays("Eid al-Fitr", new DateTime(2016, 7, 5)));
            pubholidays.Add(new Holidays("Eid al-Fitr", new DateTime(2016, 7, 6)));
            pubholidays.Add(new Holidays("Eid al-Fitr", new DateTime(2016, 7, 7)));
            pubholidays.Add(new Holidays("Feast of the Assumption", new DateTime(2016, 8, 15)));
            pubholidays.Add(new Holidays("Labor Day", new DateTime(2016, 9, 5)));
            pubholidays.Add(new Holidays("Eid al-Adha", new DateTime(2016, 9, 12)));
            pubholidays.Add(new Holidays("Eid al-Adha", new DateTime(2016, 9, 13)));
            pubholidays.Add(new Holidays("Eid al-Adha", new DateTime(2016, 9, 14)));
            pubholidays.Add(new Holidays("Rosh Hashanah", new DateTime(2016, 10, 3)));
            pubholidays.Add(new Holidays("Rosh Hashanah", new DateTime(2016, 10, 4)));
            pubholidays.Add(new Holidays("Columbus Day", new DateTime(2016, 10, 10)));
            pubholidays.Add(new Holidays("Yom Kippur", new DateTime(2016, 10, 12)));
            pubholidays.Add(new Holidays("Succoth", new DateTime(2016, 10, 17)));
            pubholidays.Add(new Holidays("Succoth", new DateTime(2016, 10, 18)));
            pubholidays.Add(new Holidays("Shemini Atzereth", new DateTime(2016, 10, 24)));
            pubholidays.Add(new Holidays("Simchas Torah", new DateTime(2016, 10, 25)));
            pubholidays.Add(new Holidays("Diwali", new DateTime(2016, 10, 30)));
            pubholidays.Add(new Holidays("All Saints Day", new DateTime(2016, 11, 1)));
            pubholidays.Add(new Holidays("Election Day", new DateTime(2016, 11, 8)));
            pubholidays.Add(new Holidays("Veterans Day", new DateTime(2016, 11, 11)));
            pubholidays.Add(new Holidays("Thanksgiving", new DateTime(2016, 11, 24)));
            pubholidays.Add(new Holidays("Immaculate Conception", new DateTime(2016, 12, 8)));
            pubholidays.Add(new Holidays("Christmas Day", new DateTime(2016, 12, 25)));
            pubholidays.Add(new Holidays("Christmas Day", new DateTime(2016, 12, 26)));

            calendar.DataContext = pubholidays;

            Holidays NextHoliday = new Holidays();

            NextHoliday = getnextholiday();

            NextHolidayDate.Text = NextHoliday.date;
            NextHolidayName.Text = NextHoliday.holiday;

            //NextHolidayDate.Text = getnextholiday();
        }

        private async void NoNetwork()
        {
            var dialog = new MessageDialog("No network connection. Please try again when internet is available!");
            await dialog.ShowAsync();
        }

        private async void createpushchannel()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            PushNotificationChannel channel = null;

            try
            {
                channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                localSettings.Values["channeluri"] = channel.Uri;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //Debug.WriteLine(localSettings.Values["channeluri"]);


        }
        private void livetiles()
        {
            //string xml = $@"
            //    <tile version='3'>
            //        <visual branding='nameAndLogo'>

            //            <binding template='TileMedium'>
            //                <text hint-wrap='true'>Suspended</text>
            //                <text hint-wrap='true' hint-style='captionSubtle'/>
            //            </binding>

            //            <binding template='TileWide'>
            //                <text hint-wrap='true'>New tile notification</text>
            //                <text hint-wrap='true' hint-style='captionSubtle'/>
            //            </binding>

            //            <binding template='TileLarge'>
            //                <text hint-wrap='true'>New tile notification</text>
            //                <text hint-wrap='true' hint-style='captionSubtle'/>
            //            </binding>

            //    </visual>
            //</tile>";


            string from = "Jennifer Parker";
            string subject = "Photos from our trip";
            string body = "Check out these awesome photos I took while in New Zealand!";


            // Construct the tile content
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new TileText()
                    {
                        Text = from
                    },

                    new TileText()
                    {
                        Text = subject,
                        Style = TileTextStyle.CaptionSubtle
                    },

                    new TileText()
                    {
                        Text = body,
                        Style = TileTextStyle.CaptionSubtle
                    }
                }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new TileText()
                    {
                        Text = from,
                        Style = TileTextStyle.Subtitle
                    },

                    new TileText()
                    {
                        Text = subject,
                        Style = TileTextStyle.CaptionSubtle
                    },

                    new TileText()
                    {
                        Text = body,
                        Style = TileTextStyle.CaptionSubtle
                    }
                }
                        }
                    }
                }
            };
            var notification = new TileNotification(content.GetXml());
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(xml);

            //string nowTimeString = DateTime.Now.ToString();

            //// Assign date/time values through XmlDocument to avoid any xml escaping issues
            //foreach (XmlElement textEl in doc.SelectNodes("//text").OfType<XmlElement>())
            //    if (textEl.InnerText.Length == 0)
            //        textEl.InnerText = nowTimeString;

            //TileNotification notification = new TileNotification(doc);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private async void pusher()
        {
            PushNotificationChannel channel = null;

            try
            {
                channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            }

            catch (Exception ex)
            {
                throw ex; 
            }

            Debug.WriteLine(channel.Uri);

            String serverUrl = "http://www.kelcya.com/";

            // Create the web request.
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(serverUrl);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            byte[] channelUriInBytes = System.Text.Encoding.UTF8.GetBytes("ChannelUri=" + channel.Uri);

            // Write the channel URI to the request stream.
            Stream requestStream = await webRequest.GetRequestStreamAsync();
            requestStream.Write(channelUriInBytes, 0, channelUriInBytes.Length);

            try
            {
                // Get the response from the server.
                WebResponse response = await webRequest.GetResponseAsync();
                StreamReader requestReader = new StreamReader(response.GetResponseStream());
                String webResponse = requestReader.ReadToEnd();
            }

            catch (Exception ex)
            {
                throw ex;
            }


        }

        public class XMLitem
        {
            public string status { get; set; }
            public string date { get; set; }
            public string meters { get; set; }
            public string day { get; set; }
        }

        public async void load()
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
            XDocument xDoc = XDocument.Parse(xml);
            //string value = (string)xDoc.Element("name");
            var parking = new XMLitem[2];
            int i = 0;
            foreach (XElement element in xDoc.Descendants("parking"))
            {
                parking[i] = new XMLitem();
                parking[i].date = element.Element("date").Value;
                parking[i].day = element.Element("day").Value;
                parking[i].status = element.Element("status").Value;
                parking[i].meters = element.Element("meters").Value;
                i++;
            }

            asptoday.Foreground = new SolidColorBrush(Colors.Green);
            asptomorrow.Foreground = new SolidColorBrush(Colors.Green);
            meterstoday.Foreground = new SolidColorBrush(Colors.Green);
            meterstomorrow.Foreground = new SolidColorBrush(Colors.Green);
            todaydate.Text = parking[1].day + ", " + parking[1].date;
            asptoday.Text = "ASP is " + parking[1].status;
            if (parking[1].status == "suspended")
            {
                asptoday.Foreground = new SolidColorBrush(Colors.Red);
            }
            meterstoday.Text = "Meters are " + parking[1].meters;
            if (parking[1].meters == "suspended")
            {
                meterstoday.Foreground = new SolidColorBrush(Colors.Red);
            }

            tomorrowdate.Text = parking[0].day + ", " + parking[0].date;
            asptomorrow.Text = "ASP is " + parking[0].status;
            if (parking[0].status == "suspended")
            {
                asptomorrow.Foreground = new SolidColorBrush(Colors.Red);
            }
            meterstomorrow.Text = "Meters are " + parking[0].meters;
            if (parking[0].meters == "suspended")
            {
                meterstomorrow.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private Holidays getnextholiday()
        {
            DateTime today = DateTime.Today;

            //today = new DateTime(2016, 12, 28);
            DateTime dateparsed = new DateTime();
            //string nextholiday;
            foreach (Holidays holi in pubholidays)
            {
                dateparsed = DateTime.Parse(holi.date + " 2016");
                if (dateparsed > today)
                {
                    return holi;
                }
            }
            Holidays holler = new Holidays("App update needed", new DateTime(2017, 1, 1));
            return holler;
        }
        public class RSSItem
        {
            public string title { get; set; }
        }
        public async void liveupdates()
        {
            string xml = string.Empty;
            Uri url = new Uri("http://twitrss.me/twitter_user_to_rss/?user=nycasp");
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            var response = await httpClient.GetAsync(url);
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            using (var streamReader = new StreamReader(responseStream))
            {
                xml = streamReader.ReadToEnd();
            }
            XDocument xDoc = XDocument.Parse(xml);
            //string value = (string)xDoc.Element("name");
            var tester = new RSSItem[30];
            var RSSdata = from rss in xDoc.Descendants("item")
                          select new RSSItem
                          {
                              title = rss.Element("title").Value,
                          };
            liveupdatesrss.ItemsSource = RSSdata;
            //int i = 0;
            //foreach (XElement element in xDoc.Descendants("item"))
            //{
            //    tester[i] = new RSSItem();
            //    tester[i].title = element.Element("title").Value;
            //    i++;
            //}
            //test.Text = tester[0].title;
        }

        private void RefreshBar_Click(object sender, RoutedEventArgs e)
        {
            bool isNetwork = NetworkInterface.GetIsNetworkAvailable();
            if (!isNetwork)
            {
                NoNetwork();
            }
            else
            {
                load();
                liveupdates();
            }
        }

        private void ReminderBar_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Remind));
        }

        private async void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            string productId = "Plusfeature";
            licenseInformation = CurrentApp.LicenseInformation;


            if (licenseInformation.IsActive)
            {
                if (licenseInformation.IsTrial)
                {
                    await CurrentApp.RequestAppPurchaseAsync(false);
                    if (!licenseInformation.IsTrial)
                    {
                        AdMediator_0EAFA0.Visibility = Visibility.Collapsed;
                        BuyButton.Visibility = Visibility.Collapsed;
                        ReminderButton.Visibility = Visibility.Visible;
                        SettingsButton.Visibility = Visibility.Visible;
                    }
                }
            }

            //licenseInformation = CurrentAppSimulator.LicenseInformation;
            //if (!licenseInformation.ProductLicenses[productId].IsActive)
            //{
            //    try
            //    {
            //        //PurchaseResults results = await CurrentAppSimulator.RequestProductPurchaseAsync(productId);
            //        await CurrentAppSimulator.RequestProductPurchaseAsync(productId);
            //        if (licenseInformation.ProductLicenses[productId].IsActive)
            //        {
            //            AdMediator_0EAFA0.Visibility = Visibility.Collapsed;
            //            BuyButton.Visibility = Visibility.Collapsed;
            //            ReminderButton.Visibility = Visibility.Visible;
            //            SettingsButton.Visibility = Visibility.Visible;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //}
            //else
            //{

            //}

        }

        private void Page_Loaded()
        {
            //StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            //StorageFile proxyFile = await proxyDataFolder.GetFileAsync("test.xml");
            //await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);

            licenseInformation = CurrentApp.LicenseInformation;

            //licenseInformation.LicenseChanged += new LicenseChangedEventHandler(licenseChangedEventHandler);


            if (licenseInformation.IsActive)
            {
                if (licenseInformation.IsTrial)
                {
                    // Show the features that are available during trial only.

                    AdMediator_0EAFA0.Visibility = Visibility.Visible;
                    BuyButton.Visibility = Visibility.Visible;
                    ReminderButton.Visibility = Visibility.Collapsed;
                    SettingsButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Show the features that are available only with a full license.

                    AdMediator_0EAFA0.Visibility = Visibility.Collapsed;
                    BuyButton.Visibility = Visibility.Collapsed;
                    ReminderButton.Visibility = Visibility.Visible;
                    SettingsButton.Visibility = Visibility.Visible;
                }
            }
            //if (licenseInformation.ProductLicenses["Plusfeature"].IsActive)
            //{
            //    AdMediator_0EAFA0.Visibility = Visibility.Collapsed;
            //    BuyButton.Visibility = Visibility.Collapsed;
            //    ReminderButton.Visibility = Visibility.Visible;
            //    SettingsButton.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    AdMediator_0EAFA0.Visibility = Visibility.Visible;
            //    BuyButton.Visibility = Visibility.Visible;
            //    ReminderButton.Visibility = Visibility.Collapsed;
            //    SettingsButton.Visibility = Visibility.Collapsed;
            //}
        }

        void licenseChangedEventHandler()
        {
            ReloadLicense(); // code is in next steps
        }

        void ReloadLicense()
        {
            if (licenseInformation.IsActive)
            {
                if (licenseInformation.IsTrial)
                {
                    // Show the features that are available during trial only.

                    AdMediator_0EAFA0.Visibility = Visibility.Visible;
                    BuyButton.Visibility = Visibility.Visible;
                    ReminderButton.Visibility = Visibility.Collapsed;
                    SettingsButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Show the features that are available only with a full license.

                    //AdMediator_0EAFA0.Visibility = Visibility.Collapsed;
                    //BuyButton.Visibility = Visibility.Collapsed;
                    //ReminderButton.Visibility = Visibility.Visible;
                    //SettingsButton.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // A license is inactive only when there' s an error.
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }
        private async void Rate_Click(object sender, RoutedEventArgs e)
        {
            //add your app id here
            await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:REVIEW?PFN={0}", Windows.ApplicationModel.Package.Current.Id.FamilyName)));
        }

        private async void Email_Click(object sender, RoutedEventArgs e)
        {
            var mailto = new Uri("mailto:?to=kelvin.yang@outlook.com&subject=[NYCASP10] Comments");
            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }
    }
}
