using SensorTagWindows.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace SensorTagWindows.Code
{
    public class HumiditySensor : SensorBase, INotifyPropertyChanged
    {
        public HumiditySensor(GattDeviceService service)
            : base(service, SensorsUId.UUID_HUM_CONF, SensorsUId.UUID_HUM_DATA)
        {

        }

        public double HumidityData
        { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private double CalculateHumidityInPercent(byte[] sensorData)
        {
            int hum = BitConverter.ToUInt16(sensorData, 2);

            hum = hum - (hum % 4);

            return (-6f) + 125f * (hum / 65535f);
        }

        protected override void NotifyAboutChanges(byte[] data)
        {
            HumidityData = CalculateHumidityInPercent(data);
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("HumidityData"));
        }
    }
}
