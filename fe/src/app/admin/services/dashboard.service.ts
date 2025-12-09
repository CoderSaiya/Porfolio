import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

export interface DashboardStats {
    totalViews: number;
    totalProjects: number;
    totalBlogs: number;
    totalMessages: number;
}

export interface DashboardChartData {
    label: string;
    value: number;
}

export interface DashboardActivity {
    id: string;
    type: 'blog' | 'project' | 'message';
    title: string;
    description: string;
    time: string;
    icon: string;
}

export interface DashboardResponse {
    stats: DashboardStats;
    chartData: DashboardChartData[];
    recentActivities: DashboardActivity[];
}

@Injectable({
    providedIn: 'root'
})
export class DashboardService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}/api/admin/dashboard`;

    getDashboardData(): Observable<DashboardResponse> {
        return this.http.get<DashboardResponse>(this.apiUrl);
    }
}
