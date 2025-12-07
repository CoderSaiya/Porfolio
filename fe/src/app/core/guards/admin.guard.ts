import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { map, take } from 'rxjs';

export const adminGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    return authService.currentUser$.pipe(
        take(1),
        map(user => {
            if (user && user.role === 'Admin') {
                return true;
            }

            // If logged in but not admin, go home
            if (user) {
                router.navigate(['/']);
                return false;
            }

            // If not logged in, go to login
            router.navigate(['/login'], {
                queryParams: { returnUrl: state.url }
            });
            return false;
        })
    );
};
