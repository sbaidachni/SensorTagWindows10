using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace SensorTagWindows.Code
{
    public class KeysSensor : SensorBase, INotifyPropertyChanged
    {
        public KeysSensor(GattDeviceService service)
            : base(service, null, SensorsUId.UUID_KEY_DATA)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool LeftKeyHit(byte[] sensorData)
        {
            return (new BitArray(sensorData))[1];
        }

        public bool RightKeyHit(byte[] sensorData)
        {
            return (new BitArray(sensorData))[0];
        }

        public bool IsRight {get;set;}

        public bool IsLeft { get; set; }

        protected override void NotifyAboutChanges(byte[] data)
        {
            IsRight = RightKeyHit(data);

            IsLeft = LeftKeyHit(data);
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsRight"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsLeft"));
            }
        }
    }
}
