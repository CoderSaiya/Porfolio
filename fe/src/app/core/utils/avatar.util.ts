export function getInitials(name: string | undefined | null): string {
    if (!name || name.trim() === '') return '?';

    const parts = name.trim().split(/\s+/);

    if (parts.length === 1) {
        // Single name: take first character
        return parts[0].charAt(0).toUpperCase();
    }

    // Multiple names: take first char of first and last name
    const firstInitial = parts[0].charAt(0);
    const lastInitial = parts[parts.length - 1].charAt(0);

    return (firstInitial + lastInitial).toUpperCase();
}

export function getAvatarColor(str: string | undefined | null): string {
    if (!str) return 'hsl(200, 50%, 50%)'; // Default blue

    // Generate hash from string
    let hash = 0;
    for (let i = 0; i < str.length; i++) {
        hash = str.charCodeAt(i) + ((hash << 5) - hash);
    }

    // Generate hue (0-360) from hash
    const hue = Math.abs(hash % 360);

    // Use fixed saturation and lightness for dark theme
    return `hsl(${hue}, 60%, 50%)`;
}

export function generateAvatarDataUrl(name: string | undefined | null, size: number = 40): string {
    const initials = getInitials(name);
    const bgColor = getAvatarColor(name);

    const canvas = document.createElement('canvas');
    canvas.width = size;
    canvas.height = size;
    const ctx = canvas.getContext('2d');

    if (!ctx) return '';

    // Draw background
    ctx.fillStyle = bgColor;
    ctx.fillRect(0, 0, size, size);

    // Draw text
    ctx.fillStyle = '#ffffff';
    ctx.font = `bold ${size * 0.4}px Inter, sans-serif`;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(initials, size / 2, size / 2);

    return canvas.toDataURL();
}
