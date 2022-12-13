using ZXing.Net.Maui;

namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class ScanningViewModel : ViewModelBase
{
    [ObservableProperty]
    bool _isDetecting = true;

    public ScanningViewModel(
        INavigationService navigationService,
        IStringLocalizer<MyStrings> stringLocalizer)
        : base(navigationService, stringLocalizer)
    {
    }

    [RelayCommand]
    async Task BarcodeDetection(BarcodeResult[] results)
    {
        if (!IsDetecting) return;
        IsDetecting = false;
        var uri = new Uri(results.First().Value);

        if (uri.Host == "chat.kuriyama.top" && uri.AbsolutePath == "/api/auth/qr/auth")
        {
            var query = uri.Query.TrimStart('?').Split('&');

            foreach (var item in query)
            {
                var keyVal = item.Split('=');
                if (keyVal.Length == 2 && keyVal[0] == "key")
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await _navigationService.GoToQrAuthPage(keyVal[1]);
                    });
                    return;
                }
            }

        }
        await Snackbar.Make(results.First().Value).Show();
    }
}
