using System.Globalization;
using Microsoft.Maui.Controls;

namespace TaskPilot.Client.Converters
{
    public class PriorityToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string priority)
            {
                switch (priority.ToLower())
                {
                    case "high":
                        return Application.Current.Resources["HighPriorityTextColor"] as Color;
                    case "medium":
                        return Application.Current.Resources["MediumPriorityTextColor"] as Color;
                    case "low":
                        return Application.Current.Resources["LowPriorityTextColor"] as Color;
                }
            }
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}