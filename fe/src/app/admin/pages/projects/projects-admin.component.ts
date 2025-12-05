import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProjectAdminService } from '../../services/project-admin.service';
import { ProjectAdmin } from '../../../core/models/project-admin.model';
import { LucideAngularModule } from 'lucide-angular';

@Component({
    selector: 'app-projects-admin',
    standalone: true,
    imports: [CommonModule, RouterModule, LucideAngularModule],
    templateUrl: './projects-admin.component.html',
    styleUrls: ['./projects-admin.component.scss']
})
export class ProjectsAdminComponent implements OnInit {
    private projectService = inject(ProjectAdminService);

    projects: ProjectAdmin[] = [];
    loading = false;
    error: string | null = null;
    projectToDelete: ProjectAdmin | null = null;

    ngOnInit() {
        this.loadProjects();
    }

    loadProjects() {
        this.loading = true;
        this.error = null;

        this.projectService.getAllProjects().subscribe({
            next: (projects) => {
                this.projects = projects;
                this.loading = false;
            },
            error: (err) => {
                this.error = 'Failed to load projects';
                this.loading = false;
                console.error(err);
            }
        });
    }

    confirmDelete(project: ProjectAdmin) {
        this.projectToDelete = project;
    }

    cancelDelete() {
        this.projectToDelete = null;
    }

    deleteProject() {
        if (!this.projectToDelete) return;

        const id = this.projectToDelete.id;
        this.projectService.deleteProject(id).subscribe({
            next: () => {
                this.projects = this.projects.filter(p => p.id !== id);
                this.projectToDelete = null;
            },
            error: (err) => {
                this.error = 'Failed to delete project';
                console.error(err);
            }
        });
    }
}
