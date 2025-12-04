import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { LucideAngularModule } from 'lucide-angular';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent {
    private fb = inject(FormBuilder);
    private authService = inject(AuthService);
    private router = inject(Router);
    private route = inject(ActivatedRoute);

    loginForm: FormGroup;
    twoFactorForm: FormGroup;

    loading = false;
    error: string | null = null;
    showTwoFactor = false;
    tempToken: string | null = null;
    username: string = '';

    constructor() {
        this.loginForm = this.fb.group({
            username: ['', [Validators.required]],
            password: ['', [Validators.required]]
        });

        this.twoFactorForm = this.fb.group({
            code: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]]
        });
    }

    onLogin() {
        if (this.loginForm.invalid) return;

        this.loading = true;
        this.error = null;

        const { username, password } = this.loginForm.value;

        this.authService.login({ username, password }).subscribe({
            next: (response) => {
                this.username = response.username;

                if (response.requiresTwoFactor) {
                    this.showTwoFactor = true;
                    this.tempToken = response.tempToken;
                } else {
                    // No 2FA - redirect immediately
                    this.redirectAfterLogin();
                }
                this.loading = false;
            },
            error: (err) => {
                this.error = err.error?.message || 'Login failed. Please check your credentials.';
                this.loading = false;
            }
        });
    }

    onVerify2FA() {
        if (this.twoFactorForm.invalid || !this.tempToken) return;

        this.loading = true;
        this.error = null;

        const { code } = this.twoFactorForm.value;

        this.authService.verify2FA(code, this.tempToken).subscribe({
            next: () => {
                this.redirectAfterLogin();
            },
            error: (err) => {
                this.error = err.error?.message || 'Invalid 2FA code';
                this.loading = false;
            }
        });
    }

    private redirectAfterLogin() {
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/admin';
        this.router.navigate([returnUrl]);
    }
}
