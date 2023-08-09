namespace QianShiChatClient.Core;

public enum ChatMessageType : sbyte
{
    /// <summary> 文字 </summary>
    Text = 1,

    /// <summary> 图片 </summary>
    Image,

    /// <summary> 视频 </summary>
    Video,

    /// <summary> 其他文件 </summary>
    OtherFile
}
