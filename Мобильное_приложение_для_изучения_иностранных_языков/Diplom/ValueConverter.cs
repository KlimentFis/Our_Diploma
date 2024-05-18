using System;
using Xamarin.Forms;

namespace Diplom
{
    public class NullToPlaceholderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
            {
                return "Не заполнено";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
