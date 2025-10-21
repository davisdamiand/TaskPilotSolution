using System.Globalization;
using Microsoft.Maui.Controls;

namespace TaskPilot.Client.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string priority)
            {
                switch (priority.ToLower())
                {
                    case "high":
                        return Application.Current.Resources["HighPriorityColor"] as Color;
                    case "medium":
                        return Application.Current.Resources["MediumPriorityColor"] as Color;
                    case "low":
                        return Application.Current.Resources["LowPriorityColor"] as Color;
                }
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}