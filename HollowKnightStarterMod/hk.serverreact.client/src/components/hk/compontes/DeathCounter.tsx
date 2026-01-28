import './DeathCounter.css';

type DeathCounterProps = {
    deathCount: number;
}

export const HollowKnightDeathCounter = ({ deathCount }: DeathCounterProps) => {
    return (
        <div style={{ position: 'relative', width: '100vw', height: '100vh', background: 'linear-gradient(to bottom, #1a1a2e, #0a0a0f)' }}>
            <div className="hk-death-counter-container">
                <div className="hk-death-counter-panel">
                    <div className="hk-death-counter-skull">💀</div>
                    <div className="hk-death-counter-value">{deathCount ?? 0}</div>
                </div>
            </div>
        </div>
    );
};