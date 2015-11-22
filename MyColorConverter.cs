using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace KC12VU_CGoL
{
    class MyColorConverter : IValueConverter
    {
        static BrushConverter bc = new BrushConverter();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string color = (string)value;
            color = color.PadLeft(6, '0');
            color = color.PadLeft(7, '#');
            Brush b = (Brush)bc.ConvertFrom(color);
            return b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
