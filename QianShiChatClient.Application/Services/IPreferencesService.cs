namespace QianShiChatClient.Application.Services;

public interface IPreferencesService
{
    bool ContainsKey(string key);

    void Set(string key, string value);

    string Get(string key, string defaultValue);

    void Remove(string key);
}
