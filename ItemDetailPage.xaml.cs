﻿using FriendsWithPaws.Data;
using Windows.ApplicationModel.DataTransfer;
using System.Text;
using Windows.Storage.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Callisto.Controls;
using Windows.UI.StartScreen;
using Windows.UI.Notifications;
using Windows.UI.Popups;
// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace FriendsWithPaws
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage : FriendsWithPaws.Common.LayoutAwarePage
    {
        public ItemDetailPage()
        {
            this.InitializeComponent();
        }
        void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            var item = (BreedDataItem)this.flipView.SelectedItem;
            request.Data.Properties.Title = item.Title;
            request.Data.Properties.Description = "Breed details and description";

            var breed = "\r\nBreed Details\r\n";
            breed += String.Join("\r\n", item.Breed_details);
            breed += ("\r\n\r\nDescription\r\n" + item.Description);

            request.Data.SetText(breed);

            var reference = RandomAccessStreamReference.CreateFromUri(new Uri(item.ImagePath.AbsoluteUri));

            request.Data.Properties.Thumbnail = reference;
            request.Data.SetBitmap(reference);
        }
        private void OnRecordingButtonClicked(object sender, RoutedEventArgs e)
        {
            var menu = new Menu();
            var flyout = new Flyout();
            flyout.Placement = PlacementMode.Top;
            flyout.HorizontalAlignment = HorizontalAlignment.Left;
            flyout.HorizontalContentAlignment = HorizontalAlignment.Left;
            flyout.Content = menu;
            flyout.IsOpen = true;

        }
        private async void OnPinBreedButtonClicked(object sende, RoutedEventArgs e)
        {
            var item = (BreedDataItem)this.flipView.SelectedItem;
            var uri = new Uri(item.TileImagePath.AbsoluteUri);

            var tile = new SecondaryTile(item.UniqueId, item.ShortTitle, item.Title, item.UniqueId, TileOptions.ShowNameOnLogo, uri);

            await tile.RequestCreateAsync();
        }

        private async void OnReminderButtonClicked(object sender, RoutedEventArgs e)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();

            if (notifier.Setting != NotificationSetting.Enabled)
            {
                var dialog = new MessageDialog("Notifications are currently disabled");
                await dialog.ShowAsync();
                return;
            }

            var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var element = template.GetElementsByTagName("text")[0];
            element.AppendChild(template.CreateTextNode("Reminder! Check Friends With Paws"));

            var date = DateTimeOffset.Now.AddSeconds(15);
            var stn = new ScheduledToastNotification(template, date);
            notifier.AddToSchedule(stn);
        }
        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var item = BreedDataSource.GetItem((String)navigationParameter);
            this.DefaultViewModel["Group"] = item.Group;
            this.DefaultViewModel["Items"] = item.Group.Items;
            this.flipView.SelectedItem = item;

            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = (BreedDataItem)this.flipView.SelectedItem;
            pageState["SelectedItem"] = selectedItem.UniqueId;

            DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
        }
    }
}
