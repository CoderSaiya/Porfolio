import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap, catchError, of, map, switchMap } from 'rxjs';
import { Router } from '@angular/router';
import { LoginRequest, LoginResponse, Verify2FARequest, Setup2FAResponse, AuthUser } from '../models/auth.model';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private http = inject(HttpClient);
    private router = inject(Router);

    private readonly API_URL = `${environment.apiUrl}/api/auth`;
    private readonly USER_STORAGE_KEY = 'currentUser';

    private currentUserSubject = new BehaviorSubject<AuthUser | null>(this.loadFromStorage());
    public currentUser$ = this.currentUserSubject.asObservable();
    public isAuthenticated$ = this.currentUser$.pipe(map(user => !!user));

    constructor() {
        // Verify user silently in background if exists in storage
        if (this.currentUserSubject.value) {
            this.verifySession().subscribe();
        }
    }

    login(credentials: LoginRequest): Observable<LoginResponse> {
        return this.http.post<LoginResponse>(`${this.API_URL}/login`, credentials, {
            withCredentials: true
        }).pipe(
            tap(res => {
                if (!res.requiresTwoFactor) {
                    const user: AuthUser = {
                        id: res.id,
                        username: res.username,
                        role: res.role,
                        fullName: res.fullName,
                        avatarUrl: res.avatarUrl
                    };
                    this.setCurrentUser(user);
                }
            })
        );
    }

    register(data: any): Observable<any> {
        return this.http.post(`${this.API_URL}/register`, data);
    }

    verify2FA(code: string, tempToken: string): Observable<any> {
        return this.http.post(`${this.API_URL}/verify-2fa?tempToken=${tempToken}`,
            { code } as Verify2FARequest,
            { withCredentials: true }
        ).pipe(
            // After successful 2FA, get user info
            switchMap(() => this.checkAuthStatus())
        );
    }

    logout(): Observable<any> {
        return this.http.post(`${this.API_URL}/logout`, {}, {
            withCredentials: true
        }).pipe(
            tap(() => {
                this.clearUser();
                this.router.navigate(['/login']);
            }),
            catchError(() => {
                // Clear user even if request fails
                this.clearUser();
                this.router.navigate(['/login']);
                return of(null);
            })
        );
    }

    refreshToken(): Observable<any> {
        return this.http.post(`${this.API_URL}/refresh`, {}, {
            withCredentials: true
        });
    }

    setup2FA(): Observable<Setup2FAResponse> {
        return this.http.get<Setup2FAResponse>(`${this.API_URL}/2fa/setup`, {
            withCredentials: true
        });
    }

    enable2FA(code: string, secret: string, recoveryCodes: string[]): Observable<any> {
        const codesString = recoveryCodes.join(',');
        return this.http.post(
            `${this.API_URL}/2fa/enable?secret=${secret}&recoveryCodes=${encodeURIComponent(codesString)}`,
            { code } as Verify2FARequest,
            { withCredentials: true }
        );
    }

    disable2FA(code: string): Observable<any> {
        return this.http.post(`${this.API_URL}/2fa/disable`,
            { code } as Verify2FARequest,
            { withCredentials: true }
        );
    }

    // Verify session silently in background (called on app init)
    private verifySession(): Observable<AuthUser | null> {
        return this.http.get<AuthUser>(`${this.API_URL}/me`, {
            withCredentials: true
        }).pipe(
            tap(user => {
                // Update with fresh data from server
                this.setCurrentUser(user);
            }),
            catchError(() => {
                // If /me fails, try to refresh token
                return this.refreshToken().pipe(
                    switchMap(() => this.http.get<AuthUser>(`${this.API_URL}/me`, { withCredentials: true })),
                    tap(user => this.setCurrentUser(user)),
                    catchError(() => {
                        console.warn('Session verification failed, but keeping user from localStorage');
                        return of(null);
                    })
                );
            })
        );
    }

    // Check authentication status (used after 2FA)
    private checkAuthStatus(): Observable<AuthUser | null> {
        return this.http.get<AuthUser>(`${this.API_URL}/me`, {
            withCredentials: true
        }).pipe(
            tap(user => {
                this.setCurrentUser(user);
            }),
            catchError(() => {
                // If /me fails (e.g. 401), try to refresh token
                return this.refreshToken().pipe(
                    switchMap(() => this.http.get<AuthUser>(`${this.API_URL}/me`, { withCredentials: true })),
                    tap(user => this.setCurrentUser(user)),
                    catchError(() => {
                        this.clearUser();
                        return of(null);
                    })
                );
            })
        );
    }

    getCurrentUser(): AuthUser | null | undefined {
        return this.currentUserSubject.value;
    }

    setCurrentUser(user: AuthUser | null): void {
        this.currentUserSubject.next(user);
        if (user) {
            this.saveToStorage(user);
        }
    }

    private clearUser(): void {
        this.currentUserSubject.next(null);
        this.removeFromStorage();
    }

    private saveToStorage(user: AuthUser): void {
        try {
            localStorage.setItem(this.USER_STORAGE_KEY, JSON.stringify(user));
        } catch (error) {
            console.error('Error saving user to localStorage:', error);
        }
    }

    private loadFromStorage(): AuthUser | null {
        try {
            const userJson = localStorage.getItem(this.USER_STORAGE_KEY);
            return userJson ? JSON.parse(userJson) : null;
        } catch (error) {
            console.error('Error loading user from localStorage:', error);
            return null;
        }
    }

    private removeFromStorage(): void {
        try {
            localStorage.removeItem(this.USER_STORAGE_KEY);
        } catch (error) {
            console.error('Error removing user from localStorage:', error);
        }
    }
}
