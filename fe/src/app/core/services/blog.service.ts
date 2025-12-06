import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface BlogPost {
    id: string;
    title: string;
    slug: string;
    summary: string;
    content: string;
    featuredImage?: string;
    categoryId?: string;
    tags: string[];
    published: boolean;
    publishedAt?: string;
    viewCount: number;
    createdAt: string;
}

export interface BlogCategory {
    id: string;
    name: string;
    slug: string;
    description?: string;
}

@Injectable({
    providedIn: 'root'
})
export class BlogService {
    private apiUrl = `${environment.apiUrl}/api/blog`;

    constructor(private http: HttpClient) { }

    getPosts(page: number = 1, pageSize: number = 10, categoryId?: string): Observable<{ data: BlogPost[], total: number }> {
        const params: any = { page, pageSize };
        if (categoryId) params.categoryId = categoryId;

        return this.http.get<{ data: BlogPost[], total: number }>(`${this.apiUrl}/posts`, { params });
    }

    getPostBySlug(slug: string): Observable<BlogPost> {
        return this.http.get<BlogPost>(`${this.apiUrl}/posts/${slug}`);
    }

    getCategories(): Observable<BlogCategory[]> {
        return this.http.get<BlogCategory[]>(`${this.apiUrl}/categories`);
    }
}
