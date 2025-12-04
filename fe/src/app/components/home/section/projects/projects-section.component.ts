import {
  AfterViewInit,
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
import { Router } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Project, RootService } from '../../../../core/services/root.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-projects-section',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './projects-section.component.html',
  styleUrls: ['./projects-section.component.scss'],
})
export class ProjectsSectionComponent implements OnInit, AfterViewInit, OnDestroy {
  private svc = inject(RootService);
  private destroyRef = inject(DestroyRef);
  private router = inject(Router);
  private zone = inject(NgZone);
  private cdr = inject(ChangeDetectorRef);

  isVisible = false;
  private observer?: IntersectionObserver;
  private scrollFallbackBound?: () => void;
  private hasTriggered = false; // Thêm flag để tránh trigger nhiều lần

  @ViewChild('sectionRef', { static: true }) sectionRef!: ElementRef<HTMLElement>;

  projects: Project[] = [];
  loading = true;
  error: string | null = null;

  ngOnInit() {
    this.svc.getProjects(true, 6)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: items => this.projects = items,
        error: err => {
          this.error = 'Không tải được dự án';
          console.error(err);
        },
        complete: () => this.loading = false
      });
  }

  ngAfterViewInit() {
    // Đợi một chút để DOM render xong
    setTimeout(() => {
      this.setupIntersectionObserver();
    }, 100);
  }

  private setupIntersectionObserver() {
    const target = this.sectionRef.nativeElement;

    const show = () => {
      if (this.hasTriggered) return;
      this.hasTriggered = true;

      this.zone.run(() => {
        this.isVisible = true;
        this.cdr.detectChanges();
      });
      this.cleanupIO();
      this.detachScrollFallback();
    };

    // Kiểm tra ngay lập tức nếu element đã trong viewport
    if (this.inViewport(target)) {
      show();
      return;
    }

    // Fallback nếu không có IntersectionObserver
    if (typeof IntersectionObserver === 'undefined') {
      this.attachScrollFallback(show);
      return;
    }

    // Setup IntersectionObserver
    this.observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting && entry.target === target) {
          show();
        }
      });
    }, {
      root: null,
      rootMargin: '0px 0px -10% 0px',
      threshold: [0, 0.1, 0.2] // Thêm nhiều threshold
    });

    this.observer.observe(target);

    // Gắn scroll fallback như backup
    this.attachScrollFallback(show);
  }

  ngOnDestroy(): void {
    this.cleanupIO();
    this.detachScrollFallback();
  }

  private cleanupIO() {
    try {
      this.observer?.disconnect();
    } catch (e) {
      console.warn('Error disconnecting observer:', e);
    }
    this.observer = undefined;
  }

  private attachScrollFallback(show: () => void) {
    if (this.scrollFallbackBound) return;

    const target = this.sectionRef.nativeElement;
    this.scrollFallbackBound = () => {
      if (!this.hasTriggered && this.inViewport(target)) {
        show();
      }
    };

    // Sử dụng window thay vì globalThis để tránh lỗi
    if (typeof window !== 'undefined') {
      window.addEventListener('scroll', this.scrollFallbackBound, { passive: true });
      window.addEventListener('resize', this.scrollFallbackBound, { passive: true });
    }

    // Kiểm tra định kỳ trong 3 giây đầu (phòng trường hợp có vấn đề về timing)
    const intervalId = setInterval(() => {
      if (this.scrollFallbackBound) {
        this.scrollFallbackBound();
      }
    }, 500);

    setTimeout(() => {
      clearInterval(intervalId);
    }, 3000);
  }

  private detachScrollFallback() {
    if (!this.scrollFallbackBound) return;

    if (typeof window !== 'undefined') {
      window.removeEventListener('scroll', this.scrollFallbackBound);
      window.removeEventListener('resize', this.scrollFallbackBound);
    }
    this.scrollFallbackBound = undefined;
  }

  private inViewport(el: HTMLElement): boolean {
    try {
      const rect = el.getBoundingClientRect();
      const windowHeight = window.innerHeight || document.documentElement.clientHeight;
      const windowWidth = window.innerWidth || document.documentElement.clientWidth;

      // Kiểm tra element có trong viewport không (với margin 10%)
      return (
        rect.top < windowHeight * 0.9 &&
        rect.bottom > windowHeight * 0.1 &&
        rect.left < windowWidth &&
        rect.right > 0
      );
    } catch (e) {
      console.warn('Error checking viewport:', e);
      return false;
    }
  }

  onImgError(ev: Event | ErrorEvent, _title: string) {
    const img = ev?.target as HTMLImageElement | null;
    if (!img) return;
    if (img.dataset['fallback'] === '1') return;
    img.dataset['fallback'] = '1';
    img.src = 'assets/placeholder.png';
  }

  fullImageOf(p: Project) {
    return this.svc.getProjectImageUrl(p.id, 'full');
  }

  navigateToProjects() {
    this.router.navigate(['/projects']);
  }

  navigateToProject(slug: string) {
    this.router.navigate(['/projects', slug]);
  }

  trackById = (_: number, p: Project) => p.id;

  protected readonly HTMLImageElement = HTMLImageElement;
}
