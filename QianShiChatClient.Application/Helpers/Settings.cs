namespace QianShiChatClient.Application.Helpers;

public static class Settings
{
    public static AppTheme Theme
    {
        get
        {
            if (!Preferences.ContainsKey(nameof(Theme)))
                return AppTheme.Unspecified;
            var appThemeStr = Preferences.Get(nameof(Theme), Enum.GetName(AppTheme.Unspecified));
            return Enum.Parse<AppTheme>(appThemeStr!);
        }
        set => Preferences.Set(nameof(Theme), value.ToString());
    }

    public static string Language
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

    public static string? AccessToken
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

    public static UserInfo? CurrentUser
    {
        get
        {
            if (!Preferences.ContainsKey(nameof(CurrentUser)))
                return null;

            var value = Preferences.Get(nameof(CurrentUser), string.Empty);
            if (string.IsNullOrWhiteSpace(value)) return null;

            return JsonSerializer.Deserialize<UserInfo>(value);
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