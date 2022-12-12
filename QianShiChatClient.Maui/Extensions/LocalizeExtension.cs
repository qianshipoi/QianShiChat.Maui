namespace QianShiChatClient.Maui;

[ContentProperty(nameof(Key))]
public class LocalizeExtension : IMarkupExtension<BindingBase>
{
    public string Key { get; set; }
    public IValueConverter Converter { get; set; }

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        return new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{Key}]",
            Source = LocalizationResourceManager.Instance,
            Converter = Converter
        };
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}


//[ContentProperty(nameof(Key))]
//public class LocalizeExtension : IMarkupExtension
//{
//    IStringLocalizer<MyStrings> _localizer;

//    public string Key { get; set; } = string.Empty;

//    public LocalizeExtension()
//    {
//        _localizer = ServiceHelper.GetService<IStringLocalizer<MyStrings>>();
//    }

//    public object ProvideValue(IServiceProvider serviceProvider)
//    {
//        string localizedText = _localizer[Key];
//        return localizedText;
//    }

//    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
//}
