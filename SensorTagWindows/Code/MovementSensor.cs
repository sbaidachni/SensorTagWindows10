using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace SensorTagWindows.Code
{
    public class MovementSensor : SensorBase, INotifyPropertyChanged
    {
        public MovementSensor(GattDeviceService service)
            : base(service, SensorsUId.UUID_MOV_CONF, SensorsUId.UUID_MOV_DATA)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override async Task EnableSensor()
        {
            await EnableSensor(new byte[] { 0x7F, 0x00 });
        }

        public MovementData CalculateAccelerometerData(byte[] rawData)
        {
            MovementData result = new MovementData();
            float SCALE = (float)4096.0;

            short x = (short)(rawData[7] << 8);
            short y = (short)(rawData[9] << 8);
            short z = (short)(rawData[11] << 8);

            result.X = ((x + rawData[6])/SCALE)*-1;
            result.Y = (y + rawData[8])/SCALE;
            result.Z = ((z + rawData[10])/SCALE)*-1;
            return result;
        }
        public MovementData CalculateGyroData(byte[] value)
        {
            MovementData result = new MovementData();
            float SCALE = (float)128.0;

            short x = (short)(value[1] << 8);
            short y = (short)(value[3] << 8);
            short z = (short)(value[5] << 8);

            result.X = (x + value[0])/SCALE;
            result.Y = (y + value[2])/ SCALE;
            result.Z = (z + value[4])/ SCALE;
            return result;
        }
        public MovementData CalculateMagData(byte[] value)
        {
            MovementData result = new MovementData();
            float SCALE = ((float)32768) / 4912;

            short x = (short)(value[13] << 8);
            short y = (short)(value[15] << 8);
            short z = (short)(value[17] << 8);

            result.X = (x+ value[12]) / SCALE;
            result.Y = (y + value[14]) / SCALE;
            result.Z = (z + value[16]) / SCALE;
            return result;
        }


        public class MovementData
        {
            public double X;
            public double Y;
            public double Z;
        }

        public double XAcc { get; set; }
        public double YAcc { get; set; }
        public double ZAcc { get; set; }

        public double XGyr { get; set; }
        public double YGyr { get; set; }
        public double ZGyr { get; set; }

        public double XMag { get; set; }
        public double YMag { get; set; }
        public double ZMag { get; set; }

        protected override void NotifyAboutChanges(byte[] data)
        {
            var coords = CalculateAccelerometerData(data);

            XAcc = coords.X;
            YAcc = coords.Y;
            ZAcc = coords.Z;

            coords = CalculateGyroData(data);

            XGyr = coords.X;
            YGyr = coords.Y;
            ZGyr = coords.Z;

            coords = CalculateMagData(data);

            XMag = coords.X;
            YMag = coords.Y;
            ZMag = coords.Z;

            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("XAcc"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("YAcc"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ZAcc"));

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("XGyr"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("YGyr"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ZGyr"));

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("XMag"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("YMag"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ZMag"));
            }
        }
    }
}
