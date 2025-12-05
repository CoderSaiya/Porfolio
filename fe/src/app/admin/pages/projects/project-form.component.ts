import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ProjectAdminService } from '../../services/project-admin.service';
import { LucideAngularModule } from 'lucide-angular';

@Component({
    selector: 'app-project-form',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
    templateUrl: './project-form.component.html',
    styleUrls: ['./project-form.component.scss']
})
export class ProjectFormComponent implements OnInit {
    private fb = inject(FormBuilder);
    private projectService = inject(ProjectAdminService);
    private router = inject(Router);
    private route = inject(ActivatedRoute);

    form!: FormGroup;
    isEdit = false;
    projectId: string | null = null;
    loading = false;
    error: string | null = null;

    ngOnInit() {
        this.projectId = this.route.snapshot.params['id'];
        this.isEdit = !!this.projectId;

        this.form = this.fb.group({
            title: ['', Validators.required],
            slug: ['', Validators.required],
            description: [''],
            highlight: [false],
            duration: [1, [Validators.required, Validators.min(1)]],
            teamSize: [1, [Validators.required, Validators.min(1)]],
            github: [''],
            demo: [''],
            technologies: this.fb.array([]),
            features: this.fb.array([])
        });

        if (this.isEdit) {
            this.loadProject();
        }
    }

    get technologies() {
        return this.form.get('technologies') as FormArray;
    }

    get features() {
        return this.form.get('features') as FormArray;
    }

    addTechnology() {
        this.technologies.push(this.fb.control('', Validators.required));
    }

    removeTechnology(index: number) {
        this.technologies.removeAt(index);
    }

    addFeature() {
        this.features.push(this.fb.control('', Validators.required));
    }

    removeFeature(index: number) {
        this.features.removeAt(index);
    }

    generateSlug() {
        const title = this.form.get('title')?.value || '';
        const slug = title
            .toLowerCase()
            .replace(/[^a-z0-9]+/g, '-')
            .replace(/^-+|-+$/g, '');
        this.form.patchValue({ slug });
    }

    loadProject() {
        // Load from list or make API call
        // For now, placeholder
    }

    onSubmit() {
        if (this.form.invalid) return;

        this.loading = true;
        this.error = null;

        const formValue = this.form.value;
        const projectData = {
            ...formValue,
            technologies: this.technologies.value.filter((t: string) => t.trim()),
            features: this.features.value.filter((f: string) => f.trim())
        };

        const request$ = this.isEdit
            ? this.projectService.updateProject(this.projectId!, projectData)
            : this.projectService.createProject(projectData);

        request$.subscribe({
            next: () => {
                this.router.navigate(['/admin/projects']);
            },
            error: (err) => {
                this.error = 'Failed to save project';
                this.loading = false;
                console.error(err);
            }
        });
    }
}
