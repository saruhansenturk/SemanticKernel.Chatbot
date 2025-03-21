using System.ClientModel;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.SemanticKernel;
using OpenAI;
using SemanticKernel.Chatbot.Hub;
using SemanticKernel.Chatbot.Service;
using SemanticKernel.Chatbot.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOpenAIChatCompletion(
    modelId: "qwen/qwq-32b:free",
    openAIClient: new OpenAIClient(
        credential: new ApiKeyCredential("API_KEY"),
        options: new OpenAIClientOptions
        {
            Endpoint = new Uri("https://openrouter.ai/api/v1")
        })
);

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy => policy
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials()
                                        .SetIsOriginAllowed(s => true));
});

builder.Services.AddSingleton<AIService>();

builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapPost("/chat", async (AIService aiService, ChatRequestVM chatRequest, CancellationToken cancellationToken)
    => await aiService.GetMessageStreamAsync(chatRequest.Prompt, chatRequest.ConnectionId, cancellationToken));

app.MapHub<AIHub>("ai-hub");

app.Run();
