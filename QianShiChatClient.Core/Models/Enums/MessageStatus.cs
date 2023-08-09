namespace QianShiChatClient.Core;

public enum MessageStatus : sbyte
{
    /// <summary>
    /// 发送中
    /// </summary>
    Sending,
    /// <summary>
    /// 发送成功
    /// </summary>
    Successful,
    /// <summary>
    /// 发送失败
    /// </summary>
    Failed
}
