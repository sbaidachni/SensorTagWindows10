using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace SensorTagWindows.Code
{
    public class PressureSensor : SensorBase, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;


        public PressureSensor(GattDeviceService service)
            : base(service, SensorsUId.UUID_BAR_CONF, SensorsUId.UUID_BAR_DATA)
        {

        }

        public double PressureData
        { get; set; }

        private double CalculatePressure(byte[] sensorData)
        {

            int sfloat = BitConverter.ToInt32(sensorData, 2);
            sfloat = sfloat >> 8;

            return sfloat / 100.0f;
        }

        protected override void NotifyAboutChanges(byte[] data)
        {
            PressureData = CalculatePressure(data);
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("PressureData"));
        }

    }
}
