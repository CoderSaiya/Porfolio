import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { BlogService, BlogPost } from '../../../../core/services/blog.service';
import { LucideAngularModule } from 'lucide-angular';

@Component({
    selector: 'app-blogs-section',
    standalone: true,
    imports: [CommonModule, LucideAngularModule],
    templateUrl: './blogs-section.component.html',
    styleUrls: ['./blogs-section.component.scss']
})
export class BlogsSectionComponent implements OnInit {
    blogs: BlogPost[] = [];
    loading = true;
    defaultImage = 'assets/images/blog-default.jpg';

    constructor(
        private blogService: BlogService,
        private router: Router
    ) { }

    ngOnInit() {
        this.loadBlogs();
    }

    loadBlogs() {
        this.blogService.getPosts(1, 6).subscribe({
            next: (response) => {
                this.blogs = response.data.filter(blog => blog.published);
                this.loading = false;
            },
            error: (err) => {
                console.error('Failed to load blogs', err);
                this.loading = false;
            }
        });
    }

    navigateToBlog(slug: string) {
        this.router.navigate(['/blog', slug]);
    }

    viewAllBlogs() {
        this.router.navigate(['/blog']);
    }

    formatDate(dateString: string): string {
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    }
}
