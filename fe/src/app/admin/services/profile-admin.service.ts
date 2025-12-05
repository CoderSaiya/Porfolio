import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface SocialLinkDTO {
    platform: number; // Enum value
    url: string;
    order: number;
}

export interface UpdateProfileDTO {
    fullName?: string;
    headline?: string;
    location?: string;
    about?: string;
    yearsExperience?: number;
    projectsCompleted?: number;
    coffees?: number;
    socialLinks?: SocialLinkDTO[];
}

export interface Profile {
    id: string;
    fullName: string;
    headline: string;
    location: string;
    avatarUrl?: string;
    about?: string;
    yearsExperience: number;
    projectsCompleted: number;
    coffees: number;
    socialLinks: any[]; // Adjust type if needed
}

@Injectable({
    providedIn: 'root'
})
export class ProfileAdminService {
    private apiUrl = `${environment.apiUrl}/admin/profile`;

    constructor(private http: HttpClient) { }

    getProfile(): Observable<Profile> {
        return this.http.get<Profile>(this.apiUrl);
    }

    updateProfile(data: UpdateProfileDTO): Observable<Profile> {
        return this.http.put<Profile>(this.apiUrl, data);
    }

    uploadAvatar(file: File): Observable<{ url: string }> {
        const formData = new FormData();
        formData.append('file', file);
        return this.http.post<{ url: string }>(`${this.apiUrl}/avatar`, formData);
    }
}
