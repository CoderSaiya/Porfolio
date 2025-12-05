export interface ProjectAdmin {
    id: string;
    title: string;
    slug: string;
    description?: string;
    highlight: boolean;
    duration: number;
    teamSize: number;
    github?: string;
    demo?: string;
    technologies: string[];
    features: string[];
    imageUrl?: string;
    createdAt?: string;
}

export interface CreateProjectRequest {
    title: string;
    slug: string;
    description?: string;
    highlight: boolean;
    duration: number;
    teamSize: number;
    github?: string;
    demo?: string;
    technologies: string[];
    features: string[];
}

export interface UpdateProjectRequest {
    title?: string;
    slug?: string;
    description?: string;
    highlight?: boolean;
    duration?: number;
    teamSize?: number;
    github?: string;
    demo?: string;
    technologies?: string[];
    features?: string[];
}
