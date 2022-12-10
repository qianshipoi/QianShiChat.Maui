namespace QianShiChatClient.Maui.Helpers
{
    public class ChatMessageSelector : DataTemplateSelector
    {
        public DataTemplate OtherTextMessageDataTemplate { get; set; }
        public DataTemplate SelfTextMessageDataTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if(item is not ChatMessage message)
            {
                throw new Exception("item should be ChatMessage.");
            }

            return message.IsSelfSend ? SelfTextMessageDataTemplate : OtherTextMessageDataTemplate;
        }
    }
}
