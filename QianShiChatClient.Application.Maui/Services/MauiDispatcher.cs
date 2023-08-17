namespace QianShiChatClient.Application.Maui.Services;

public class MauiDispatcher : IGlobalDispatcher
{
    private readonly IDispatcher _dispatcher;

    public MauiDispatcher(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public bool IsDispatchRequired => _dispatcher.IsDispatchRequired;


    public IGlobalDispatcherTimer CreateTimer()
    {
        return new MauiDispatcherTimer(_dispatcher);
    }

    public bool Dispatch(Action action)
    {
        return _dispatcher.Dispatch(action);
    }

    public bool DispatchDelayed(TimeSpan delay, Action action)
    {
        return _dispatcher.DispatchDelayed(delay, action);
    }
}
