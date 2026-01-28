import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useEffect, useState } from "react";


let connection: HubConnection | null = null;

const getHkHubConnection = (): HubConnection => {
    if (!connection) {
        connection = new HubConnectionBuilder()
            .withUrl("/hubs/hk")
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();
    }
    return connection;
};


export const useHkHub = () => {
    const [isConnected, setIsConnected] = useState(false);
    const connection = getHkHubConnection();

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

export type HazardDeathEvent = {
    className: "HazardDeathEvent",
    hazardDeathDto: {
        "hazardTypeDto": "SPIKES" | "NON_HAZARD" | "ACID" | "LAVA" | "PIT",
    },
}

export const useHazardDeathEvent = (
    eventHandler: (event: HazardDeathEvent) => void
) => {
    const { connection } = useHkHub();

    useEffect(() => {
        connection.on("HazardDeathEvent", (event) => {
            console.log(event);
            return eventHandler(event);
        });

        return () => {
            connection.off("HazardDeathEvent", eventHandler);
        };
    }, [connection, eventHandler]);
};

export type GrubSavedEvent = {
    grubCount: number;
}

export const useGrubSavedEvent = (
    eventHandler: (event: GrubSavedEvent) => void
) => {
    const { connection } = useHkHub();

    useEffect(() => {
        connection.on("GrubSavedEvent", (event) => {
            console.log(event);
            return eventHandler(event);
        });

        return () => {
            connection.off("GrubSavedEvent", eventHandler);
        };
    }, [connection, eventHandler]);
};


export const useDeathEvent = (
    eventHandler: (event: {}) => void
) => {
    const { connection } = useHkHub();

    useEffect(() => {
        connection.on("DeathEvent", (event) => {
            console.log(event);
            return eventHandler(event);
        });

        return () => {
            connection.off("DeathEvent", eventHandler);
        };
    }, [connection, eventHandler]);
};


export const useRespawnEvent = (
    eventHandler: (event: {
        playerData: {
            geo: number,
            grubsCollected: number,
            deathCount?: number
        }
    }) => void
) => {
    const { connection } = useHkHub();

    useEffect(() => {
        connection.on("RespawnEvent", (event) => {
            console.log(event);
            return eventHandler(event);
        });

        return () => {
            connection.off("RespawnEvent", eventHandler);
        };
    }, [connection, eventHandler]);
};