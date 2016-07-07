using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SensorTagWindows.Code
{
    public class GyroscopeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = value as MovementSensor;
            return String.Format($"X: {item.XGyr:n2} {'\u00b0'}/S, Y: {item.YGyr:n2} {'\u00b0'}/S, Z: {item.ZGyr:n2} {'\u00b0'}/S");

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
