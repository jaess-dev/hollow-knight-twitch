import './SocialBar.css';

interface Social {
    platform: string;
    icon: string;
    username: string;
    url: string;
    className: string;
}

interface SocialItemProps extends Social { }

export const SocialBar: React.FC = () => {
    const socials: Social[] = [
        { platform: 'Instagram', icon: '📷', username: '@JaessDev', url: 'https://instagram.com/JaessDev', className: 'instagram' },
        { platform: 'YouTube', icon: '▶️', username: '@JaessDev-Gaming', url: 'https://youtube.com/@JaessDev', className: 'youtube' },
        { platform: 'TikTok', icon: '🎵', username: '@JaessDev', url: 'https://tiktok.com/@JaessDev', className: 'tiktok' }
    ];

    return (
        <div className="social-bar">
            {socials.map((social, index) => (
                <SocialItem key={index} {...social} />
            ))}
        </div>
    );
};

const SocialItem: React.FC<SocialItemProps> = ({ platform, icon, username, url, className }) => (
    <div className="social-item" onClick={() => window.open(url, '_blank')}>
        <div className={`social-icon ${className}`}>{icon}</div>
        <div className="social-handle">
            <div className="social-label">{platform}</div>
            <div className="social-username">{username}</div>
        </div>
    </div>
);