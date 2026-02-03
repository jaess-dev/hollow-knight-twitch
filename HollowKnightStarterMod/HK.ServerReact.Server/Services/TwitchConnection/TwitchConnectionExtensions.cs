using System.Text.Json;

namespace HK.ServerReact.Server.Services.TwitchConnection;


public static class TwitchConnectionExtensions
{
    extension(IEndpointRouteBuilder self)
    {
        /// <summary>
        /// Maps two additional endpoints for retrieving an oauth token.
        /// 
        /// The first aus {host}/{pathBase}/auth/twitch/login, which should be called to start the oauth process.
        /// The second is {host}/{pathBase}/auth/twitch/callback, which should be added as the redirect uri in the twitch app.
        /// </summary>
        /// <param name="clientId">The client id of the twitch app</param>
        /// <param name="clientSecret">The client secret of the twitch app</param>
        /// <param name="host">The host of this server, i.e. https://domain.any</param>
        /// <param name="pathBase">The base for these urls, i.e. "api"</param>
        public void MapTwitchAuthEndpoints(
            string clientId,
            string clientSecret,
            string host,
            string pathBase = "")
        {
            var http = new HttpClient();
            var uri = $"{host}{pathBase}/auth/twitch/callback";
            var RedirectUri = Uri.EscapeDataString(uri);

            RouteGroupBuilder group = self.MapGroup(pathBase);
            /// 1️⃣ Redirect user/bot to Twitch OAuth
            group.MapGet("/auth/twitch/login", async (ITokenStore tokenStore) =>
            {
                uri = uri;
                var tokens = (await tokenStore.LoadAsync() ?? []).ToArray();
                if (tokens is { Length: > 0 } && tokens.Any(t => t.ExpiresAtUtc >= DateTimeOffset.UtcNow))
                {
                    return Results.Ok("Got a token loaded");
                }

                // todo: Redirect rui is completly ignored
                var url =
                    "https://id.twitch.tv/oauth2/authorize" +
                    $"?client_id={clientId}" +
                    $"&redirect_uri={uri}" +
                    "&response_type=code" +
                    "&scope=chat:read chat:edit";

                return Results.Redirect(url);
            });

            /// 2️⃣ OAuth callback + token exchange
            group.MapGet("/auth/twitch/callback", async (HttpContext ctx, ITokenStore tokenStore) =>
            {
                var code = ctx.Request.Query["code"].ToString();
                if (string.IsNullOrEmpty(code))
                    return Results.BadRequest("Missing code");

                var response = await http.PostAsync(
                    "https://id.twitch.tv/oauth2/token" +
                    $"?client_id={clientId}" +
                    $"&client_secret={clientSecret}" +
                    $"&code={code}" +
                    $"&grant_type=authorization_code" +
                    $"&redirect_uri={RedirectUri}",
                    null
                );

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var accessToken = doc.RootElement.GetProperty("access_token").GetString()!;
                var refreshToken = doc.RootElement.GetProperty("refresh_token").GetString()!;
                var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();

                var tokens = await tokenStore.LoadAsync() ?? [];
                await tokenStore.SaveAsync(
                    [
                        ..tokens,
                        new TwitchToken
                        {
                            AccessToken = accessToken,
                            RefreshToken = refreshToken,
                            ExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(expiresIn)
                        }
                    ]
                );

                return Results.Ok("OAuth success. Token stored.");
            });
        }
    }
}