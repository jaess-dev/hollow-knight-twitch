import { useEffect, useState } from "react";
import { useDeathEvent, useGrubSavedEvent, useHazardDeathEvent } from "../../lib/signalr/useHkHub";
import type { EventType } from "./HkEffect";
import HollowKnightEventOverlay from "./HkEffect";
// import { HollowKnightDeathCounter } from "./compontes/DeathCounter";


export const HkOverlay = () => {
    const [currentEvent, setCurrentEvent] = useState<EventType>("NONE");
    // const [deathCount, setDeathCount] = useState<number | null>(0);

    useHazardDeathEvent((_) => setCurrentEvent("HAZARD_HIT"));
    useGrubSavedEvent((_) => setCurrentEvent("GRUB_SAVED"));
    useDeathEvent((_) => setCurrentEvent("PLAYER_DEATH"));
    // useRespawnEvent(({ playerData: { deathCount } }) => setDeathCount(deathCount || null))

    // useEffect(() => {
    //     // setCurrentEvent("HAZARD_HIT");
    //     // setCurrentEvent("GRUB_SAVED");
    //     setCurrentEvent("PLAYER_DEATH");
    // }, []);

    const handleEventComplete = () => setCurrentEvent("NONE");

    return (
        <>
            <HollowKnightEventOverlay
                currentEvent={currentEvent}
                onEventComplete={handleEventComplete}
            />
            {/* {deathCount && <HollowKnightDeathCounter deathCount={deathCount} />} */}
        </>
    );
};



