using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using SensorTagWindows.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SensorTagWindows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DevicePage : Page
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "";
        static string deviceKey = "";

        DispatcherTimer timer = new DispatcherTimer();
        int sensorCount = 0;

        public DevicePage()
        {
            this.InitializeComponent();
            
        }

        DeviceInformation device;
        BluetoothLEDevice leDevice;
        DeviceWatcher watcher;
        GattDeviceCollection devices;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += DevicePage_BackRequested;
            device = e.Parameter as DeviceInformation;
            devices = new GattDeviceCollection(device);
            
            leDevice = await BluetoothLEDevice.FromIdAsync(device.Id);
            string selector = "(System.DeviceInterface.Bluetooth.DeviceAddress:=\"" + leDevice.BluetoothAddress.ToString("X") + "\")";

            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("SensorTag", deviceKey));

            watcher = DeviceInformation.CreateWatcher(selector);
            watcher.Added += Watcher_Added;
            watcher.Removed += Watcher_Removed;
            watcher.Start();

            base.OnNavigatedTo(e);
        }

        private void Watcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            //throw new NotImplementedException();
        }

        private async void Watcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
            {
                try
                {
                    var dev = await GattDeviceService.FromIdAsync(args.Id);
                    if (dev != null)
                    {
                        switch (dev.Uuid.ToString())
                        {
                            case SensorsUId.UUID_HUM_SERV:
                                devices.Humidity = new HumiditySensor(dev);
                                await devices.Humidity.EnableSensor();
                                await devices.Humidity.EnableNotifications();
                                //hGrid.DataContext = devices.Humidity;
                                sensorCount++;
                                break;
                            case SensorsUId.UUID_BAR_SERV:
                                devices.Pressure = new PressureSensor(dev);
                                await devices.Pressure.EnableSensor();
                                 
                                await devices.Pressure.EnableNotifications();
                                //pGrid.DataContext = devices.Pressure;
                                sensorCount++;
                                break;
                            case SensorsUId.UUID_KEY_SERV:
                                devices.Keys = new KeysSensor(dev);
                                await devices.Keys.EnableNotifications();
                                //keysGrid.DataContext = devices.Keys;
                                sensorCount++;
                                break;
                            case SensorsUId.UUID_MOV_SERV:
                                devices.Movement = new MovementSensor(dev);
                                await devices.Movement.EnableSensor();
                                await devices.Movement.EnableNotifications();
                                sensorCount++;
                                break;
                            case SensorsUId.UUID_IRT_SERV:
                                devices.IRTemperature = new IRTemperatureSensor(dev);
                                await devices.IRTemperature.EnableSensor();
                                await devices.IRTemperature.EnableNotifications();
                                sensorCount++;
                                break;
                            case SensorsUId.UUID_INF_SERV:
                                devices.Info = new DeviceInfo(dev);
                                await devices.Info.Initialize();
                                sensorCount++;
                                break;
                            case SensorsUId.UUID_OPT_SERV:
                                devices.Light = new LightSensor(dev);
                                await devices.Light.EnableSensor();
                                await devices.Light.EnableNotifications();
                                sensorCount++;
                                break;
                        }
                        if (sensorCount == 7)
                        {
                            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () =>
                            {
                                progress.Visibility = Visibility.Collapsed;
                                this.DataContext = devices;
                            });
                            timer.Interval = new TimeSpan(0, 0, 5);
                            timer.Tick += Timer_Tick;
                            //timer.Start();
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            });
            
        }

        private async void Timer_Tick(object sender, object e)
        {
            IoTMessage message = new IoTMessage()
            {
                humidity = String.Format($"{devices.Humidity.HumidityData:n2}"),
                temperature = String.Format($"{devices.IRTemperature.AmbientTemperature:n2}"),
                pressure = String.Format($"{devices.Pressure.PressureData:n2}"),
                time = DateTime.Now.ToString()
            };

            var messageString = JsonConvert.SerializeObject(message);
            var messageOut = new Message(Encoding.ASCII.GetBytes(messageString));

            try
            {
                await deviceClient.SendEventAsync(messageOut);
            }
            catch(Exception ex)
            {

            }
        }

        private void DevicePage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            watcher.Stop();
            timer.Stop();
            base.OnNavigatedFrom(e);
        }
    }
}
