using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ZtPatcher
{
    class PatchItemConverter : BaseConverter, IValueConverter
    {
        public PatchItemConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (KeyValuePair<int, byte[]>)value;

            return string.Format(CultureInfo.InvariantCulture, "{0:X8} : {1}", item.Key, BitConverter.ToString(item.Value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
