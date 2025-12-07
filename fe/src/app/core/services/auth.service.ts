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

    private currentUserSubject = new BehaviorSubject<AuthUser | null>(null);
    public currentUser$ = this.currentUserSubject.asObservable();
    public isAuthenticated$ = this.currentUser$.pipe(map(user => !!user));

    constructor() {
        // Try to restore session on init
        this.checkAuthStatus().subscribe();
    }

    login(credentials: LoginRequest): Observable<LoginResponse> {
        return this.http.post<LoginResponse>(`${this.API_URL}/login`, credentials, {
            withCredentials: true
        }).pipe(
            tap(res => {
                if (!res.requiresTwoFactor) {
                    this.currentUserSubject.next({
                        username: res.username,
                        role: res.role
                    });
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
                this.currentUserSubject.next(null);
                this.router.navigate(['/login']);
            }),
            catchError(() => {
                // Clear user even if request fails
                this.currentUserSubject.next(null);
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

    // Check if user is authenticated by attempting a protected request
    private checkAuthStatus(): Observable<AuthUser | null> {
        return this.http.get<AuthUser>(`${this.API_URL}/me`, {
            withCredentials: true
        }).pipe(
            tap(user => {
                this.currentUserSubject.next(user);
            }),
            catchError(() => {
                this.currentUserSubject.next(null);
                return of(null);
            })
        );
    }

    getCurrentUser(): AuthUser | null {
        return this.currentUserSubject.value;
    }

    setCurrentUser(user: AuthUser | null): void {
        this.currentUserSubject.next(user);
    }
}
