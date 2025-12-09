import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LucideAngularModule, LayoutDashboard, FileText, FolderGit2, MessageSquare, TrendingUp, Users, Clock, Plus, ArrowRight, MoreHorizontal, Settings } from 'lucide-angular';
import { DashboardResponse, DashboardService } from '../../services/dashboard.service';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [CommonModule, RouterModule, LucideAngularModule],
    templateUrl: './dashboard.component.html',
    styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
    data = signal<DashboardResponse | null>(null);
    maxValue = 20; // Default max value for chart, can be dynamic based on data

    constructor(private dashboardService: DashboardService) { }

    ngOnInit() {
        this.dashboardService.getDashboardData().subscribe({
            next: (res) => {
                this.data.set(res);
                // Calculate max value for chart scaling
                if (res.chartData?.length) {
                    this.maxValue = Math.max(...res.chartData.map(d => d.value)) * 1.2;
                }
            },
            error: (err) => console.error('Failed to load dashboard data', err)
        });
    }
}
