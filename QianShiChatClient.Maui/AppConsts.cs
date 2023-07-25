namespace QianShiChatClient.Maui;

public sealed class AppConsts
{
    public const string DatabaseFilename = "ChatSQLite.db3";

    public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    public const string API_CLIENT_NAME = "ChatApi";

    public static string CLIENT_TYPE = $"MauiClient{DeviceInfo.Platform}{DeviceInfo.Idiom}";

    public static string API_BASE_URL = DeviceInfo.Platform == DevicePlatform.Android ?
            "https://chat.kuriyama.top" :
            "https://chat.kuriyama.top";

    public static bool IsDesktop => DeviceInfo.Idiom == DeviceIdiom.Desktop;
}