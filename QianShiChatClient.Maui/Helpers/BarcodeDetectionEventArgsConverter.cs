namespace QianShiChatClient.Maui.Helpers;

public class BarcodeDetectionEventArgsConverter : BaseConverterOneWay<BarcodeDetectionEventArgs, object>
{
    public override object DefaultConvertReturnValue { get; set; } = null;

    [return: NotNullIfNotNull(nameof(value))]
    public override object ConvertFrom(BarcodeDetectionEventArgs value, CultureInfo culture) => value switch
    {
        null => null,
        _ => value.Results
    };
}