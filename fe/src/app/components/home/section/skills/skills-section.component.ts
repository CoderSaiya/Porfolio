import {
  ChangeDetectorRef,
  Component,
  DestroyRef,
  ElementRef,
  inject,
  NgZone,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule, Server, Database, Cloud, Code, Wrench, GitBranch } from 'lucide-angular';
import { RootService, SkillCategories } from '../../../../core/services/root.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';


@Component({
  selector: 'app-skills-section',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './skills-section.component.html',
  styleUrls: ['./skills-section.component.scss'],
})
export class SkillsSectionComponent implements OnInit, OnDestroy {
  private svc = inject(RootService);
  private destroyRef = inject(DestroyRef);
  private zone = inject(NgZone);
  private cdr = inject(ChangeDetectorRef);

  isVisible = false;
  private observer?: IntersectionObserver;
  @ViewChild('sectionRef', { static: true }) sectionRef!: ElementRef<HTMLElement>;

  loading = true;
  error: string | null = null;

  skillCategories: SkillCategories[] = [];
  extraTech = ['RabbitMQ', 'Elasticsearch', 'MongoDB', 'GraphQL', 'gRPC', 'FluentValidation', 'MediatR', 'Serilog', 'NLog', 'HealthChecks', 'OpenAPI'];

  ngOnInit() {
    this.observer = new IntersectionObserver(([entry]) => {
      if (entry.isIntersecting) {
        this.zone.run(() => {
          this.isVisible = true;
          this.cdr.markForCheck();
        });
        this.observer?.unobserve(this.sectionRef.nativeElement);
      }
    }, { threshold: 0.3, rootMargin: '0px 0px -10% 0px' });

    // đợi view gắn xong rồi observe (tránh race nhỏ)
    queueMicrotask(() => this.observer!.observe(this.sectionRef.nativeElement));

    this.svc.getSkills()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: res => {
          this.skillCategories = res;
        },
        error: err => {
          this.error = 'Không tải được kỹ năng';
          console.error(err);
        },
        complete: () => this.loading = false
      });
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
  }
}
