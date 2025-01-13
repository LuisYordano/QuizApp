using QuizApp.Components;
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using System.ClientModel;
using Azure;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

var op = new OpenAIClientOptions
{
    Endpoint = new Uri(builder.Configuration["CustomOpenAI:Endpoint"] ?? throw new InvalidOperationException("Missing CustomOpenAI:Endpoint"))    
};
var cred = new ApiKeyCredential(builder.Configuration["CustomOpenAI:Key"] ?? throw new InvalidOperationException("Missing CustomOpenAI:Key"));

var innerChatClient = new OpenAIClient(cred,op)
    .AsChatClient(builder.Configuration["CustomOpenAI:Model"] ?? throw new InvalidOperationException("Missing CustomOpenAI:Model"));
//
// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddChatClient(pipeline => pipeline
    .Use(innerChatClient));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
