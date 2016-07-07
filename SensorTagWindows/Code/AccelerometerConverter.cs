using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SensorTagWindows.Code
{
    public class AccelerometerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = value as MovementSensor;
            return String.Format($"X: {item.XAcc:n2} G, Y: {item.YAcc:n2} G, Z: {item.ZAcc:n2} G");

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
