import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { LucideAngularModule, Shield, User, LogOut } from 'lucide-angular';
import { Setup2FAResponse, AuthUser } from '../../core/models/auth.model';

@Component({
    selector: 'app-settings',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
    styleUrls: ['./settings.component.scss'],
    templateUrl: './settings.component.html'
})
export class SettingsComponent implements OnInit {
    private authService = inject(AuthService);
    private fb = inject(FormBuilder);

    user: AuthUser | null = null;
    is2FAEnabled = false;

    showSetup = false;
    setupData: Setup2FAResponse | null = null;
    recoveryCodes: string[] = [];
    loading = false;

    verifyForm = this.fb.group({
        code: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
    });

    ngOnInit() {
        this.authService.currentUser$.subscribe(u => {
            if (u) {
                this.user = u;
                this.is2FAEnabled = !!u.twoFactorEnabled;
            }
        });
    }

    toggle2FA() {
        if (this.is2FAEnabled) {
        } else {
            this.startSetup();
        }
    }

    startSetup() {
        this.loading = true;
        this.authService.setup2FA().subscribe({
            next: (res) => {
                this.setupData = res;
                this.showSetup = true;
                this.loading = false;
            },
            error: (err) => {
                console.error(err);
                this.loading = false;
            }
        });
    }

    enable2FA() {
        if (this.verifyForm.invalid || !this.setupData) return;

        this.loading = true;
        this.authService.enable2FA(
            this.verifyForm.value.code!,
            this.setupData.secret,
            this.setupData.recoveryCodes
        ).subscribe({
            next: () => {
                this.is2FAEnabled = true;
                this.showSetup = false;
                this.recoveryCodes = this.setupData!.recoveryCodes;
                this.loading = false;
            },
            error: (err) => {
                console.error(err);
                this.loading = false;
                // Show error message
            }
        });
    }

    logout() {
        this.authService.logout().subscribe();
    }
}
