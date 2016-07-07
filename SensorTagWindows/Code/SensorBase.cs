using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using Windows.UI.Core;

namespace SensorTagWindows.Code
{
    public  class SensorBase
    {

        CoreDispatcher dispatcher;

        protected GattDeviceService deviceService;

        private string sensorConfigUuid;
        private string sensorDataUuid;

        private GattCharacteristic dataCharacteristic;

        public SensorBase(GattDeviceService dService, string sensorConfigUuid, string sensorDataUuid)
        {
            this.deviceService = dService;
            this.sensorConfigUuid = sensorConfigUuid;
            this.sensorDataUuid = sensorDataUuid;
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        }

        public string SensorServiceUuid
        {
            get { return deviceService.Uuid.ToString(); }
        }

        protected string SensorConfigUuid
        {
            get { return sensorConfigUuid; }
        }

        protected string SensorDataUuid
        {
            get { return sensorDataUuid; }
        }

        public virtual async Task EnableSensor()
        {
            await EnableSensor(new byte[] { 1 });
        }

        public virtual async Task DisableSensor()
        {
            GattCharacteristic configCharacteristic = deviceService.GetCharacteristics(new Guid(sensorConfigUuid))[0];

            GattCommunicationStatus status = await configCharacteristic.WriteValueAsync((new byte[] { 0 }).AsBuffer());
       }

        public virtual async Task EnableNotifications()
        {
            dataCharacteristic = deviceService.GetCharacteristics(new Guid(sensorDataUuid))[0];

            dataCharacteristic.ValueChanged -= dataCharacteristic_ValueChanged;
            dataCharacteristic.ValueChanged += dataCharacteristic_ValueChanged;

            GattCommunicationStatus status =
                    await dataCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Notify);
        }

        public virtual async Task DisableNotifications()
        {
            dataCharacteristic = deviceService.GetCharacteristics(new Guid(sensorDataUuid))[0];

            dataCharacteristic.ValueChanged -= dataCharacteristic_ValueChanged;

            GattCommunicationStatus status =
                await dataCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                GattClientCharacteristicConfigurationDescriptorValue.None);
        }

        public async Task<byte[]> ReadValue()
        {
            if (dataCharacteristic == null)
                dataCharacteristic = deviceService.GetCharacteristics(new Guid(sensorDataUuid))[0];

            GattReadResult readResult = await dataCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);

            var sensorData = new byte[readResult.Value.Length];

            DataReader.FromBuffer(readResult.Value).ReadBytes(sensorData);

            return sensorData;
        }

        protected async Task EnableSensor(byte[] sensorEnableData)
        {
            try
            {
                GattCharacteristic configCharacteristic = deviceService.GetCharacteristics(new Guid(sensorConfigUuid))[0];

                GattCommunicationStatus status = await configCharacteristic.WriteValueAsync(sensorEnableData.AsBuffer());
            }
            catch (Exception ex)
            {

            }
        }

        private async void dataCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var data = new byte[args
                .CharacteristicValue.Length];

            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);


            await dispatcher.RunAsync(CoreDispatcherPriority.Normal,() =>
            {
                NotifyAboutChanges(data);
            });
        }

        protected virtual void NotifyAboutChanges(byte[] data) { }
    }
}
