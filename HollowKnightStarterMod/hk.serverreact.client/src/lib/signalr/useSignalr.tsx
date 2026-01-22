import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useEffect, useState } from "react";


let connection: HubConnection | null = null;

const getSignalRConnection = (): HubConnection => {
    if (!connection) {
        connection = new HubConnectionBuilder()
            .withUrl("/hubs/notifications")
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();
    }
    return connection;
};


export const useSignalR = () => {
    const [isConnected, setIsConnected] = useState(false);
    const connection = getSignalRConnection();

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
