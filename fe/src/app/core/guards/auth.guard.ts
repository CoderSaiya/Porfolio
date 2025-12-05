import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { map, take } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    return authService.isAuthenticated$.pipe(
        take(1),
        map(isAuth => {
            if (isAuth) {
                return true;
            }

            router.navigate(['/login'], {
                queryParams: { returnUrl: state.url }
            });
            return false;
        })
    );
};
