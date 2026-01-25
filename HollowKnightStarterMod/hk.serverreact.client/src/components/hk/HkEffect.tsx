import React from 'react';
import './HkEffect.css';

// Event types
export type EventType =
    'PLAYER_DEATH' |
    'GRUB_SAVED' |
    'HAZARD_HIT' |
    'NONE';

// Player Death Animation Component
const PlayerDeathAnimation = () => {
    console.log("Player death");
    return (
        <div className="absolute inset-0 flex items-center justify-center pointer-events-none">
            <div className="soul-container">
                <div className="shade-soul"></div>
                <div className="particle particle-1"></div>
                <div className="particle particle-2"></div>
                <div className="particle particle-3"></div>
                <div className="particle particle-4"></div>
            </div>
            <div className="death-flash"></div>
            <div className="death-text">SOUL DEPARTED</div>
        </div>
    );
};

// Grub Saved Animation Component
const GrubSavedAnimation = () => {
    return (
        <div className="absolute inset-0 flex items-center justify-center pointer-events-none">
            <div className="grub-container">
                <div className="grub-glow"></div>
                <div className="grub-icon">🐛</div>
                <div className="grub-particle grub-particle-1"></div>
                <div className="grub-particle grub-particle-2"></div>
                <div className="grub-particle grub-particle-3"></div>
                <div className="grub-particle grub-particle-4"></div>
                <div className="grub-particle grub-particle-5"></div>
            </div>
            <div className="grub-text">GRUB RESCUED</div>
        </div>
    );
};

// Hazard Hit Animation Component
const HazardHitAnimation = () => {
    return (
        <div className="absolute inset-0 pointer-events-none">
            <div className="hazard-flash"></div>
            <div className="hazard-shake">
                <div className="hazard-vignette"></div>
            </div>
            <div className="hazard-impact"></div>
            <div className="hazard-impact-2"></div>
        </div>
    );
};

interface HollowKnightEventOverlayProps {
    currentEvent: EventType;
    onEventComplete?: () => void;
}

// Higher Order Component - Event Manager
const HollowKnightEventOverlay: React.FC<HollowKnightEventOverlayProps> = ({
    currentEvent,
    onEventComplete
}) => {
    React.useEffect(() => {
        if (currentEvent === "NONE") return;

        const duration = currentEvent === "HAZARD_HIT" ? 800 : 3000;
        const timer = setTimeout(() => {
            onEventComplete?.();
        }, duration);

        return () => clearTimeout(timer);
    }, [currentEvent, onEventComplete]);

    return (
        <div className="w-full h-screen bg-gradient-to-b from-gray-900 to-black relative overflow-hidden">
            {currentEvent === "PLAYER_DEATH" && <PlayerDeathAnimation />}
            {currentEvent === "GRUB_SAVED" && <GrubSavedAnimation />}
            {currentEvent === "HAZARD_HIT" && <HazardHitAnimation />}
        </div>
    );
};

export default HollowKnightEventOverlay;