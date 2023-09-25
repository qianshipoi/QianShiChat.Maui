using QianShiChatClient.Application.IServices;

namespace QianShiChatClient.Maui.Services;

public class DialogService : IDialogService
{
    public Task Toast(
        string message,
        ToastDuration duration = ToastDuration.Short,
        double textSize = 14
    ) => CommunityToolkit.Maui.Alerts.Toast.Make(message, duration switch
    {
        ToastDuration.Short => CommunityToolkit.Maui.Core.ToastDuration.Short,
        ToastDuration.Long => CommunityToolkit.Maui.Core.ToastDuration.Long,
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

    private IPopupNavigation Service => MopupService.Instance;

    public Task PushMessageDialog(bool animate = true) =>
        Service.PushAsync(new MessageDialog(), animate);

    public Task PopAllAsync(bool animate = true) => Service.PopAllAsync(animate);
}