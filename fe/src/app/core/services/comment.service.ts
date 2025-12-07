import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CommentResponse {
    id: string;
    content: string;
    userId: string;
    userName: string;
    userAvatar?: string;
    createdAt: Date;
    parentId?: string;
    replies: CommentResponse[];
    likesCount: number;
    isLikedByCurrentUser: boolean;
}

export interface CreateCommentRequest {
    blogPostId: string;
    content: string;
    parentId?: string;
}

@Injectable({
    providedIn: 'root'
})
export class CommentService {
    private apiUrl = `${environment.apiUrl}/api/comments`;

    constructor(private http: HttpClient) { }

    getComments(blogId: string): Observable<CommentResponse[]> {
        return this.http.get<CommentResponse[]>(`${this.apiUrl}/${blogId}`);
    }

    addComment(request: CreateCommentRequest): Observable<CommentResponse> {
        return this.http.post<CommentResponse>(this.apiUrl, request);
    }

    deleteComment(commentId: string): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${commentId}`);
    }
}
