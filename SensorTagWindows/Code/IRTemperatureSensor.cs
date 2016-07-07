using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace SensorTagWindows.Code
{
    public class IRTemperatureSensor : SensorBase, INotifyPropertyChanged
    {
        public IRTemperatureSensor(GattDeviceService service)
            : base( service,SensorsUId.UUID_IRT_CONF, SensorsUId.UUID_IRT_DATA)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double CalculateAmbientTemperature(byte[] sensorData)
        {
            return BitConverter.ToUInt16(sensorData, 2) / 128.0;
        }

        public double AmbientTemperature { get; set; }

        public double CalculateTargetTemperature(byte[] sensorData)
        {
            return BitConverter.ToUInt16(sensorData, 0) / 128.0;
            //return CalculateTargetTemperature(sensorData, (BitConverter.ToUInt16(sensorData, 2) / 128.0));
        }

        public double TargetTemperature { get; set; }


        private double CalculateTargetTemperature(byte[] sensorData, double ambientTemperature)
        {
            double Vobj2 = BitConverter.ToInt16(sensorData, 0);
            Vobj2 *= 0.00000015625;

            double Tdie = ambientTemperature + 273.15;

            double S0 = 5.593E-14;
            double a1 = 1.75E-3;
            double a2 = -1.678E-5;
            double b0 = -2.94E-5;
            double b1 = -5.7E-7;
            double b2 = 4.63E-9;
            double c2 = 13.4;
            double Tref = 298.15;
            double S = S0 * (1 + a1 * (Tdie - Tref) + a2 * Math.Pow((Tdie - Tref), 2));
            double Vos = b0 + b1 * (Tdie - Tref) + b2 * Math.Pow((Tdie - Tref), 2);
            double fObj = (Vobj2 - Vos) + c2 * Math.Pow((Vobj2 - Vos), 2);
            double tObj = Math.Pow(Math.Pow(Tdie, 4) + (fObj / S), .25);

            return tObj - 273.15;
        }

        protected override void NotifyAboutChanges(byte[] data)
        {
            AmbientTemperature = CalculateAmbientTemperature(data);
            TargetTemperature = CalculateTargetTemperature(data);
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("AmbientTemperature"));
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TargetTemperature"));
        }
    }
}
