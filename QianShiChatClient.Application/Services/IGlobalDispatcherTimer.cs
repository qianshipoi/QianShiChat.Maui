namespace QianShiChatClient.Application.Services;

public interface IGlobalDispatcherTimer
{
    //
    // 摘要:
    //     Gets or sets the amount of time between timer ticks.
    TimeSpan Interval { get; set; }
    //
    // 摘要:
    //     Gets or sets whether the timer should repeat.
    //
    // 言论：
    //     When set the false, the timer will run only once.
    bool IsRepeating { get; set; }
    //
    // 摘要:
    //     Gets a value that indicates whether the timer is running.
    bool IsRunning { get; }

    //
    // 摘要:
    //     Occurs when the timer interval has elapsed.
    event EventHandler Tick;

    //
    // 摘要:
    //     Starts the timer.
    void Start();
    //
    // 摘要:
    //     Stops the timer.
    void Stop();
}
