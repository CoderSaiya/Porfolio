import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-admin-layout',
    standalone: true,
    imports: [CommonModule, RouterOutlet],
    template: `
    <div class="admin-layout">
      <aside class="admin-sidebar">
        <h2>Admin Panel</h2>
        <p>Sidebar will be implemented</p>
      </aside>
      <main class="admin-content">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
    styles: [`
    .admin-layout {
      display: flex;
      min-height: 100vh;
      background: var(--background);
    }

    .admin-sidebar {
      width: 250px;
      background: var(--card);
      border-right: 1px solid var(--border);
      padding: 2rem;

      h2 {
        color: var(--accent);
        margin: 0 0 1rem 0;
      }

      p {
        color: var(--foreground-secondary);
      }
    }

    .admin-content {
      flex: 1;
      padding: 2rem;
    }
  `]
})
export class AdminLayoutComponent { }
