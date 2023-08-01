namespace QianShiChatClient.Maui.Helpers;

public class PathToFilenameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is not string path || string.IsNullOrWhiteSpace(path))
        {
            return BindableProperty.UnsetValue;
        }

        return Path.GetFileName(path);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
