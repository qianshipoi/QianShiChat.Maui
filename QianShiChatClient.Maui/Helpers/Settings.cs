namespace QianShiChatClient.Maui.Helpers;

public static class Settings
{
    public static AppTheme Theme
    {
        get
        {
            if (!Preferences.ContainsKey(nameof(Theme)))
                return AppTheme.Light;

            return Enum.Parse<AppTheme>(Preferences.Get(nameof(Theme), Enum.GetName(AppTheme.Light)));
        }
        set => Preferences.Set(nameof(Theme), value.ToString());
    }

    public static CultureInfo Culture
    {
        get
        {
            if (!Preferences.ContainsKey(nameof(Culture)))
                return Thread.CurrentThread.CurrentCulture;

            return new CultureInfo(Preferences.Get(nameof(Culture), Thread.CurrentThread.CurrentCulture.Name));
        }
        set => Preferences.Set(nameof(Culture), value.Name);
    }

    public static string AccessToken
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

    public static UserInfo CurrentUser
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
