namespace QianShiChatClient.Application.Services;

public interface IGlobalDispatcher
{
    //
    // 摘要:
    //     Gets a value that indicates whether dispatching is required for this action.
    bool IsDispatchRequired { get; }

    //
    // 摘要:
    //     Creates a new instance of an Microsoft.Maui.Dispatching.IDispatcherTimer object
    //     associated with this dispatcher.
    IGlobalDispatcherTimer CreateTimer();
    //
    // 摘要:
    //     Schedules the provided action on the UI thread from a worker thread.
    //
    // 参数:
    //   action:
    //     The System.Action to be scheduled for processing on the UI thread.
    //
    // 返回结果:
    //     true when the action has been dispatched successfully, otherwise false.
    bool Dispatch(Action action);
    //
    // 摘要:
    //     Schedules the provided action on the UI thread from a worker thread, taking into
    //     account the provided delay.
    //
    // 参数:
    //   delay:
    //     The delay taken into account before action is dispatched.
    //
    //   action:
    //     The System.Action to be scheduled for processing on the UI thread.
    //
    // 返回结果:
    //     true when the action has been dispatched successfully, otherwise false.
    bool DispatchDelayed(TimeSpan delay, Action action);
}
