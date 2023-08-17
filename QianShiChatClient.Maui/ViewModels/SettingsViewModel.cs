using AppTheme = Microsoft.Maui.ApplicationModel.AppTheme;

namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class SettingsViewModel : ViewModelBase
{
    private readonly Settings _settings;

    [ObservableProperty]
    private string _currentLang;

    [ObservableProperty]
    private AppTheme _currentAppTheme;

    [ObservableProperty]
    private CultureInfo _currentLanguage;

    public List<string> Langs { get; }

    public List<CultureInfo> Languages { get; }
    public List<AppTheme> AppThemes { get; }

    public SettingsViewModel(Settings settings)
    {
        _settings = settings;
        AppThemes = new List<AppTheme> { AppTheme.Unspecified, AppTheme.Light, AppTheme.Dark };
        CurrentAppTheme = _settings.Theme switch { 
            Core.AppTheme.Light => AppTheme.Light,
            Core.AppTheme.Dark => AppTheme.Dark,
            _ => AppTheme.Unspecified,
        };

        Languages = new List<CultureInfo>()
        {
            new CultureInfo("zh-CN"),
            new CultureInfo("en-US"),
        };
        CurrentLanguage = Languages.FirstOrDefault(x => string.Compare(x.Name, _settings.Language, true) == 0);
    }

    partial void OnCurrentAppThemeChanged(AppTheme value)
    {
        App.Current.UserAppTheme = value;
        _settings.Theme = value switch
        {
            AppTheme.Light => Core.AppTheme.Light,
            AppTheme.Dark => Core.AppTheme.Dark,
            _ => Core.AppTheme.Unspecified,
        };
    }

    partial void OnCurrentLanguageChanged(CultureInfo value)
    {
        if (value == null || string.Compare(_settings.Language, value.Name, true) == 0)
        {
            return;
        }
        LocalizationResourceManager.Instance.SetCulture(value);
        _settings.Language = value.Name;
    }
}