import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule, Menu, X, Code, User, Briefcase, Mail } from 'lucide-angular';

interface NavItem { id: string; label: string; icon: string; }

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
    { id: 'hero', label: 'Trang chủ', icon: 'Code' },
    { id: 'about', label: 'Giới thiệu', icon: 'User' },
    { id: 'skills', label: 'Kỹ năng', icon: 'Briefcase' },
    { id: 'projects', label: 'Dự án', icon: 'Briefcase' },
    { id: 'contact', label: 'Liên hệ', icon: 'Mail' },
  ];

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
}
