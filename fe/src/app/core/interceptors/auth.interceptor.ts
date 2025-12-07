import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router);
    const authService = inject(AuthService);

    const authReq = req.clone({
        withCredentials: true
    });

    return next(authReq).pipe(
        catchError((error: HttpErrorResponse) => {
            if (error.status === 401) {
                // Ignore 401 for /me and /refresh to allow AuthService to handle auto-refresh/null user
                if (req.url.includes('/me') || req.url.includes('/refresh')) {
                    return throwError(() => error);
                }

                // For other requests, clear auth and redirect
                authService.setCurrentUser(null);
                router.navigate(['/login']);
            }
            return throwError(() => error);
        })
    );
};
