using System.Collections;

namespace HK.ServerReact.Server.Features;

public interface IFeature
{
    void AddServices(IServiceCollection services);
    void MapEndpoints(WebApplication app);
}

public interface ITypedFeature;

public sealed class FeatureProvider() : IEnumerable<IFeature>
{
    private readonly List<IFeature> _features = [];
    private readonly Dictionary<Type, object> _typedFeatureContainer = [];


    public T[] GetFeatures<T>() where T : ITypedFeature
    {
        var key = typeof(T);
        T[] typedFeatures;
        if (_typedFeatureContainer.TryGetValue(key, out var obj))
        {
            typedFeatures = obj as T[] ?? [];
        }
        else
        {
            typedFeatures = _features.OfType<T>().ToArray();
            _typedFeatureContainer.Add(key, typedFeatures);
        }

        return typedFeatures;
    }

    public FeatureProvider(ReadOnlySpan<IFeature> features) : this()
    {
        _features.AddRange(features);
    }

    public void Add(IFeature feature)
    {
        _features.Add(feature);
    }

    public void AddServices(IServiceCollection services)
    {
        foreach (var feature in _features)
            feature.AddServices(services);
    }
    public void MapEndpoints(WebApplication app)
    {
        foreach (var feature in _features)
            feature.MapEndpoints(app);
    }

    public IEnumerator<IFeature> GetEnumerator()
    {
        foreach (var feature in _features)
            yield return feature;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}