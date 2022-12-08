namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class SettingsViewModel : ViewModelBase
{
    readonly ChatHub _chatHub;
    readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    string _currentLang;
    [ObservableProperty]
    AppTheme _currentAppTheme;

    public ObservableCollection<string> Langs { get; }
    public ObservableCollection<AppTheme> AppThemes { get; }

    public SettingsViewModel(
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService,
        ChatHub chatHub,
        IServiceProvider serviceProvider)
        : base(navigationService, stringLocalizer)
    {
        Title = "设置";
        AppThemes = new ObservableCollection<AppTheme> { AppTheme.Unspecified, AppTheme.Light, AppTheme.Dark };
        CurrentAppTheme = App.Current.RequestedTheme;

        Langs = new ObservableCollection<string>() { "English", "简体中文" };
        _chatHub = chatHub;
        _serviceProvider = serviceProvider;
        //_currentLang = string.Compare(Settings.Culture.Name, "zh-CN", true) == 0 ? "简体中文" : "English";
    }

    partial void OnCurrentAppThemeChanged(AppTheme value)
    {
        App.Current.UserAppTheme = value;
    }

    partial void OnCurrentLangChanged(string value)
    {
        var culture = new CultureInfo(value == "English" ? "en-US" : "zh-CN");

        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Settings.Culture = culture;

        App.Current.MainPage = _serviceProvider.GetRequiredService<AppShell>();
        _navigationService.GoToSettingsPage();
    }
}
