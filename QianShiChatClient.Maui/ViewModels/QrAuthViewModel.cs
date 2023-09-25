using QianShiChatClient.Application.IServices;

namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class QrAuthViewModel : ViewModelBase, IQueryAttributable
{
    private readonly IApiClient _apiClient;

    [ObservableProperty]
    private bool _preAuthSuccessed;

    [ObservableProperty]
    private string _key;

    public QrAuthViewModel(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Key = query[nameof(Key)].ToString();
        await PreAuth();
    }

    private async Task PreAuth()
    {
        try
        {
            var response = await _apiClient.QrPreAuthAsync(Key);
            if (response.Code == 200)
            {
                PreAuthSuccessed = true;
                return;
            }

            await Snackbar.Make($"预授权失败：{response.Message}").Show();
        }
        catch (Exception ex)
        {
            await Snackbar.Make($"预授权失败：{ex.Message}").Show();
        }
    }

    [RelayCommand]
    private async Task Pass()
    {
        try
        {
            var response = await _apiClient.QrAuthAsync(Key);
            if (response.Code == 200)
            {
                await Toast.Make("授权成功").Show();
                await _navigationService.GoToRoot();
                return;
            }
            await Snackbar.Make($"授权失败：{response.Message}").Show();
        }
        catch (Exception ex)
        {
            await Snackbar.Make($"授权失败：{ex.Message}").Show();
        }
    }

    [RelayCommand]
    private Task Reject() => _navigationService.GoToMessagePage();
}