using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace SensorTagWindows.Code
{
    public class GattDeviceCollection: INotifyPropertyChanged
    {
        DeviceInformation device;

        public event PropertyChangedEventHandler PropertyChanged;

        public GattDeviceCollection(DeviceInformation device)
        {
            this.device = device;
        }

        private HumiditySensor humidity;
        public HumiditySensor Humidity {
            get { return humidity;  }
            set { humidity = value; humidity.PropertyChanged += Humidity_PropertyChanged; }
        }

        private void Humidity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Humidity"));
        }

        private PressureSensor pressure;
        public PressureSensor Pressure {
            get { return pressure; } set { pressure = value; pressure.PropertyChanged += Pressure_PropertyChanged; } }

        private void Pressure_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Pressure"));
        }

        private KeysSensor keys;
        public KeysSensor Keys {
            get { return keys; }
            set { keys = value; keys.PropertyChanged += Keys_PropertyChanged; }
        }

        private void Keys_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Keys"));
        }

        private MovementSensor movement;
        public MovementSensor Movement {
            get { return movement; }
            set { movement = value; movement.PropertyChanged += Movement_PropertyChanged; }
        }

        private void Movement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Movement"));
        }

        private IRTemperatureSensor irTemperature;
        public IRTemperatureSensor IRTemperature {
            set { irTemperature = value; irTemperature.PropertyChanged += IrTemperature_PropertyChanged; }
            get { return irTemperature; }
        }

        private void IrTemperature_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IRTemperature"));
        }

        public DeviceInfo Info { get; set; }

        private LightSensor light;
        public LightSensor Light {
            get { return light; }
            set { light = value; light.PropertyChanged += Light_PropertyChanged; }
        }

        private void Light_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Light"));
        }

        public string DeviceID
        {
            get
            {
                return device.Id;
            }
        }
    }
}
