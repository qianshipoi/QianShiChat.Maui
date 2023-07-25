namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class ScanningViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isDetecting = true;

    [ObservableProperty]
    private BarcodeReaderOptions _barcodeReaderOptions = new BarcodeReaderOptions
    {
        Formats = BarcodeFormat.QrCode,
        Multiple = false,
    };

    public ScanningViewModel(
        INavigationService navigationService,
        IStringLocalizer<MyStrings> stringLocalizer)
        : base(navigationService, stringLocalizer)
    {
    }

    [RelayCommand]
    private async Task BarcodeDetection(BarcodeResult[] results)
    {
        if (!IsDetecting) return;
        IsDetecting = false;

        var result = results.First().Value;
        var uri = new Uri(result);

        if (uri.Host == "chat.kuriyama.top" && uri.AbsolutePath == "/api/auth/qr/auth")
        {
            var query = uri.Query.TrimStart('?').Split('&');

            foreach (var item in query)
            {
                var keyVal = item.Split('=');
                if (keyVal.Length == 2 && keyVal[0] == "key")
                {
                    await MainThread.InvokeOnMainThreadAsync(async () => {
                        await _navigationService.GoToQrAuthPage(keyVal[1]);
                    });
                    return;
                }
            }
        }

        using var cancellationTokenSource = new CancellationTokenSource();

        await Snackbar.Make(result, async () => {
            await Clipboard.Default.SetTextAsync(result);
            cancellationTokenSource.Cancel();
        }, "Copy", TimeSpan.FromSeconds(10)).Show();

        try
        {
            await Task.Delay(10000, cancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
        }
        finally
        {
            IsDetecting = true;
        }
    }
}