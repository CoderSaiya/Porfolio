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
    updatedAt: string;
}

export interface CreateBlogDTO {
    title: string;
    slug: string;
    summary: string;
    content: string;
    featuredImage?: string;
    categoryId?: string;
    tags: string[];
    published: boolean;
}

export interface UpdateBlogDTO {
    title?: string;
    slug?: string;
    summary?: string;
    content?: string;
    featuredImage?: string;
    categoryId?: string;
    tags?: string[];
    published?: boolean;
}

export interface BlogFilter {
    categoryId?: string;
    tags?: string[];
    search?: string;
    published?: boolean;
    page: number;
    pageSize: number;
}

@Injectable({
    providedIn: 'root'
})
export class BlogAdminService {
    private apiUrl = `${environment.apiUrl}/api/admin/blogs`;

    constructor(private http: HttpClient) { }

    getBlogs(filter: BlogFilter): Observable<{ data: BlogPost[], total: number }> {
        const params: any = {
            page: filter.page,
            pageSize: filter.pageSize
        };

        if (filter.search) params.search = filter.search;
        if (filter.categoryId) params.categoryId = filter.categoryId;
        if (filter.published !== undefined) params.published = filter.published;
        if (filter.tags && filter.tags.length > 0) params.tags = filter.tags.join(',');

        return this.http.get<{ data: BlogPost[], total: number }>(this.apiUrl, { params, withCredentials: true });
    }

    getBlog(id: string): Observable<BlogPost> {
        return this.http.get<BlogPost>(`${this.apiUrl}/${id}`, { withCredentials: true });
    }

    createBlog(data: FormData): Observable<BlogPost> {
        return this.http.post<BlogPost>(this.apiUrl, data, { withCredentials: true });
    }

    updateBlog(id: string, data: UpdateBlogDTO): Observable<BlogPost> {
        return this.http.put<BlogPost>(`${this.apiUrl}/${id}`, data, { withCredentials: true });
    }

    updateBlogFormData(id: string, data: FormData): Observable<BlogPost> {
        return this.http.put<BlogPost>(`${this.apiUrl}/${id}`, data, { withCredentials: true });
    }

    deleteBlog(id: string): Observable<{ message: string }> {
        return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`, { withCredentials: true });
    }

    uploadFeaturedImage(id: string, file: File): Observable<{ message: string; imageUrl: string }> {
        const formData = new FormData();
        formData.append('file', file);
        return this.http.post<{ message: string; imageUrl: string }>(
            `${this.apiUrl}/${id}/image`,
            formData,
            { withCredentials: true }
        );
    }
}
