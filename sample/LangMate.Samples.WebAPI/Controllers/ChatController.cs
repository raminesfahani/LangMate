using LangMate.Abstractions.Contracts;
using LangMate.Abstractions.Dto;
using LangMate.Samples.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace LangMate.Samples.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController(IAICoreService aiCoreService) : ControllerBase
    {
        private readonly IAICoreService _aiCoreService = aiCoreService;

        [HttpGet("history")]
        public List<ChatMessage> GetChatHistory()
        {
            return _aiCoreService.GetChatHistory();
        }

        [HttpGet("models")]
        public List<string> GetModels()
        {
            return _aiCoreService.GetAvailableModels();
        }

        [HttpPost("completion")]
        public async Task<AIResponse?> ChatAsync([FromBody] GenerateCompletionModel model, CancellationToken cancellationToken)
        {
            return await _aiCoreService.GenerateCompletionAsync(model.Prompt, model.ChatRole, model.NewConversation, cancellationToken);
        }

        [HttpPost("stream_completion")]
        public async Task<string> StreamChatAsync([FromBody] GenerateStreamingCompletionModel model, CancellationToken cancellationToken)
        {
            var response = "";
            await foreach (var item in _aiCoreService.GenerateStreamingCompletionAsync(model.Prompt, model.ChatRole, model.NewConversation, cancellationToken))
            {
                Console.Write(item?.Text);
                response += item?.Text;
            }

            Console.WriteLine();

            return response;
        }
    }
}
