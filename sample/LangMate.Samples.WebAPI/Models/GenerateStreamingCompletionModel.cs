using Microsoft.Extensions.AI;

namespace LangMate.Samples.WebAPI.Models
{
    public class GenerateStreamingCompletionModel
    {
        public required string Prompt { get; set; }
        public ChatRole ChatRole { get; set; } = ChatRole.User;
        public bool NewConversation { get; set; } = false;
    }
}
