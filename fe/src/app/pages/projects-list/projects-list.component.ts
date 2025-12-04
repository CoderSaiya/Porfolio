import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { Project, RootService } from '../../core/services/root.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
    selector: 'app-projects-list',
    standalone: true,
    imports: [CommonModule, RouterLink, FormsModule, LucideAngularModule],
    templateUrl: './projects-list.component.html',
    styleUrls: ['./projects-list.component.scss'],
})
export class ProjectsListComponent implements OnInit {
    private svc = inject(RootService);
    private destroyRef = inject(DestroyRef);
    private router = inject(Router);

    projects: Project[] = [];
    filteredProjects: Project[] = [];
    loading = true;
    error: string | null = null;

    searchQuery = '';
    selectedTechnology: string | null = null;
    showFeaturedOnly = false;

    allTechnologies: string[] = [];

    ngOnInit() {
        this.loadProjects();
    }

    loadProjects() {
        this.svc.getProjects()
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: projects => {
                    this.projects = projects;
                    this.filteredProjects = projects;
                    this.extractTechnologies();
                    this.loading = false;
                },
                error: err => {
                    this.error = 'Không thể tải danh sách dự án';
                    console.error(err);
                    this.loading = false;
                }
            });
    }

    extractTechnologies() {
        const techSet = new Set<string>();
        this.projects.forEach(p => {
            p.technologies.forEach(tech => techSet.add(tech));
        });
        this.allTechnologies = Array.from(techSet).sort();
    }

    applyFilters() {
        let result = [...this.projects];

        // Filter by search query
        if (this.searchQuery.trim()) {
            const query = this.searchQuery.toLowerCase();
            result = result.filter(p =>
                p.title.toLowerCase().includes(query) ||
                p.description.toLowerCase().includes(query)
            );
        }

        // Filter by technology
        if (this.selectedTechnology) {
            result = result.filter(p =>
                p.technologies.includes(this.selectedTechnology!)
            );
        }

        // Filter by featured
        if (this.showFeaturedOnly) {
            result = result.filter(p => p.highlight);
        }

        this.filteredProjects = result;
    }

    onSearchChange() {
        this.applyFilters();
    }

    selectTechnology(tech: string | null) {
        this.selectedTechnology = this.selectedTechnology === tech ? null : tech;
        this.applyFilters();
    }

    toggleFeatured() {
        this.showFeaturedOnly = !this.showFeaturedOnly;
        this.applyFilters();
    }

    clearFilters() {
        this.searchQuery = '';
        this.selectedTechnology = null;
        this.showFeaturedOnly = false;
        this.filteredProjects = [...this.projects];
    }

    navigateToProject(slug: string) {
        this.router.navigate(['/projects', slug]);
    }

    onImgError(ev: Event, _title: string) {
        const img = ev?.target as HTMLImageElement | null;
        if (!img) return;
        if (img.dataset['fallback'] === '1') return;
        img.dataset['fallback'] = '1';
        img.src = 'assets/placeholder.png';
    }

    trackById = (_: number, p: Project) => p.id;
}
