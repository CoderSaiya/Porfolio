import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { LucideAngularModule, Menu, X, Code, User, Briefcase, Mail, Sun, Moon, Coffee, BookOpen, LogIn } from 'lucide-angular';
import { ThemeService } from '../../../core/services/theme.service';

interface NavItem {
  id: string;
  label: string;
  icon: string;
  type?: 'scroll' | 'route';
  route?: string;
}

@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss'],
})
export class NavigationComponent {
  isOpen = false;
  activeSection: string = 'hero';

  navItems: NavItem[] = [
    { id: 'hero', label: 'Trang chủ', icon: 'Code', type: 'scroll' },
    { id: 'about', label: 'Giới thiệu', icon: 'User', type: 'scroll' },
    { id: 'skills', label: 'Kỹ năng', icon: 'Briefcase', type: 'scroll' },
    { id: 'projects', label: 'Dự án', icon: 'Briefcase', type: 'scroll' },
    { id: 'blogs', label: 'Blog', icon: 'BookOpen', type: 'scroll' },
    { id: 'contact', label: 'Liên hệ', icon: 'Mail', type: 'scroll' },
  ];

  routeItems: NavItem[] = [
    { id: 'login', label: 'Đăng nhập', icon: 'LogIn', type: 'route', route: '/login' },
  ];

  constructor(
    public themeService: ThemeService,
    private router: Router
  ) { }

  @HostListener('window:scroll')
  onScroll() {
    if (typeof document === 'undefined') return;
    const pos = window.scrollY + 100;
    for (const item of this.navItems) {
      const el = document.getElementById(item.id);
      if (!el) continue;
      const rect = el.getBoundingClientRect();
      const top = window.scrollY + rect.top;
      if (pos >= top && pos < top + rect.height) { this.activeSection = item.id; break; }
    }
  }

  scrollTo(id: string) {
    if (typeof document === 'undefined') return;
    document.getElementById(id)?.scrollIntoView({ behavior: 'smooth' });
    this.isOpen = false;
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
    this.isOpen = false;
  }

  handleNavClick(item: NavItem) {
    if (item.type === 'route' && item.route) {
      this.navigateTo(item.route);
    } else {
      this.scrollTo(item.id);
    }
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }
}
