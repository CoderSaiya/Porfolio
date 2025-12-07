import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { LucideAngularModule } from 'lucide-angular';
import { environment } from '../../../environments/environment';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, LucideAngularModule, RouterModule],
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
    private fb = inject(FormBuilder);
    private authService = inject(AuthService);
    private router = inject(Router);
    private route = inject(ActivatedRoute);

    apiUrl = environment.apiUrl;

    loginForm!: FormGroup;
    twoFAForm!: FormGroup;
    loading = false;
    errorMessage: string | null = null;
    requiresTwoFactor = false;
    tempToken: string | null = null;
    registered = false;

    ngOnInit() {
        if (this.route.snapshot.queryParams['registered']) {
            this.registered = true;
        }

        this.loginForm = this.fb.group({
            username: ['', Validators.required],
            password: ['', Validators.required]
        });

        this.twoFAForm = this.fb.group({
            code: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
        });
    }

    onLogin() {
        if (this.loginForm.invalid) return;

        this.loading = true;
        this.errorMessage = null;

        this.authService.login(this.loginForm.value).subscribe({
            next: (response) => {
                if (response.requiresTwoFactor) {
                    this.requiresTwoFactor = true;
                    this.tempToken = response.tempToken;
                } else {
                    this.navigateToReturnUrl(response.role);
                }
                this.loading = false;
            },
            error: (err) => {
                this.errorMessage = 'Tên đăng nhập hoặc mật khẩu không đúng';
                this.loading = false;
            }
        });
    }

    onVerify2FA() {
        if (this.twoFAForm.invalid || !this.tempToken) return;

        this.loading = true;
        this.errorMessage = null;

        this.authService.verify2FA(this.twoFAForm.value.code, this.tempToken).subscribe({
            next: () => {
                this.authService.currentUser$.subscribe(user => {
                    if (user) {
                        this.navigateToReturnUrl(user.role);
                    }
                });
            },
            error: (err) => {
                this.errorMessage = 'Mã xác thực không đúng';
                this.loading = false;
            }
        });
    }

    cancelTwoFactor() {
        this.requiresTwoFactor = false;
        this.tempToken = null;
        this.twoFAForm.reset();
        this.errorMessage = null;
    }

    loginWith(provider: string) {
        window.location.href = `${this.apiUrl}/api/auth/external/${provider}/start`;
    }

    private navigateToReturnUrl(role: string) {
        let defaultUrl = '/';
        if (role === 'Admin') {
            defaultUrl = '/admin';
        }

        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || defaultUrl;
        this.router.navigate([returnUrl]);
    }
}
