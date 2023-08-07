using CommunityToolkit.Maui.Core;

namespace QianShiChatClient.MauiBlazor.Services;

public class DialogService : IDialogService
{
    public Task Toast(
        string message,
        ToastDuration duration = ToastDuration.Short,
        double textSize = 14
    ) => CommunityToolkit.Maui.Alerts.Toast.Make(message, duration, textSize).Show();

    public Task Snackbar(
        string message,
        Action action = null,
        string actionButtonText = "OK",
        TimeSpan? duration = null,
        SnackbarOptions visualOptions = null,
        IView anchor = null
    ) =>
        CommunityToolkit.Maui.Alerts.Snackbar
            .Make(message, action, actionButtonText, duration, visualOptions, anchor)
            .Show();

    public Task PushMessageDialog(bool animate = true) => throw new NotSupportedException();

    public Task PopAllAsync(bool animate = true) => throw new NotSupportedException();
}