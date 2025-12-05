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
        { label: 'Tổng quan', icon: 'LayoutDashboard', route: '/admin/dashboard' },
        { label: 'Dự án', icon: 'FolderGit2', route: '/admin/projects' },
        { label: 'Kỹ năng', icon: 'Code', route: '/admin/skills' },
        { label: 'Tin nhắn', icon: 'Mail', route: '/admin/messages' },
        { label: 'Hồ sơ', icon: 'User', route: '/admin/profile' }
    ];

    logout() {
        this.authService.logout().subscribe();
    }
}
