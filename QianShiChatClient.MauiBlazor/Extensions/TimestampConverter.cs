using System.Globalization;

namespace QianShiChatClient.MauiBlazor.Extensions;

public class TimestampConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null || value is not long val) return DateTimeOffset.Now;

        return val.ToDateTimeOffset().LocalDateTime;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}