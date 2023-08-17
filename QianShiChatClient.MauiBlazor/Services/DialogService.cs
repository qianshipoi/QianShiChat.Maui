namespace QianShiChatClient.MauiBlazor.Services;

public class DialogService : IDialogService
{
    public Task Toast(
        string message,
        Core.ToastDuration duration = Core.ToastDuration.Short,
        double textSize = 14
    ) => CommunityToolkit.Maui.Alerts.Toast.Make(message, duration switch
    {
        Core.ToastDuration.Short => CommunityToolkit.Maui.Core.ToastDuration.Short,
        Core.ToastDuration.Long => CommunityToolkit.Maui.Core.ToastDuration.Long,
        _ => CommunityToolkit.Maui.Core.ToastDuration.Short,
    }, textSize).Show();

    public Task Snackbar(
        string message,
        Action action = null,
        string actionButtonText = "OK",
        TimeSpan? duration = null
    ) =>
        CommunityToolkit.Maui.Alerts.Snackbar
            .Make(message, action, actionButtonText, duration)
            .Show();

    public Task PushMessageDialog(bool animate = true) => throw new NotSupportedException();

    public Task PopAllAsync(bool animate = true) => throw new NotSupportedException();
}