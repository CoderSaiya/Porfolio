import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { LucideAngularModule } from 'lucide-angular';

@Component({
    selector: 'app-admin-layout',
    standalone: true,
    imports: [CommonModule, RouterModule, LucideAngularModule],
    templateUrl: './admin-layout.component.html',
    styleUrls: ['./admin-layout.component.scss']
})
export class AdminLayoutComponent {
    private authService = inject(AuthService);

    menuItems = [
        { label: 'Dashboard', icon: 'LayoutDashboard', route: '/admin/dashboard' },
        { label: 'Projects', icon: 'FolderGit2', route: '/admin/projects' },
        { label: 'Skills', icon: 'Code', route: '/admin/skills' },
        { label: 'Messages', icon: 'Mail', route: '/admin/messages' },
        { label: 'Profile', icon: 'User', route: '/admin/profile' }
    ];

    logout() {
        this.authService.logout().subscribe();
    }
}
