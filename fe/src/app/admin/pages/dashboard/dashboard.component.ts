import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [CommonModule],
    template: `
    <div class="dashboard">
      <h1>Admin <span class="accent">Dashboard</span></h1>
      <p>Welcome to the admin panel. This is a placeholder for the dashboard.</p>
    </div>
  `,
    styles: [`
    .dashboard {
      h1 {
        font-size: 2rem;
        font-weight: 700;
        margin: 0 0 1rem 0;
        color: var(--foreground);
      }

      p {
        color: var(--foreground-secondary);
      }
    }
  `]
})
export class DashboardComponent { }
