import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
    SkillCategoryAdmin,
    CreateSkillCategoryRequest,
    UpdateSkillCategoryRequest,
    ReorderCategoriesRequest
} from '../../core/models/skill-admin.model';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class SkillAdminService {
    private http = inject(HttpClient);
    private readonly API_URL = `${environment.apiUrl}/api/admin/skills`;

    getAllCategories(): Observable<SkillCategoryAdmin[]> {
        return this.http.get<SkillCategoryAdmin[]>(`${environment.apiUrl}/api/portfolio/skills`, {
            withCredentials: true
        });
    }

    createCategory(category: CreateSkillCategoryRequest): Observable<SkillCategoryAdmin> {
        return this.http.post<SkillCategoryAdmin>(`${this.API_URL}/categories`, category, {
            withCredentials: true
        });
    }

    updateCategory(id: string, category: UpdateSkillCategoryRequest): Observable<SkillCategoryAdmin> {
        return this.http.put<SkillCategoryAdmin>(`${this.API_URL}/categories/${id}`, category, {
            withCredentials: true
        });
    }

    deleteCategory(id: string): Observable<{ message: string }> {
        return this.http.delete<{ message: string }>(`${this.API_URL}/categories/${id}`, {
            withCredentials: true
        });
    }

    reorderCategories(request: ReorderCategoriesRequest): Observable<{ message: string }> {
        return this.http.post<{ message: string }>(`${this.API_URL}/categories/reorder`, request, {
            withCredentials: true
        });
    }
}
