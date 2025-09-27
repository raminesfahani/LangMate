using LangMate.Abstractions.Contracts;
using LangMate.Core.Providers;
using LangMate.Persistence.NoSQL.MongoDB.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ollama;
using System;
using System.ComponentModel.DataAnnotations;

namespace LangMate.AppHost.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController(ILogger<ChatController> logger, IOllamaFactoryProvider ollamaFactoryProvider) : ControllerBase
    {
        private readonly ILogger<ChatController> _logger = logger;
        private readonly IOllamaFactoryProvider _ollamaFactoryProvider = ollamaFactoryProvider;

        [HttpGet("conversations")]
        public IActionResult GetConversations()
        {
            var models = _ollamaFactoryProvider.GetAllConversations();
            return Ok(models);
        }

        [HttpGet("conversations/{id}")]
        public async Task<IActionResult> GetConversationAsync([Required] string id)
        {
            var models = await _ollamaFactoryProvider.GetConversationAsync(id);
            return Ok(models);
        }

        [HttpPost("conversations")]
        public async Task<IActionResult> StreamChatAsync([FromBody] GenerateChatCompletionRequest model, CancellationToken cancellationToken)
        {
            var response = "";
            var results = await _ollamaFactoryProvider.StartNewChatCompletionAsync(model, cancellationToken);
            await foreach (var item in results.response)
            {
                Console.Write(item?.Message.Content);
                response += item?.Message.Content;
            }

            await _ollamaFactoryProvider.AddMessageToConversation(results.conversationId, new Message
            {
                Role = MessageRole.Assistant,
                Content = response
            });

            Console.WriteLine();

            return Ok(new
            {
                message = response,
                results.conversationId
            });
        }

        [HttpPut("conversations/{conversationId}")]
        public async Task<IActionResult> StreamChatAsync([FromRoute][Required] string conversationId, [FromBody] GenerateChatCompletionRequest model, CancellationToken cancellationToken)
        {
            var response = "";
            var results = await _ollamaFactoryProvider.GenerateChatCompletionAsync(conversationId, model, cancellationToken);
            await foreach (var item in results)
            {
                Console.Write(item?.Message.Content);
                response += item?.Message.Content;
            }

            await _ollamaFactoryProvider.AddMessageToConversation(conversationId, new Message
            {
                Role = MessageRole.Assistant,
                Content = response
            });

            Console.WriteLine();

            return Ok(new
            {
                message = response,
                conversationId
            });
        }

        [HttpDelete("conversations/{id}")]
        public async Task<IActionResult> StreamChatAsync([FromRoute][Required] string id, CancellationToken cancellationToken)
        {
            await _ollamaFactoryProvider.DeleteConversationAsync(id);
            return Ok();
        }
    }
}
