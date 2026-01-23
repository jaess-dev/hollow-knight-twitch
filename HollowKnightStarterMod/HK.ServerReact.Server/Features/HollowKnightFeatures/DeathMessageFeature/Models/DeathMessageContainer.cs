using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace HK.ServerReact.Server.Features.HollowKnightFeatures.DeathMessageFeature.Models;


public sealed class DeathMessageContainer(params string[] messages) : IEnumerable<string>
{
    private readonly string[] _messages = messages;
    public int Length => _messages.Length;

    public string this[int idx] => _messages[idx];

    public IEnumerator<string> GetEnumerator()
    {
        return _messages.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _messages.GetEnumerator();
    }
}