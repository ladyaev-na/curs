using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace curs.Converters
{
    public class BoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Обрабатываем как bool, так и int
            if (value is bool confirmBool)
            {
                return confirmBool ? "Подтверждено" : "Не подтверждено";
            }
            if (value is int confirmInt)
            {
                return confirmInt == 1 ? "Подтверждено" : "Не подтверждено";
            }
            return "Не подтверждено"; // Значение по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Не требуется для одностороннего преобразования
        }
    }
}