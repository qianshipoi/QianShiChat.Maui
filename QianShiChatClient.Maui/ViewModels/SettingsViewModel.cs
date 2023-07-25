namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _currentLang;

    [ObservableProperty]
    private AppTheme _currentAppTheme;

    [ObservableProperty]
    private CultureInfo _currentLanguage;

    public List<string> Langs { get; }

    public List<CultureInfo> Languages { get; }
    public List<AppTheme> AppThemes { get; }

    public SettingsViewModel(
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService)
        : base(navigationService, stringLocalizer)
    {
        AppThemes = new List<AppTheme> { AppTheme.Unspecified, AppTheme.Light, AppTheme.Dark };
        CurrentAppTheme = Settings.Theme;

        Languages = new List<CultureInfo>()
        {
            new CultureInfo("zh-CN"),
            new CultureInfo("en-US"),
        };
        CurrentLanguage = Languages.FirstOrDefault(x => string.Compare(x.Name, Settings.Language, true) == 0);
    }

    partial void OnCurrentAppThemeChanged(AppTheme value)
    {
        App.Current.UserAppTheme = value;
        Settings.Theme = value;
    }

    partial void OnCurrentLanguageChanged(CultureInfo value)
    {
        if (value == null || string.Compare(Settings.Language, value.Name, true) == 0)
        {
            return;
        }
        LocalizationResourceManager.Instance.SetCulture(value);
        Settings.Language = value.Name;
    }
}