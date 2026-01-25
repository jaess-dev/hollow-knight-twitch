import { useEffect, useState } from "react";
import { useDeathEvent, useGrubSavedEvent, useHazardDeathEvent } from "../../lib/signalr/useSignalr";
import type { EventType } from "./HkEffect";
import HollowKnightEventOverlay from "./HkEffect";


export const HkOverlay = () => {
    const [currentEvent, setCurrentEvent] = useState<EventType>("NONE");

    useHazardDeathEvent((_) => setCurrentEvent("HAZARD_HIT"));
    useGrubSavedEvent((_) => setCurrentEvent("GRUB_SAVED"));
    useDeathEvent((_) => setCurrentEvent("PLAYER_DEATH"));

    // useEffect(() => {
    //     // setCurrentEvent("HAZARD_HIT");
    //     // setCurrentEvent("GRUB_SAVED");
    //     setCurrentEvent("PLAYER_DEATH");
    // }, []);

    const handleEventComplete = () => setCurrentEvent("NONE");

    return (
        <HollowKnightEventOverlay
            currentEvent={currentEvent}
            onEventComplete={handleEventComplete}
        />
    );
};