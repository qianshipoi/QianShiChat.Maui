using ZXing.Net.Maui;

namespace QianShiChatClient.Maui.Views;

public partial class ScanningPage : ContentPage
{
    public ScanningPage()
    {
        InitializeComponent();
        cameraBarcodeReaderView.Options = new BarcodeReaderOptions()
        {
            Formats = BarcodeFormat.QrCode,
            Multiple = false,
        };
    }

    bool _isIdentify = false;

    private void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (!_isIdentify)
        {
            _isIdentify = true;
            Dispatcher.Dispatch(async () =>
            {
                await Shell.Current.Navigation.PushModalAsync(new QrAuthPage(e.Results[0].Value));
            });
        }
    }
}