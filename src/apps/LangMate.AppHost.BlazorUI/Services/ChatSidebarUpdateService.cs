namespace LangMate.AppHost.BlazorUI.Services
{
    public class ChatSidebarUpdateService
    {
        public event Func<Task>? OnChatListRefresh;

        public async Task RaiseChatListRefresh()
        {
            if (OnChatListRefresh is not null)
            {
                await OnChatListRefresh.Invoke();
            }
        }
    }

}