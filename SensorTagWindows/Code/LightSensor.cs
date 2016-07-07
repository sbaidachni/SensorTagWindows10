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
    public class LightSensor:SensorBase, INotifyPropertyChanged
    {
        public LightSensor(GattDeviceService service):base(service,SensorsUId.UUID_OPT_CONF,SensorsUId.UUID_OPT_DATA)
        {

        }

        public double LightLevel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected override void NotifyAboutChanges(byte[] data)
        {
            LightLevel = CalculateLight(data);
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LightLevel"));
        }

        private double CalculateLight(byte[] data)
        {
            int mantissa;
            int exponent;
            ushort sfloat = BitConverter.ToUInt16(data, 0);

            mantissa = sfloat & 0x0FFF;
            exponent = (sfloat >> 12) & 0xFF;

            double output;
            double magnitude = Math.Pow(2.0f, exponent);
            output = (mantissa * magnitude);

            return output / 100.0f;
        }
    }
}
