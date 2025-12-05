import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateProjectRequest, ProjectAdmin, UpdateProjectRequest } from '../../core/models/project-admin.model';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ProjectAdminService {
    private http = inject(HttpClient);
    private readonly API_URL = `${environment.apiUrl}/api/admin/projects`;

    getAllProjects(): Observable<ProjectAdmin[]> {
        return this.http.get<ProjectAdmin[]>(`${environment.apiUrl}/api/portfolio/projects`, {
            withCredentials: true
        });
    }

    createProject(project: CreateProjectRequest): Observable<ProjectAdmin> {
        return this.http.post<ProjectAdmin>(this.API_URL, project, {
            withCredentials: true
        });
    }

    updateProject(id: string, project: UpdateProjectRequest): Observable<ProjectAdmin> {
        return this.http.put<ProjectAdmin>(`${this.API_URL}/${id}`, project, {
            withCredentials: true
        });
    }

    deleteProject(id: string): Observable<{ message: string }> {
        return this.http.delete<{ message: string }>(`${this.API_URL}/${id}`, {
            withCredentials: true
        });
    }

    uploadImage(id: string, file: File, variant: string = 'thumb'): Observable<{ message: string; imageUrl: string }> {
        const formData = new FormData();
        formData.append('file', file);

        return this.http.post<{ message: string; imageUrl: string }>(
            `${this.API_URL}/${id}/image?variant=${variant}`,
            formData,
            { withCredentials: true }
        );
    }
}
