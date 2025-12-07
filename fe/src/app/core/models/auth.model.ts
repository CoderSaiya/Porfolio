export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    id: string;
    requiresTwoFactor: boolean;
    tempToken: string | null;
    username: string;
    role: string;
    fullName?: string;
    avatarUrl?: string;
}

export interface Verify2FARequest {
    code: string;
}

export interface Setup2FAResponse {
    secret: string;
    qrCodeDataUrl: string;
    recoveryCodes: string[];
}

export interface AuthUser {
    id: string;
    username: string;
    role: string;
    fullName?: string;
    avatarUrl?: string;
    twoFactorEnabled?: boolean;
}
