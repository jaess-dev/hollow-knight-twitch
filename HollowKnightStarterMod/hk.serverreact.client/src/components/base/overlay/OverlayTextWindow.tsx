import "./OverlayTextWindow.css";

import React, { useState, useEffect, useRef, useCallback } from 'react';

// ─── Types ────────────────────────────────────────────────────────────────────

type ScreenStatus = 'starting' | 'brb' | 'ended';

interface TimerConfig {
  enabled: boolean;
  duration: number; // in seconds
}

interface ScreenOverlayProps {
  status: ScreenStatus;
  timer?: TimerConfig;
  onTimerEnd?: () => void;
}

// ─── Helpers ──────────────────────────────────────────────────────────────────

const formatTime = (totalSeconds: number): string => {
  const h = Math.floor(totalSeconds / 3600);
  const m = Math.floor((totalSeconds % 3600) / 60);
  const s = totalSeconds % 60;
  if (h > 0) return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
  return `${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
};

const STATUS_CONFIG: Record<ScreenStatus, { title: string; subtitle: string; color: string; glowColor: string }> = {
  starting: {
    title: 'STREAM STARTING',
    subtitle: 'Get ready — we\'re about to go live',
    color: '#00ff41',
    glowColor: 'rgba(0, 255, 65, 0.6)',
  },
  brb: {
    title: 'BE RIGHT BACK',
    subtitle: 'Stepping away for a moment',
    color: '#00ff88',
    glowColor: 'rgba(0, 255, 136, 0.6)',
  },
  ended: {
    title: 'STREAM ENDED',
    subtitle: 'Thanks for watching — see you next time',
    color: '#39ff14',
    glowColor: 'rgba(57, 255, 20, 0.6)',
  },
};

// ─── Timer Ring (SVG Circle Progress) ─────────────────────────────────────────

interface TimerRingProps {
  timeLeft: number;
  totalTime: number;
  color: string;
  glowColor: string;
}

const TimerRing: React.FC<TimerRingProps> = ({ timeLeft, totalTime, color, glowColor }) => {
  const radius = 90;
  const circumference = 2 * Math.PI * radius;
  const progress = totalTime > 0 ? timeLeft / totalTime : 0;
  const offset = circumference * (1 - progress);

  return (
    <div className="timer-ring-wrapper">
      <svg className="timer-ring-svg" width="240" height="240" viewBox="0 0 240 240">
        {/* Background circle */}
        <circle
          cx="120" cy="120" r={radius}
          fill="none"
          stroke="rgba(255,255,255,0.06)"
          strokeWidth="12"
        />
        {/* Glow filter */}
        <defs>
          <filter id="glow">
            <feGaussianBlur stdDeviation="4" result="coloredBlur" />
            <feMerge>
              <feMergeNode in="coloredBlur" />
              <feMergeNode in="SourceGraphic" />
            </feMerge>
          </filter>
        </defs>
        {/* Progress arc */}
        <circle
          cx="120" cy="120" r={radius}
          fill="none"
          stroke={color}
          strokeWidth="12"
          strokeLinecap="round"
          strokeDasharray={circumference}
          strokeDashoffset={offset}
          transform="rotate(-90 120 120)"
          filter="url(#glow)"
          style={{ transition: 'stroke-dashoffset 0.8s cubic-bezier(0.4, 0, 0.2, 1)' }}
        />
      </svg>
      {/* Time text centered inside the ring */}
      <div className="timer-ring-time" style={{ color, textShadow: `0 0 12px ${glowColor}` }}>
        {formatTime(timeLeft)}
      </div>
    </div>
  );
};

// ─── Particle ─────────────────────────────────────────────────────────────────

interface Particle {
  id: number;
  x: number;
  y: number;
  size: number;
  speedX: number;
  speedY: number;
  opacity: number;
}

const useParticles = (count: number): Particle[] => {
  const [particles] = useState<Particle[]>(() =>
    Array.from({ length: count }, (_, i) => ({
      id: i,
      x: Math.random() * 100,
      y: Math.random() * 100,
      size: Math.random() * 3 + 1,
      speedX: (Math.random() - 0.5) * 0.3,
      speedY: (Math.random() - 0.5) * 0.3,
      opacity: Math.random() * 0.5 + 0.1,
    }))
  );
  return particles;
};

// ─── Main Screen Overlay Component ───────────────────────────────────────────

const OverlayTextWindow: React.FC<ScreenOverlayProps> = ({ status, timer, onTimerEnd }) => {
  const [timeLeft, setTimeLeft] = useState<number>(timer?.duration ?? 0);
  const intervalRef = useRef<ReturnType<typeof setInterval> | null>(null);
  const particles = useParticles(30);

  const config = STATUS_CONFIG[status];

  // Reset and start timer whenever status or timer config changes
  useEffect(() => {
    if (intervalRef.current) clearInterval(intervalRef.current);

    if (timer?.enabled && timer.duration > 0) {
      setTimeLeft(timer.duration);

      intervalRef.current = setInterval(() => {
        setTimeLeft(prev => {
          if (prev <= 1) {
            clearInterval(intervalRef.current!);
            onTimerEnd?.();
            return 0;
          }
          return prev - 1;
        });
      }, 1000);
    } else {
      setTimeLeft(0);
    }

    return () => { if (intervalRef.current) clearInterval(intervalRef.current); };
  }, [status, timer?.enabled, timer?.duration]);

  const showTimer = timer?.enabled && timer.duration > 0;

  return (
    <div className="screen-overlay">
      {/* Drifting particles */}
      {particles.map(p => (
        <div
          key={p.id}
          className="screen-particle"
          style={{
            left: `${p.x}%`,
            top: `${p.y}%`,
            width: p.size,
            height: p.size,
            opacity: p.opacity,
            background: config.color,
            boxShadow: `0 0 ${p.size * 2}px ${config.color}`,
            animationDuration: `${8 + p.size * 3}s`,
            animationDelay: `${p.id * 0.2}s`,
          }}
        />
      ))}

      {/* Content card */}
      <div className="screen-content" style={{ borderColor: config.color, boxShadow: `0 0 40px ${config.glowColor}, inset 0 0 40px ${config.glowColor.replace('0.6', '0.05')}` }}>
        {/* Scan line */}
        <div className="screen-scanline" />

        {/* Corner decorations */}
        <div className="screen-corner screen-corner-tl" style={{ borderColor: config.color, boxShadow: `-6px -6px 12px ${config.glowColor}` }} />
        <div className="screen-corner screen-corner-tr" style={{ borderColor: config.color, boxShadow: `6px -6px 12px ${config.glowColor}` }} />
        <div className="screen-corner screen-corner-bl" style={{ borderColor: config.color, boxShadow: `-6px 6px 12px ${config.glowColor}` }} />
        <div className="screen-corner screen-corner-br" style={{ borderColor: config.color, boxShadow: `6px 6px 12px ${config.glowColor}` }} />

        {/* Timer ring or status icon */}
        {showTimer ? (
          <TimerRing timeLeft={timeLeft} totalTime={timer.duration} color={config.color} glowColor={config.glowColor} />
        ) : (
          <div className="screen-icon" style={{ color: config.color, textShadow: `0 0 20px ${config.glowColor}` }}>
            {status === 'starting' && '▶'}
            {status === 'brb' && '⏳'}
            {status === 'ended' && '■'}
          </div>
        )}

        {/* Title */}
        <h1 className="screen-title" style={{ color: config.color, textShadow: `0 0 10px ${config.glowColor}, 0 0 30px ${config.glowColor}` }}>
          {config.title}
        </h1>

        {/* Subtitle */}
        <p className="screen-subtitle">{config.subtitle}</p>

        {/* Divider */}
        <div className="screen-divider" style={{ background: `linear-gradient(90deg, transparent, ${config.color}, transparent)` }} />

        {/* Timer text fallback when timer is on */}
        {showTimer && (
          <p className="screen-timer-label" style={{ color: config.color }}>
            {timeLeft > 0 ? 'RETURNING IN' : 'TIME\'S UP'}
          </p>
        )}
      </div>
    </div>
  );
};

export default OverlayTextWindow;