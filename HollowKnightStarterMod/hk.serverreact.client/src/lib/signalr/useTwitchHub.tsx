import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useEffect, useState } from "react";


let connection: HubConnection | null = null;

const getTwitchHubConnection = (): HubConnection => {
    if (!connection) {
        connection = new HubConnectionBuilder()
            .withUrl("/hubs/twitch")
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();
    }
    return connection;
};


export const useTwitchHub = () => {
    const [isConnected, setIsConnected] = useState(false);
    const connection = getTwitchHubConnection();

    useEffect(() => {
        if (connection.state === "Disconnected") {
            connection.start()
                .then(() => setIsConnected(true))
                .catch(err => console.error("SignalR connection error:", err));
        } else if (connection.state === "Connected") {
            setIsConnected(true);
        }

        const handleClose = () => setIsConnected(false);
        const handleReconnect = () => setIsConnected(true);

        connection.onclose(handleClose);
        connection.onreconnected(handleReconnect);

        return () => {
            connection.off("close", handleClose);
            connection.off("reconnected", handleReconnect);
        };
    }, [connection]);

    return { connection, isConnected };
};


export type TwitchMessageEvent = {
    className: "TwitchMessageEvent",
    chatMessage: ChatMessage,
}


export type Emote = {
    id: string;
    name: string;
    startIndex: number;
    endIndex: number;
    imageUrl: string;
};

export type ChatMessage = {
    badgeInfo: any[];
    badges: { key: string, value: string }[];
    bits: number;
    bitsInDollars: number;
    botUsername: string;
    channel: string;
    chatReply: null;
    cheerBadge: null;
    customRewardId: null;
    displayName: string;
    emoteReplacedMessage: null;
    emoteSet: { emotes: Emote[], rawEmoteSetString: string };
    hexColor: string;
    hypeChat: null;
    id: string;
    isBroadcaster: boolean;
    isFirstMessage: false;
    isHighlighted: false;
    isMe: false;
    isSkippingSubModule: boolean;
    message: string;
    noisy: number;
    rawIrcMessage: string;
    roomId: string;
    subscribedMonthCount: number;
    tmiSent: string;
    undocumentedTags: { "client-nonce": string; flags: string; "returning-chatter": string };
    userDetail: { isModerator: boolean; isSubscriber: boolean; hasTurbo: boolean; isVip: boolean; isPartner: boolean; issStaff: boolean };
    userId: string;
    userType: number;
    username: string;
};


export const useTwitchMessageEvent = (
    eventHandler: (event: TwitchMessageEvent) => void
) => {
    const { connection } = useTwitchHub();

    useEffect(() => {
        connection.on("TwitchMessageEvent", (event) => {
            console.log(event);
            return eventHandler(event);
        });

        return () => {
            connection.off("TwitchMessageEvent", eventHandler);
        };
    }, [connection, eventHandler]);
};
