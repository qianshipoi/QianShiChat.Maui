namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class SettingsViewModel : ViewModelBase
{
    readonly ChatHub _chatHub;
    readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    string _currentLang;
    [ObservableProperty]
    AppTheme _currentAppTheme;
    [ObservableProperty]
    CultureInfo _currentLanguage;

    public List<string> Langs { get; }

    public List<CultureInfo> Languages { get; }
    public List<AppTheme> AppThemes { get; }

    public SettingsViewModel(
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService,
        ChatHub chatHub,
        IServiceProvider serviceProvider)
        : base(navigationService, stringLocalizer)
    {
        Title = "设置";
        _chatHub = chatHub;
        _serviceProvider = serviceProvider;

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

        Thread.CurrentThread.CurrentCulture = value;
        Thread.CurrentThread.CurrentUICulture = value;
        CultureInfo.DefaultThreadCurrentCulture = value;
        CultureInfo.DefaultThreadCurrentUICulture = value;
        Settings.Language = value.Name;

        App.Current.MainPage = _serviceProvider.GetRequiredService<AppShell>();
        _navigationService.GoToSettingsPage();
    }
}
