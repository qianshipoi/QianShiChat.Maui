namespace QianShiChatClient.Core;

public enum AppTheme
{
    //
    // 摘要:
    //     Default, unknown or unspecified theme.
    Unspecified = 0,
    //
    // 摘要:
    //     Light theme.
    Light = 1,
    //
    // 摘要:
    //     Dark theme.
    Dark = 2
}

/// <summary>
/// Toast duration
/// </summary>
public enum ToastDuration
{
    /// <summary>
    /// Displays Toast for a short time (~2 seconds).
    /// </summary>
    Short,

    /// <summary>
    /// Displays Toast for a long time (~3.5 seconds).
    /// </summary>
    Long
}