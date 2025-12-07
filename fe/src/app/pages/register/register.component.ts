import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { LucideAngularModule } from 'lucide-angular';

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule, LucideAngularModule],
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
    private fb = inject(FormBuilder);
    private authService = inject(AuthService);
    private router = inject(Router);

    registerForm = this.fb.group({
        username: ['', [Validators.required, Validators.minLength(3)]],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', [Validators.required]],
        fullName: ['']
    }, { validators: this.passwordMatchValidator });

    loading = false;
    errorMessage: string | null = null;

    passwordMatchValidator(g: any) {
        return g.get('password').value === g.get('confirmPassword').value
            ? null : { mismatch: true };
    }

    onSubmit() {
        if (this.registerForm.invalid) return;

        this.loading = true;
        this.errorMessage = null;

        const { confirmPassword, ...registerData } = this.registerForm.value;

        this.authService.register(registerData).subscribe({
            next: () => {
                this.router.navigate(['/login'], { queryParams: { registered: 'true' } });
            },
            error: (err) => {
                this.loading = false;
                this.errorMessage = err.error?.message || 'Registration failed';
            }
        });
    }
}
