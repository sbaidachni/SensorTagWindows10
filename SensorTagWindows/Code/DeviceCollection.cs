using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace SensorTagWindows.Code
{
    public class DeviceCollection
    {
        public ObservableCollection<DeviceInformation> Items { get; set; }

        public DeviceCollection()
        {
            Items = new ObservableCollection<DeviceInformation>();
        }
    }
}
