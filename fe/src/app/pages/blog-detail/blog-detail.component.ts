import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { BlogService, BlogPost } from '../../core/services/blog.service';

@Component({
    selector: 'app-blog-detail',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './blog-detail.component.html',
    styleUrls: ['./blog-detail.component.scss']
})
export class BlogDetailComponent implements OnInit {
    blog: BlogPost | null = null;
    loading = false;

    constructor(
        private route: ActivatedRoute,
        private blogService: BlogService
    ) { }

    ngOnInit(): void {
        const slug = this.route.snapshot.params['slug'];
        if (slug) {
            this.loadBlog(slug);
        }
    }

    loadBlog(slug: string) {
        this.loading = true;
        this.blogService.getPostBySlug(slug).subscribe({
            next: (blog) => {
                this.blog = blog;
                this.loading = false;
            },
            error: (err) => {
                console.error('Failed to load blog', err);
                this.loading = false;
            }
        });
    }
}
