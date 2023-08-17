namespace QianShiChatClient.Application.Helpers;

public class Settings
{
    IPreferencesService Preferences;

    public Settings(IPreferencesService preferences)
    {
        Preferences = preferences;
    }

    public AppTheme Theme
    {
        get
        {
            if (!Preferences.ContainsKey(nameof(Theme)))
                return AppTheme.Unspecified;
            var appThemeStr = Preferences.Get(nameof(Theme), AppTheme.Unspecified.ToString());
            return Enum.Parse<AppTheme>(appThemeStr!);
        }
        set => Preferences.Set(nameof(Theme), value.ToString());
    }

    public string Language
    {
        get
        {
            if (!Preferences.ContainsKey(nameof(Language)))
                return Thread.CurrentThread.CurrentCulture.Name;
            return Preferences.Get(nameof(Language), Thread.CurrentThread.CurrentCulture.Name);
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Preferences.Remove(nameof(Language));
                return;
            }
            Preferences.Set(nameof(Language), value);
        }
    }

    public string? AccessToken
    {
        get
        {
            if (!Preferences.ContainsKey(nameof(AccessToken)))
                return null;
            return Preferences.Get(nameof(AccessToken), string.Empty);
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Preferences.Remove(nameof(AccessToken));
                return;
            }
            Preferences.Set(nameof(AccessToken), value);
        }
    }

    public UserInfoModel? CurrentUser
    {
        get
        {
            if (!Preferences.ContainsKey(nameof(CurrentUser)))
                return null;

            var value = Preferences.Get(nameof(CurrentUser), string.Empty);
            if (string.IsNullOrWhiteSpace(value)) return null;

            return JsonSerializer.Deserialize<UserInfoModel>(value);
        }
        set
        {
            if (value == null)
            {
                Preferences.Remove(nameof(CurrentUser));
                return;
            }
            Preferences.Set(nameof(CurrentUser), JsonSerializer.Serialize(value));
        }
    }
}