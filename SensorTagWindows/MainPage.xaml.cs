using SensorTagWindows.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SensorTagWindows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DeviceCollection deviceList = new DeviceCollection();
        DeviceWatcher deviceWatcher;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            deviceList.Items.Clear();
            deviceListView.ItemsSource = deviceList.Items;

            deviceWatcher = DeviceInformation.CreateWatcher(
                "System.ItemNameDisplay:~~\"SensorTag\"",
                new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" },
                DeviceInformationKind.AssociationEndpoint);

            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.Start();

            base.OnNavigatedTo(e);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            deviceWatcher.Stop();
            base.OnNavigatedFrom(e);
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () =>
            {
                var el = (from a in deviceList.Items where a.Id.Equals(args.Id) select a).FirstOrDefault();
                if (el!=null)
                    deviceList.Items.Remove(el);
            });
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    deviceList.Items.Add(args);
                });
        }

        private async void deviceListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as DeviceInformation;
            if (item.Pairing.CanPair)
            {
                var result=await item.Pairing.PairAsync();
                if ((result.Status==DevicePairingResultStatus.Paired)||(result.Status == DevicePairingResultStatus.AlreadyPaired))
                {
                    this.Frame.Navigate(typeof(DevicePage),item);
                }
            }
            else if (item.Pairing.IsPaired==true)
            {
                this.Frame.Navigate(typeof(DevicePage), item);
            }
        }
    }
}
