using TwitchLib.Client.Interfaces;


namespace twitch_con.TwClient;

public interface ITwitchClientSubscriber
{
    void Sub(ITwitchClient client);
}
