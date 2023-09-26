namespace QianShiChatClient.Maui.Helpers;

public class MessageModelSelector : DataTemplateSelector
{
    public DataTemplate OtherTextMessageDataTemplate { get; set; }
    public DataTemplate SelfTextMessageDataTemplate { get; set; }
    public DataTemplate SelfFileMessageDataTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is not MessageModel message)
        {
            throw new Exception("item should be ChatMessageModel.");
        }

        if (message.MessageType == ChatMessageType.Text)
        {
            return message.IsSelfSend ? SelfTextMessageDataTemplate : OtherTextMessageDataTemplate;
        }

        return message.IsSelfSend ? SelfFileMessageDataTemplate : throw new NotSupportedException();
    }
}
