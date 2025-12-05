import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export enum MessageStatus {
    Unread = 0,
    Read = 1
}

export interface ContactMessage {
    id: string;
    name: string;
    email: string;
    subject: string;
    message: string;
    status: MessageStatus;
    createdAt: string;
    readAt?: string;
}

export interface MessageFilterDTO {
    status?: MessageStatus;
    fromDate?: string;
    toDate?: string;
    page: number;
    pageSize: number;
    searchTerm?: string;
}

export interface PaginatedResult<T> {
    data: T[];
    total: number;
    page: number;
    pageSize: number;
}

@Injectable({
    providedIn: 'root'
})
export class MessageAdminService {
    private apiUrl = `${environment.apiUrl}/admin/messages`;

    constructor(private http: HttpClient) { }

    getMessages(filter: MessageFilterDTO): Observable<PaginatedResult<ContactMessage>> {
        let params = new HttpParams()
            .set('page', filter.page.toString())
            .set('pageSize', filter.pageSize.toString());

        if (filter.status !== undefined && filter.status !== null) {
            params = params.set('status', filter.status.toString());
        }
        if (filter.searchTerm) {
            params = params.set('searchTerm', filter.searchTerm);
        }
        if (filter.fromDate) {
            params = params.set('fromDate', filter.fromDate);
        }
        if (filter.toDate) {
            params = params.set('toDate', filter.toDate);
        }

        return this.http.get<PaginatedResult<ContactMessage>>(this.apiUrl, { params });
    }

    getMessage(id: string): Observable<ContactMessage> {
        return this.http.get<ContactMessage>(`${this.apiUrl}/${id}`);
    }

    updateStatus(id: string, status: MessageStatus): Observable<void> {
        return this.http.put<void>(`${this.apiUrl}/${id}/status`, { status });
    }

    deleteMessage(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    bulkDelete(ids: string[]): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/bulk`, { body: { ids } });
    }
}
