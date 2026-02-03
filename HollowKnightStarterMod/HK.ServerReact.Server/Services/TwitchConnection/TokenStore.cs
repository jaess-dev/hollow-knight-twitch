using System.Text.Json;

namespace HK.ServerReact.Server.Services.TwitchConnection;

public interface ITokenStore
{
    event Action<ITokenStore>? OnChange;
    Task SaveAsync(IEnumerable<TwitchToken> data);
    Task<IEnumerable<TwitchToken>?> LoadAsync();
}

public class TwitchToken
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTimeOffset ExpiresAtUtc { get; set; }
}

internal class TwitchTokenJsonStore(string path) : JsonStore<IEnumerable<TwitchToken>>(path), ITokenStore
{
    event Action<ITokenStore>? ITokenStore.OnChange
    {
        add => OnChange += (_) => value?.Invoke(this);
        remove => OnChange -= (_) => value?.Invoke(this);
    }
}


internal class JsonStore<T>(string path) where T : class
{
    public event Action<JsonStore<T>>? OnChange;

    private readonly string _path = path;

    private readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public async Task SaveAsync(T data)
    {
        var json = JsonSerializer.Serialize(data, Options);
        await File.WriteAllTextAsync(_path, json);
        OnChange?.Invoke(this);
    }

    public async Task<T?> LoadAsync()
    {
        if (!File.Exists(_path))
            return null;

        var json = await File.ReadAllTextAsync(_path);
        return JsonSerializer.Deserialize<T>(json);
    }
}

internal record TargetChannel(string targetChannel);