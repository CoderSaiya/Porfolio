import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    return authService.isAuthenticated$.pipe(
        map(isAuth => {
            if (isAuth) {
                return true;
            }

            // Redirect to login with return URL
            router.navigate(['/login'], {
                queryParams: { returnUrl: state.url }
            });
            return false;
        })
    );
};
