import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { ProjectDetail, RootService } from '../../core/services/root.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
    selector: 'app-project-detail',
    standalone: true,
    imports: [CommonModule, RouterLink, LucideAngularModule],
    templateUrl: './project-detail.component.html',
    styleUrls: ['./project-detail.component.scss'],
})
export class ProjectDetailComponent implements OnInit {
    private svc = inject(RootService);
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private location = inject(Location);
    private destroyRef = inject(DestroyRef);

    project: ProjectDetail | null = null;
    loading = true;
    error: string | null = null;

    ngOnInit() {
        const slug = this.route.snapshot.paramMap.get('slug');
        if (!slug) {
            this.error = 'Không tìm thấy dự án';
            this.loading = false;
            return;
        }

        this.loadProject(slug);
    }

    loadProject(slug: string) {
        this.svc.getProjectDetail(slug)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: project => {
                    this.project = project;
                    this.loading = false;
                },
                error: err => {
                    this.error = 'Không tìm thấy dự án hoặc đã có lỗi xảy ra';
                    console.error(err);
                    this.loading = false;
                }
            });
    }

    goBack() {
        this.location.back();
    }

    navigateToProjects() {
        this.router.navigate(['/projects']);
    }

    onImgError(ev: Event) {
        const img = ev?.target as HTMLImageElement | null;
        if (!img) return;
        if (img.dataset['fallback'] === '1') return;
        img.dataset['fallback'] = '1';
        img.src = 'assets/placeholder.png';
    }
}
