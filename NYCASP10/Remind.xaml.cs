using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NYCASP10
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Remind : Page
    {
        public Remind()
        {
            this.InitializeComponent();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            //localSettings.Values["ReminderOn"] = false;
            var notifier = ToastNotificationManager.CreateToastNotifier();
            var scheduled = notifier.GetScheduledToastNotifications();
            var itemId = "Toaster2";

            if (scheduled.Count > 0)
            {
                for (int i = 0, len = scheduled.Count; i < len; i++)
                {
                    // The itemId value is the unique ScheduledTileNotification.Id assigned to the 
                    // notification when it was created.
                    if (scheduled[i].Id == itemId)
                    {
                        object location = localSettings.Values["Location"];
                        CarLocation.Text = location.ToString();
                        object parkedday = localSettings.Values["ParkedOnDay"];
                        ParkedOnDay.SelectedValue = parkedday;
                        object time = localSettings.Values["PickedTime"];
                        PickedTime.Time = (TimeSpan)time;
                        object remindtime = localSettings.Values["RemindTime"];
                        RemindTime.SelectedValue = remindtime;
                        ReminderSet.Text = "Reminder is set";
                    }
                }
            }
            else
            {
                ReminderSet.Text = "Reminder is not set";
            }
            if (localSettings.Values["remindertime"] != null)
            {
                if ((DateTimeOffset)localSettings.Values["remindertime"] < DateTime.Now)
                {
                    ReminderSet.Text = "Reminder is not set";
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["Location"] = CarLocation.Text;
            localSettings.Values["ParkedOnDay"] = ParkedOnDay.SelectedValue;
            localSettings.Values["PickedTime"] = PickedTime.Time;
            //localSettings.Values["ReminderOn"] = true;
            localSettings.Values["RemindTime"] = RemindTime.SelectedValue;

            TimeSpan targettime = PickedTime.Time;
            DateTime newtime = new DateTime();

            DateTime today = DateTime.Today;
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilParkedDay = 0;

            if (ParkedOnDay.SelectedIndex == 0)
            {
                daysUntilParkedDay = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
                if (DateTime.Today.DayOfWeek.ToString() == "Monday")
                {
                    if (DateTime.Now.TimeOfDay > targettime)
                    {
                        daysUntilParkedDay = (((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7) + 7;
                    }
                }
            }
            else if (ParkedOnDay.SelectedIndex == 1)
            {
                daysUntilParkedDay = ((int)DayOfWeek.Tuesday - (int)today.DayOfWeek + 7) % 7;
                if (DateTime.Today.DayOfWeek.ToString() == "Tuesday")
                {
                    if (DateTime.Now.TimeOfDay > targettime)
                    {
                        daysUntilParkedDay = (((int)DayOfWeek.Tuesday - (int)today.DayOfWeek + 7) % 7) + 7;
                    }
                }
            }
            else if (ParkedOnDay.SelectedIndex == 2)
            {
                daysUntilParkedDay = ((int)DayOfWeek.Wednesday - (int)today.DayOfWeek + 7) % 7;
                if (DateTime.Today.DayOfWeek.ToString() == "Wednesday")
                {
                    if (DateTime.Now.TimeOfDay > targettime)
                    {
                        daysUntilParkedDay = (((int)DayOfWeek.Wednesday - (int)today.DayOfWeek + 7) % 7) + 7;
                    }
                }
            }
            else if (ParkedOnDay.SelectedIndex == 3)
            {
                daysUntilParkedDay = ((int)DayOfWeek.Thursday - (int)today.DayOfWeek + 7) % 7;
                if (DateTime.Today.DayOfWeek.ToString() == "Thursday")
                {
                    if (DateTime.Now.TimeOfDay > targettime)
                    {
                        daysUntilParkedDay = (((int)DayOfWeek.Thursday - (int)today.DayOfWeek + 7) % 7) + 7;
                    }
                }
            }
            else if (ParkedOnDay.SelectedIndex == 4)
            {
                daysUntilParkedDay = ((int)DayOfWeek.Friday - (int)today.DayOfWeek + 7) % 7;
                if (DateTime.Today.DayOfWeek.ToString() == "Friday")
                {
                    if (DateTime.Now.TimeOfDay > targettime)
                    {
                        daysUntilParkedDay = (((int)DayOfWeek.Friday - (int)today.DayOfWeek + 7) % 7) + 7;
                    }
                }
            }
            else if (ParkedOnDay.SelectedIndex == 5)
            {
                daysUntilParkedDay = ((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7;
                if (DateTime.Today.DayOfWeek.ToString() == "Saturday")
                {
                    if (DateTime.Now.TimeOfDay > targettime)
                    {
                        daysUntilParkedDay = (((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7) + 7;
                    }
                }
            }
            else if (ParkedOnDay.SelectedIndex == 6)
            {
                daysUntilParkedDay = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;
                if (DateTime.Today.DayOfWeek.ToString() == "Sunday")
                {
                    if (DateTime.Now.TimeOfDay > targettime)
                    {
                        daysUntilParkedDay = (((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7) + 7;
                    }
                }
            }

            DateTime nextParkedDay = today.AddDays(daysUntilParkedDay);
            nextParkedDay = new DateTime(nextParkedDay.Year, nextParkedDay.Month, nextParkedDay.Day, targettime.Hours, targettime.Minutes, targettime.Seconds);
            //MessageBox.Show(nextParkedDay.ToString());

            if (RemindTime.SelectedIndex == 0)
            {
                newtime = nextParkedDay.AddMinutes(-5);
                if (DateTime.Now > newtime)
                {
                    newtime = newtime.AddDays(7);
                }
                //MessageBox.Show(newtime.ToString());
            }
            else if (RemindTime.SelectedIndex == 1)
            {
                newtime = nextParkedDay.AddMinutes(-10);
                if (DateTime.Now > newtime)
                {
                    newtime = newtime.AddDays(7);
                }
                //MessageBox.Show(newtime.ToString());
            }
            else if (RemindTime.SelectedIndex == 2)
            {
                newtime = nextParkedDay.AddMinutes(-15);
                if (DateTime.Now > newtime)
                {
                    newtime = newtime.AddDays(7);
                }
                //MessageBox.Show(newtime.ToString());
            }
            else if (RemindTime.SelectedIndex == 3)
            {
                newtime = nextParkedDay.AddMinutes(-30);
                if (DateTime.Now > newtime)
                {
                    newtime = newtime.AddDays(7);
                }
                //MessageBox.Show(newtime.ToString());
            }
            else if (RemindTime.SelectedIndex == 4)
            {
                newtime = nextParkedDay.AddHours(-1);
                if (DateTime.Now > newtime)
                {
                    newtime = newtime.AddDays(7);
                }
                //MessageBox.Show(newtime.ToString());
            }
            else if (RemindTime.SelectedIndex == 5)
            {
                newtime = nextParkedDay.AddHours(-2);
                if (DateTime.Now > newtime)
                {
                    newtime = newtime.AddDays(7);
                }
                //MessageBox.Show(newtime.ToString());
            }
            else if (RemindTime.SelectedIndex == 6)
            {
                newtime = nextParkedDay.AddHours(-4);
                if (DateTime.Now > newtime)
                {
                    newtime = newtime.AddDays(7);
                }
                //MessageBox.Show(newtime.ToString());
            }
            else if (RemindTime.SelectedIndex == 7)
            {
                newtime = nextParkedDay.AddHours(-12);
                if (DateTime.Now > newtime)
                {
                    newtime = newtime.AddDays(7);
                }
                //MessageBox.Show(newtime.ToString());
            }
            else if (RemindTime.SelectedIndex == 8)
            {
                newtime = nextParkedDay.AddDays(-1);
                if (DateTime.Now > newtime)
                {
                    newtime = newtime.AddDays(7);
                }
                //MessageBox.Show(newtime.ToString());
            }
            string movecarmaintext = "Move your car on " + localSettings.Values["Location"];
            string movecarsubtext = "No Parking " + nextParkedDay.ToString();
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(movecarmaintext));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(movecarsubtext));

            DateTime dueTime = newtime;
            ScheduledToastNotification scheduledToast = new ScheduledToastNotification(toastXml, dueTime);
            scheduledToast.Id = "Toaster2";

            ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
            //ReminderSet.Text = newtime.ToString();
            ReminderSet.Text = "Reminder is set for " + dueTime.ToString();
            localSettings.Values["remindertime"] = (DateTimeOffset)newtime;
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            //localSettings.Values["ReminderOn"] = false;

            var notifier = ToastNotificationManager.CreateToastNotifier();
            var scheduled = notifier.GetScheduledToastNotifications();

            var itemId = "Toaster2";

            for (int i = 0, len = scheduled.Count; i < len; i++)
            {

                // The itemId value is the unique ScheduledTileNotification.Id assigned to the 
                // notification when it was created.
                if (scheduled[i].Id == itemId)
                {
                    notifier.RemoveFromSchedule(scheduled[i]);
                }
            }
            ReminderSet.Text = "Reminder is not set";
        }
    }
}
