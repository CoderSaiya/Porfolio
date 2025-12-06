import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { BlogService, BlogPost, BlogCategory } from '../../core/services/blog.service';

@Component({
    selector: 'app-blog-list',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './blog-list.component.html',
    styleUrls: ['./blog-list.component.scss']
})
export class BlogListComponent implements OnInit {
    blogs: BlogPost[] = [];
    categories: BlogCategory[] = [];
    total = 0;
    page = 1;
    pageSize = 9;
    selectedCategoryId?: string;
    loading = false;

    readonly Math = Math;

    constructor(private blogService: BlogService) { }

    ngOnInit(): void {
        this.loadBlogs();
        this.loadCategories();
    }

    loadBlogs() {
        this.loading = true;
        this.blogService.getPosts(this.page, this.pageSize, this.selectedCategoryId).subscribe({
            next: (res) => {
                this.blogs = res.data;
                this.total = res.total;
                this.loading = false;
            },
            error: (err) => {
                console.error('Failed to load blogs', err);
                this.loading = false;
            }
        });
    }

    loadCategories() {
        this.blogService.getCategories().subscribe({
            next: (cats) => {
                this.categories = cats;
            },
            error: (err) => console.error('Failed to load categories', err)
        });
    }

    filterByCategory(categoryId?: string) {
        this.selectedCategoryId = categoryId;
        this.page = 1;
        this.loadBlogs();
    }

    onPageChange(newPage: number) {
        this.page = newPage;
        this.loadBlogs();
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    get totalPages(): number {
        return Math.ceil(this.total / this.pageSize);
    }

    get pages(): number[] {
        return Array.from({ length: this.totalPages }, (_, i) => i + 1);
    }

    getCategoryName(categoryId?: string): string {
        if (!categoryId) return '';
        const cat = this.categories.find(c => c.id === categoryId);
        return cat ? cat.name : '';
    }
}
