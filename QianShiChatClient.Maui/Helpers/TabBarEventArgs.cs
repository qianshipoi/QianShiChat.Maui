namespace QianShiChatClient.Maui.Helpers;

public class TabBarEventArgs : EventArgs
{
    public PageType CurrentPage { get; private set; }

    public TabBarEventArgs(PageType currentPage)
    {
        CurrentPage = currentPage;
    }
}
public enum PageType
{
    DevoirsPage, AgendaPage, NotesPage, MessagesPage, AbsencesPage
}
