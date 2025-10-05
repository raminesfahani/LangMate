using LangMate.Abstractions.Contracts;
using LangMate.AppHost.ApiService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ollama;
using System;
using System.ComponentModel.DataAnnotations;

namespace LangMate.AppHost.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController(ILogger<ChatController> logger, IOllamaFactory ollamaFactoryProvider) : ControllerBase
    {
        private readonly ILogger<ChatController> _logger = logger;
        private readonly IOllamaFactory _ollamaFactoryProvider = ollamaFactoryProvider;

        /// <summary>
        /// Retrieves all stored chat conversations.
        /// </summary>
        /// <remarks>
        /// Returns a list of conversation metadata, including titles and models.
        /// </remarks>
        /// <returns>List of <see cref="ConversationDocument"/> objects.</returns>
        [HttpGet("conversations")]
        public IActionResult GetConversations()
        {
            var models = _ollamaFactoryProvider.GetAllConversations();
            return Ok(models);
        }

        /// <summary>
        /// Retrieves a specific conversation by its unique identifier.
        /// </summary>
        /// <param name="id">The conversation ID.</param>
        /// <returns>The full <see cref="ConversationDocument"/> including message history.</returns>
        [HttpGet("conversations/{id}")]
        public async Task<IActionResult> GetConversationAsync([Required] string id)
        {
            var models = await _ollamaFactoryProvider.GetConversationAsync(id);
            return Ok(models);
        }

        /// <summary>
        /// Starts a new chat conversation and generates a streaming chat completion.
        /// </summary>
        /// <param name="model">The chat completion request containing initial messages and model info.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// An object containing the new conversation ID and the generated message content.
        /// </returns>
        [HttpPost("conversations")]
        public async Task<IActionResult> StartNewChatCompletionAsync([FromBody] GenerateChatCompletionRequest model, CancellationToken cancellationToken)
        {
            var response = "";
            var results = await _ollamaFactoryProvider.StartNewChatCompletionAsync(model, cancellationToken);
            await foreach (var item in results.response)
            {
                response += item?.Message.Content;
            }

            return Created($"/Chat/conversations/{results.conversationId}", new StartNewChatResponse(response, results.conversationId));
        }

        /// <summary>
        /// Continues an existing conversation by generating a new streaming chat completion.
        /// </summary>
        /// <param name="conversationId">The ID of the conversation to continue.</param>
        /// <param name="model">The chat completion request with new messages.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>
        /// An object containing the conversation ID and the generated message content.
        /// </returns>
        [HttpPut("conversations/{conversationId}")]
        public async Task<IActionResult> GenerateChatCompletionAsync([FromRoute][Required] string conversationId, [FromBody] GenerateChatCompletionRequest model, CancellationToken cancellationToken)
        {
            var response = "";
            var results = await _ollamaFactoryProvider.GenerateChatCompletionAsync(conversationId, model, cancellationToken);
            await foreach (var item in results)
            {
                response += item?.Message.Content;
            }

            return Accepted(new StartNewChatResponse(response, conversationId));
        }

        /// <summary>
        /// Deletes a conversation and all its associated messages.
        /// </summary>
        /// <param name="id">The conversation ID to delete.</param>
        /// <returns>HTTP 200 OK on successful deletion.</returns>
        [HttpDelete("conversations/{id}")]
        public async Task<IActionResult> DeleteConversationAsync([FromRoute][Required] string id)
        {
            await _ollamaFactoryProvider.DeleteConversationAsync(id);
            return NoContent();
        }
    }
}
