namespace QianShiChatClient.Maui.Views;

public partial class QrAuthPage : ContentPage
{
    string _authPath;
    string _key;

    public QrAuthPage(string authPath)
    {
        InitializeComponent();
        _authPath = authPath;
        _authPath.IndexOf("key=");
        var query = _authPath.Substring(_authPath.IndexOf("key="));
        query = query.Substring(4);
        var index = query.IndexOf("&");
        if (index != -1) query = query.Substring(0, index);
        _key = query;
        AuthPathControl.Text = _authPath;
        KeyControl.Text = _key;
        _ = PreAuth();
    }

    async Task PreAuth()
    {
        try
        {
            var client = ServiceHelper.Current.GetRequiredService<IApiClient>();
            var response = await client.QrPreAuthAsync(_key);
            if (response.Code != 200)
            {
                await Snackbar.Make($"Ô¤ÊÚÈ¨Ê§°Ü£º{response.Message}").Show();
            }
        }
        catch (Exception ex)
        {
            await Snackbar.Make($"Ô¤ÊÚÈ¨Ê§°Ü£º{ex.Message}").Show();
        }
    }

    async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            var client = ServiceHelper.Current.GetRequiredService<IApiClient>();
            var response = await client.QrAuthAsync(_key);
            if (response.Code == 200)
            {
                await Shell.Current.Navigation.PopModalAsync();
                await Shell.Current.Navigation.PopToRootAsync();
                await Toast.Make("ÊÚÈ¨³É¹¦").Show();
                return;
            }
            await Snackbar.Make($"ÊÚÈ¨Ê§°Ü£º{response.Message}").Show();
        }
        catch (Exception ex)
        {
            await Snackbar.Make($"ÊÚÈ¨Ê§°Ü£º{ex.Message}").Show();
        }
    }

    async void Button_Clicked_1(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopModalAsync();
    }
}