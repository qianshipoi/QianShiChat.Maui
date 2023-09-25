using QianShiChatClient.Application.IServices;

namespace QianShiChatClient.Application.Maui.Services;

public class MauiDispatcherTimer : IGlobalDispatcherTimer
{
    private readonly IDispatcherTimer _timer;

    public MauiDispatcherTimer(IDispatcher dispatcher)
    {
        _timer = dispatcher.CreateTimer();
        _timer.Tick += Tick;
    }

    public TimeSpan Interval { get => _timer.Interval; set => _timer.Interval = value; }
    public bool IsRepeating { get => _timer.IsRepeating; set => _timer.IsRepeating = value; }

    public bool IsRunning => _timer.IsRunning;

    public event EventHandler Tick = default!;

    public void Start()
    {
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }
}
