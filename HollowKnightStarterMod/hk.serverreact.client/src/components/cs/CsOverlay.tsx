import { useEffect, useState } from "react";
import { usePlayerScoreUpdated, type MatchStats } from "../../lib/signalr/useCsHub"
import CS2KDAOverlay, { type KDAStats } from "./CsKDA";

export function CsOverlay() {
    const [score, setScore] = useState<KDAStats | null>(null);

    usePlayerScoreUpdated((e) => {
        console.log(e.Player.MatchStats.Kills);
        const kda = {
            kills: e.Player.MatchStats.Kills,
            deaths: e.Player.MatchStats.Deaths,
            assists: e.Player.MatchStats.Assists,
        };
        console.log(kda);
        setScore(
            kda.kills === -1
                && kda.deaths === -1
                && kda.assists === -1
            ? null : kda
        );
    });

    return <>
        {score && <CS2KDAOverlay stats={score} />}
    </>
}