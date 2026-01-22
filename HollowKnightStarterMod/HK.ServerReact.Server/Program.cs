using System.Text.Json;
using HK.Domain;
using HK.ServerReact.Server.Hubs;
using HollowKnightStarterMod;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:54024",  // React dev server
                "https://localhost:54024"  // React dev server
                                           // ,"https://your-production-frontend.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Required for SignalR
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<BotConnector>();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseWebSockets();
app.UseRouting();

app.UseAuthorization();

app.MapHub<NotificationsHub>("/hubs/notifications");
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.MapPost("/api/hk/event",
    async (
        BotConnector botConnector,
        ILogger<Program> logger,
        JsonDocument payload
    ) =>
    {
        JsonElement root = payload.RootElement;

        var eventJson = root.GetProperty("event");
        string className = eventJson
            .GetProperty("ClassName")
            .GetString()
            ?? throw new BadHttpRequestException("ClassName missing");

        IEvent? @event = className switch
        {
            nameof(DeathEvent) => eventJson.Deserialize<DeathEvent>(),
            _ => null
        };

        switch (@event)
        {
            case DeathEvent deathEvent:
                await botConnector.SendYouDiedAsync();
                break;
        }
    });

app.Run();
