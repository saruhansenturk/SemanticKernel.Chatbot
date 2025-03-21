using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernel.Chatbot.Hub;

namespace SemanticKernel.Chatbot.Service
{
    public class AIService
    {
        private readonly IHubContext<AIHub> _hubContext;
        private readonly IChatCompletionService _chatCompletionService;

        // Klasik constructor yöntemi
        public AIService(IHubContext<AIHub> hubContext, IChatCompletionService chatCompletionService)
        {
            _hubContext = hubContext;
            _chatCompletionService = chatCompletionService;
        }

        public async Task GetMessageStreamAsync(string prompt, string connectionId,
            CancellationToken? cancellationToken = default!)
        {
            await foreach (var response in _chatCompletionService.GetStreamingChatMessageContentsAsync(prompt))
            {
                cancellationToken?.ThrowIfCancellationRequested();

                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", response.ToString());
            }
        }
    }
}
