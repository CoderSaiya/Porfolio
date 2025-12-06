import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface BlogCategory {
    id: string;
    name: string;
    slug: string;
    description?: string;
    order: number;
}

export interface CreateCategoryDTO {
    name: string;
    slug: string;
    description?: string;
    order: number;
}

@Injectable({
    providedIn: 'root'
})
export class BlogCategoryService {
    private apiUrl = `${environment.apiUrl}/api/admin/blogs/categories`;

    constructor(private http: HttpClient) { }

    getCategories(): Observable<BlogCategory[]> {
        return this.http.get<BlogCategory[]>(this.apiUrl, { withCredentials: true });
    }

    createCategory(data: CreateCategoryDTO): Observable<BlogCategory> {
        return this.http.post<BlogCategory>(this.apiUrl, data, { withCredentials: true });
    }

    updateCategory(id: string, data: CreateCategoryDTO): Observable<BlogCategory> {
        return this.http.put<BlogCategory>(`${this.apiUrl}/${id}`, data, { withCredentials: true });
    }

    deleteCategory(id: string): Observable<{ message: string }> {
        return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`, { withCredentials: true });
    }
}
