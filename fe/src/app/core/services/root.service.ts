import {inject, Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {environment} from '../../../enviroments/enviroment';
import {map} from 'rxjs';

export interface Profile {
  fullName: string;
  headline: string;
  location: string;
  socialLinks: SocialLink[]
}

interface SocialLink {
  platform: Platform;
  url: string;
}

enum Platform {
  Github = 1,
  Linkedin = 2,
  Twitter = 3,
  Email = 4,
  Website = 5,
  Other = 99
}

export interface SkillCategories {
  title: string;
  icon?: string;
  color?: string;
  skills: SkillItem[];
}

interface SkillItem {
  name: string;
  level: number;
}

export interface Project {
  id: string;
  slug: string;
  title: string;
  description: string;
  highlight: boolean;
  duration? : number | null;
  teamSize? : number | null;
  technologies: string[];
  features: string[];
  thumb?: string | null;
  github?: string | null;
  demo?: string | null;
}

export interface ProjectDetail extends Project {
  image?: string | null;
}


@Injectable({ providedIn: 'root' })
export class RootService {
  private http = inject(HttpClient)
  private apiUrl = `${environment.apiUrl}/api/portfolio`

  getProfile() {
    return this.http
      .get<Profile>(`${this.apiUrl}/profile`)
  }

  getSkills() {
    return this.http
      .get<SkillCategories[]>(`${this.apiUrl}/skills`)
  }

  getProjects(highlight: boolean | null = null, limit: number | null = null) {
    let params = new HttpParams();
    if (highlight !== null) {
      params = params.set('highlight', highlight);
    }
    if (limit !== null) {
      params = params.set('limit', limit.toString());
    }

    return this.http
      .get<Project[]>(`${this.apiUrl}/projects`, { params })
      .pipe(map(list => list.map(p => this.normalizeProject(p))));
  }

  getProjectDetail(slug: string) {
    return this.http
      .get<ProjectDetail>(`${this.apiUrl}/projects/${slug}`)
  }

  getProjectImageUrl(id: string, variant: 'thumb' | 'full' = 'thumb'): string {
    return this.absolutize(`/api/portfolio/projects/${id}/image/${variant}`);
  }

  private normalizeProject(p: Project): Project {
    const thumb = p.thumb && p.thumb.trim()
      ? this.absolutize(p.thumb)
      : this.getProjectImageUrl(p.id, 'thumb');

    return { ...p, thumb };
  }

  private normalizeProjectDetail(p: ProjectDetail): ProjectDetail {
    const base = this.normalizeProject(p);
    const image = p.image && p.image.trim()
      ? this.absolutize(p.image)
      : this.getProjectImageUrl(p.id, 'full');
    return { ...base, image };
  }

  private absolutize(path: string): string {
    if (!path) return '';
    if (/^https?:\/\//i.test(path)) return path; // đã là absolute
    // ghép base API + path tương đối
    return `${environment.apiUrl}${path.startsWith('/') ? '' : '/'}${path}`;
  }
}
