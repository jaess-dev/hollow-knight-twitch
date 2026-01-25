import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useEffect, useState } from "react";


let connection: HubConnection | null = null;

const getCsHubConnection = (): HubConnection => {
    if (!connection) {
        connection = new HubConnectionBuilder()
            .withUrl("/hubs/cs")
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();
    }
    return connection;
};


export const useCsHub = () => {
    const [isConnected, setIsConnected] = useState(false);
    const connection = getCsHubConnection();

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

export const PlayerTeam = {
    Undefined: -1,
    Spectator: 0,
    T: 1,
    CT: 2,
} as const;

export type PlayerTeam = typeof PlayerTeam[keyof typeof PlayerTeam];

export const PlayerActivity = {
    //
    // Summary:
    //     Undefined.
    Undefined: -1,
    //
    // Summary:
    //     The player is playing.
    Playing: 0,
    //
    // Summary:
    //     The player is in a menu.
    Menu: 1,
    //
    // Summary:
    //     The player is inputting text.
    TextInput: 2
} as const;

export type PlayerActivity = typeof PlayerActivity[keyof typeof PlayerActivity];

export type PlayerState = {
    //
    // Summary:
    //     The player health amount.
    Health: number;

    //
    // Summary:
    //     The player armor amount.
    Armor: number;

    //
    // Summary:
    //     Does the player have a helmet?
    HasHelmet: boolean;

    //
    // Summary:
    //     The amount the player is flashed. From 0 to 255.
    FlashAmount: number;

    //
    // Summary:
    //     The amount the player is smoked. From 0 to 255.
    SmokedAmount: number;

    //
    // Summary:
    //     The amount the player is burning. From 0 to 255.
    BurningAmount: number;

    //
    // Summary:
    //     The amount of money the player has.
    Money: number;

    //
    // Summary:
    //     The number of kills the player has in the current round.
    RoundKills: number;

    //
    // Summary:
    //     The number of headshot kills the player has in the current round.
    RoundHSKills: number;

    //
    // Summary:
    //     The total damage amount the player has earned in the current round.
    RoundTotalDamage: number;

    //
    // Summary:
    //     The total equipment value of the player.
    EquipmentValue: number;

    //
    // Summary:
    //     Does the player have a defuse kit?
    HasDefuseKit: boolean;
};

export type MatchStats = {
    //
    // Summary:
    //     The number of kills.
    Kills: number;

    //
    // Summary:
    //     The number of assits.
    Assists: number;

    //
    // Summary:
    //     The number of deaths.
    Deaths: number;

    //
    // Summary:
    //     The number of MVPs.
    MVPs: number;

    //
    // Summary:
    //     The amount of score.
    Score: number;
}

export type Player = {
    name: string;
    XPOverloadLevel: number;
    Clan: string;
    ObserverSlot: number;

    Team: PlayerTeam;
    Activity: PlayerActivity;
    State: PlayerState;
    MatchStats: MatchStats;
    // Weapons: Weapon[];
    // SpectationTarget: string;
    // Position: Vector3D;
    // ForwardDirection: Vector3D;
};

export type PlayerScoreUpdated = {
    New: number,
    Previous: number,
    Player: Player,
};

export type CEWrapper<T> = {
    EventInfo: T
}

export const usePlayerScoreUpdated = (
    eventHandler: (event: PlayerScoreUpdated) => void
) => {
    const { connection } = useCsHub();

    useEffect(() => {
        connection.on("CEPlayerScoreChanged", (event: CEWrapper<PlayerScoreUpdated>) => {
            return eventHandler(event.EventInfo);
        });

        return () => {
            connection.off("CEPlayerScoreChanged", eventHandler);
        };
    }, [connection, eventHandler]);
};
