using System.Text.Json;
using DotNetEnv;
using HK.Domain;
using HK.ServerReact.Server.Config;
using HK.ServerReact.Server.Features;
using HK.ServerReact.Server.Features.CounterStrikeFeatures;
using HK.ServerReact.Server.Features.HollowKnightFeatures;
using HK.ServerReact.Server.Features.HollowKnightFeatures.DeathMessageFeature;
using HK.ServerReact.Server.Features.HollowKnightFeatures.RespawnMessageFeature;
using HK.ServerReact.Server.Features.TwClient;
using HK.ServerReact.Server.Services;
using HK.ServerReact.Server.Services.TwitchConnection;
using Microsoft.AspNetCore.Mvc;

IFeature[] hkFeatures = [
    new HkEventDistributerFeature(),
    new DeathMessageFeature(),
    new RespawnMessageFeature(),
    new HazardResponseFeature(),
    new DeathCounterFeature(),
];
IFeature[] csFeatures = [
    new CsEventDistributer(),
    new KobeFeature(),
    new ThatsHotFeature(),
];
FeatureProvider features = [
    new TwitchSubscriptionFeature(),
    ..hkFeatures,
    ..csFeatures,
];

Env.Load();

var builder = WebApplication.CreateBuilder(args);

string targetChannel = Environment.GetEnvironmentVariable("TARGET_CHANNEL")!;
string botName = Environment.GetEnvironmentVariable("BOT_NAME")!;
string clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET")!;
string clientId = Environment.GetEnvironmentVariable("CLIENT_ID")!;

builder.Services
    .ConfigureServices()
    .ConfigureTwitchBot(targetChannel, botName, clientSecret, clientId);
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
features.MapEndpoints(app);

app.MapPost("/api/twitch/message", async (
    IBotMessageSender botConnector,
    ILogger<Program> logger,
    [FromBody] MessageDot payload) =>
{
    var message = payload.Message;
    logger.LogInformation("Received message: {MESSAGE}", message);
    await botConnector.SendChatMessageAsync(message);
});

app.MapTwitchAuthEndpoints(clientId, clientSecret, "https://localhost:54024", pathBase: "/api");

app.MapFallbackToFile("/index.html");
app.Run();


public record MessageDot(string Message);