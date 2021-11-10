using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Twoxzi.RemoteManager
{
    public class DateTimeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DateTime)
            {
               return ((DateTime)value).ToString(culture.DateTimeFormat);
            }
            //throw new NotImplementedException();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            var str = value as String;
            if(str!=null && targetType == typeof(DateTime))
            {
                return DateTime.Parse(str);
            }
            return value;
        }
    }
}
