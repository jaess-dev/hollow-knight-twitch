using System.Text.Json;
using HK.Domain;
using HK.ServerReact.Server.Features;
using HK.ServerReact.Server.Features.CounterStrikeFeatures;
using HK.ServerReact.Server.Features.HollowKnightFeatures;
using HK.ServerReact.Server.Features.HollowKnightFeatures.DeathMessageFeature;
using HK.ServerReact.Server.Features.HollowKnightFeatures.RespawnMessageFeature;
using HK.ServerReact.Server.Features.Hubs;
using HK.ServerReact.Server.Services;
using Microsoft.AspNetCore.Mvc;

IFeature[] hkFeatures = [
    new HkEventDistributerFeature(),
    new DeathMessageFeature(),
    new RespawnMessageFeature(),
    new HazardResponseFeature(),
];
IFeature[] csFeatures = [
    new CsEventDistributer(),
    new KobeFeature(),
    new ThatsHotFeature(),
];
FeatureProvider features = [
    ..hkFeatures,
    ..csFeatures,
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

builder.Services.AddSingleton(static sp =>
{
    var url = sp.GetRequiredService<IConfiguration>().GetSection("WebSocket").Get<string>() ?? "ws://localhost:8080";
    return new WebSocketService(new Uri(url), sp.GetRequiredService<ILogger<WebSocketService>>());
});
builder.Services.AddSingleton<IWebSocketService>(sp =>
    sp.GetRequiredService<WebSocketService>());
builder.Services.AddSingleton<IBotConnectorServices, BotWsConnectorService>();

// builder.Services.AddHostedService<WebSocketHostedService>();


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

app.MapControllers();
app.MapFallbackToFile("/index.html");
features.MapEndpoints(app);

app.MapPost("/api/twitch/message", async (
    IBotConnectorServices botConnector,
    ILogger<Program> logger,
    [FromBody] MessageDot payload) =>
{
    var message = payload.Message;
    logger.LogInformation("Received message: {MESSAGE}", message);
    await botConnector.SendChatMessageAsync(message);
});

app.Run();

public record MessageDot(string Message);