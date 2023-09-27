using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace GameOfLife
{
    class MyColorConverter : IValueConverter
    {
        static BrushConverter bc = new BrushConverter();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string color = (string)value;
            color = color.PadLeft(6, '0');
            color = color.PadLeft(7, '#');
            Brush b = bc.ConvertFrom(color) as Brush;
            return b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
