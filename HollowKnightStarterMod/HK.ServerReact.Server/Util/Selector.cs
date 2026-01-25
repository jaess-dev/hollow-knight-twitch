namespace HK.ServerReact.Server.Util;

public static class Selector
{
    private static Random _random = new();
    public static T ChooseRandom<T>(this Span<T> col)
    {
        return col[_random.Next(col.Length)];
    }
}