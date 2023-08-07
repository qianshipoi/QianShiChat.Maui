using CommunityToolkit.Maui.Core;

namespace QianShiChatClient.Core.Services;

public interface IDialogService
{
    Task PopAllAsync(bool animate = true);

    Task PushMessageDialog(bool animate = true);

    Task Snackbar(
        string message,
        Action action = null,
        string actionButtonText = "OK",
        TimeSpan? duration = null,
        SnackbarOptions visualOptions = null,
        IView anchor = null
    );

    Task Toast(
        string message,
        ToastDuration duration = ToastDuration.Short,
        double textSize = 14
    );
}