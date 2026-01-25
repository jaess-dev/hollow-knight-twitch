using System.Text.Json;
using HK.Domain;
using HK.ServerReact.Server.Features;
using HK.ServerReact.Server.Features.HollowKnightFeatures;
using HK.ServerReact.Server.Features.HollowKnightFeatures.DeathMessageFeature;
using HK.ServerReact.Server.Features.HollowKnightFeatures.RespawnMessageFeature;
using HK.ServerReact.Server.Hubs;
using HK.ServerReact.Server.Services;
using Microsoft.AspNetCore.Mvc;

FeatureProvider features = [
    new HkEventDistributerFeature(),
    new DeathMessageFeature(),
    new RespawnMessageFeature(),
    new HazardResponseFeature(),
];

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

builder.Services.AddSingleton<BotConnectorServices>();
features.AddServices(builder.Services);

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
features.MapEndpoints(app);

app.MapPost("/api/twitch/message", async (
    BotConnectorServices botConnector,
    ILogger<Program> logger,
    [FromBody] MessageDot payload) =>
{
    var message = payload.Message;
    logger.LogInformation("Received message: {MESSAGE}", message);
    await botConnector.SendChatMessageAsync(message);
});

app.Run();

public record MessageDot(string Message);