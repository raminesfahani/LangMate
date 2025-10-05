namespace LangMate.AppHost.ApiService.Models
{
    public class StartNewChatResponse(string message, string conversationId)
    {
        public string Message { get; } = message;
        public string ConversationId { get; } = conversationId;

        public override bool Equals(object? obj)
        {
            return obj is StartNewChatResponse other &&
                   Message == other.Message &&
                   ConversationId == other.ConversationId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Message, ConversationId);
        }
    }
}
