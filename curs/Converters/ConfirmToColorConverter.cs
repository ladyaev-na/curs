using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace curs.Converters
{
    public class ConfirmToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Обрабатываем как bool, так и int
            if (value is bool confirmBool)
            {
                return confirmBool ? Color.FromArgb("#4CAF50") : Color.FromArgb("#FF0000");
            }
            if (value is int confirmInt)
            {
                return confirmInt == 1 ? Color.FromArgb("#4CAF50") : Color.FromArgb("#FF0000");
            }
            return Color.FromArgb("#666666"); // Цвет по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Не требуется для одностороннего преобразования
        }
    }
}