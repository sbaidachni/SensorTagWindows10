using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SensorTagWindows.Code
{
    public class MagnetometerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = value as MovementSensor;
            return String.Format($"X: {item.XMag:n2} uT, Y: {item.YMag:n2} uT, Z: {item.ZMag:n2} uT");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
