import { Injectable, signal, effect } from '@angular/core';

export type Theme = 'dark' | 'light';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'portfolio-theme';
  
  // Signal to track current theme
  theme = signal<Theme>(this.getInitialTheme());

  constructor() {
    // Effect to apply theme changes to document
    effect(() => {
      const currentTheme = this.theme();
      document.documentElement.setAttribute('data-theme', currentTheme);
      localStorage.setItem(this.THEME_KEY, currentTheme);
    });
  }

  /**
   * Get initial theme from localStorage or system preference
   */
  private getInitialTheme(): Theme {
    // Check localStorage first
    const stored = localStorage.getItem(this.THEME_KEY) as Theme | null;
    if (stored === 'dark' || stored === 'light') {
      return stored;
    }

    // Check system preference
    if (typeof window !== 'undefined' && window.matchMedia) {
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      return prefersDark ? 'dark' : 'light';
    }

    // Default to dark
    return 'dark';
  }

  /**
   * Toggle between dark and light themes
   */
  toggleTheme(): void {
    this.theme.update(current => current === 'dark' ? 'light' : 'dark');
  }

  /**
   * Set theme explicitly
   */
  setTheme(theme: Theme): void {
    this.theme.set(theme);
  }

  /**
   * Get current theme value
   */
  getCurrentTheme(): Theme {
    return this.theme();
  }

  /**
   * Check if current theme is dark
   */
  isDark(): boolean {
    return this.theme() === 'dark';
  }
}
