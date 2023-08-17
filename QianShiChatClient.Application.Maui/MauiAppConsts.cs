namespace QianShiChatClient.Application.Maui;

public sealed class MauiAppConsts
{
    public const string DatabaseFilename = "ChatSQLite.db3";

    public static readonly string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    public static readonly string CLIENT_TYPE = $"MauiClient{DeviceInfo.Platform}{DeviceInfo.Idiom}";

    public static readonly string API_BASE_URL = DeviceInfo.Platform == DevicePlatform.Android ?
            "https://chat.kuriyama.top" :
            "https://chat.kuriyama.top";

    public static bool IsDesktop => DeviceInfo.Idiom == DeviceIdiom.Desktop;
}