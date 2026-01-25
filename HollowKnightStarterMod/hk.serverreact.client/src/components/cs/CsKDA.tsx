import React, { useState } from 'react';
import './CsKDA.css';

export type KDAStats = {
    kills: number;
    deaths: number;
    assists: number;
}

const CS2KDAOverlay = ({ stats }: { stats?: KDAStats }) => {
    if (stats === undefined){
        return <></>;
    }

    const [pulseKill, setPulseKill] = useState(false);
    const [pulseDeath, setPulseDeath] = useState(false);
    const [pulseAssist, setPulseAssist] = useState(false);

    const ratio = stats.deaths === 0 ? stats.kills.toFixed(2) : (stats.kills / stats.deaths).toFixed(2);

    return (
        <div className="cs-overlay-container">
            <div className="kda-panel">
                <div className="kda-main-panel">
                    <div className="kda-stats-container">
                        {/* Kills */}
                        <div className={`stat-line ${pulseKill ? 'stat-flash' : ''}`}>
                            <div className="stat-label stat-label-kill">KILLS</div>
                            <div className="stat-value">{stats.kills}</div>
                        </div>

                        {/* Deaths */}
                        <div className={`stat-line ${pulseDeath ? 'stat-flash' : ''}`}>
                            <div className="stat-label stat-label-death">DEATHS</div>
                            <div className="stat-value">{stats.deaths}</div>
                        </div>

                        {/* Assists */}
                        <div className={`stat-line ${pulseAssist ? 'stat-flash' : ''}`}>
                            <div className="stat-label stat-label-assist">ASSISTS</div>
                            <div className="stat-value">{stats.assists}</div>
                        </div>

                        {/* Separator */}
                        <div className="stat-separator"></div>

                        {/* K/D Ratio */}
                        <div className="stat-line">
                            <div className="stat-label stat-label-ratio">RATIO</div>
                            <div className="stat-value stat-value-ratio">{ratio}</div>
                        </div>
                    </div>

                    <div className="kda-bottom-accent"></div>
                </div>
            </div>
        </div>
    );
};

export default CS2KDAOverlay;