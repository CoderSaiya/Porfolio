import { Component, OnInit, inject } from '@angular/core';
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
export class LoginComponent implements OnInit {
    private fb = inject(FormBuilder);
    private authService = inject(AuthService);
    private router = inject(Router);
    private route = inject(ActivatedRoute);

    loginForm!: FormGroup;
    twoFAForm!: FormGroup;
    loading = false;
    errorMessage: string | null = null;
    requiresTwoFactor = false;
    tempToken: string | null = null;

    ngOnInit() {
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
                    this.navigateToReturnUrl();
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
                this.navigateToReturnUrl();
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

    private navigateToReturnUrl() {
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/admin';
        this.router.navigate([returnUrl]);
    }
}
