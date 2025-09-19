import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  inject, NgZone,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import {LucideAngularModule} from 'lucide-angular';
import {NgForOf} from '@angular/common';

interface StatItem {
  icon: string;
  label: string;
  value: string;
}

@Component({
  selector: 'app-about-section',
  standalone: true,
  templateUrl: './about-section.component.html',
  imports: [
    LucideAngularModule,
    NgForOf
  ],
  styleUrls: ['./about-section.component.scss']
})
export class AboutSectionComponent implements OnInit, AfterViewInit, OnDestroy {
  private cdr = inject(ChangeDetectorRef);
  private zone = inject(NgZone);

  isVisible = false;
  private observer?: IntersectionObserver;
  private hasTriggered = false;
  private scrollFallbackBound?: () => void;

  @ViewChild('sectionRef', { static: true }) sectionRef!: ElementRef<HTMLElement>;

  stats: StatItem[] = [
    { icon: 'Calendar', label: 'Kinh nghiệm', value: 'Thực tập' },
    { icon: 'Award', label: 'Dự án hoàn thành', value: '8+' },
    { icon: 'Coffee', label: 'Cốc cà phê', value: '0+' },
    { icon: 'MapPin', label: 'Vị trí', value: 'Hồ Chí Minh, VN' },
  ];

  ngOnInit(): void {
    this.observer = new IntersectionObserver(([entry]) => entry.isIntersecting && (this.isVisible = true), { threshold: .3 });
    this.observer.observe(this.sectionRef.nativeElement);
  }

  ngAfterViewInit(): void {
    // Setup IntersectionObserver sau khi ViewChild đã sẵn sàng
    setTimeout(() => {
      this.setupIntersectionObserver();
    }, 100);
  }

  private setupIntersectionObserver(): void {
    const target = this.sectionRef.nativeElement;

    const show = () => {
      if (this.hasTriggered) return;
      this.hasTriggered = true;

      this.zone.run(() => {
        this.isVisible = true;
        this.cdr.detectChanges();
      });
      this.cleanupObserver();
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
      threshold: [0, 0.1, 0.3]
    });

    this.observer.observe(target);

    // Gắn scroll fallback như backup
    this.attachScrollFallback(show);
  }

  private attachScrollFallback(show: () => void): void {
    if (this.scrollFallbackBound) return;

    const target = this.sectionRef.nativeElement;
    this.scrollFallbackBound = () => {
      if (!this.hasTriggered && this.inViewport(target)) {
        show();
      }
    };

    if (typeof window !== 'undefined') {
      window.addEventListener('scroll', this.scrollFallbackBound, { passive: true });
      window.addEventListener('resize', this.scrollFallbackBound, { passive: true });
    }

    // Kiểm tra định kỳ trong 3 giây đầu
    const intervalId = setInterval(() => {
      if (this.scrollFallbackBound) {
        this.scrollFallbackBound();
      }
    }, 500);

    setTimeout(() => {
      clearInterval(intervalId);
    }, 3000);
  }

  private detachScrollFallback(): void {
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

  private cleanupObserver(): void {
    try {
      this.observer?.disconnect();
    } catch (e) {
      console.warn('Error disconnecting observer:', e);
    }
    this.observer = undefined;
  }

  ngOnDestroy(): void {
    this.cleanupObserver();
    this.detachScrollFallback();
  }
}
