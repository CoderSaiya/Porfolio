import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BlogService, BlogPost, BlogCategory } from '../../core/services/blog.service';
import { LucideAngularModule } from 'lucide-angular';

@Component({
    selector: 'app-blog-list',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule, LucideAngularModule],
    templateUrl: './blog-list.component.html',
    styleUrls: ['./blog-list.component.scss']
})
export class BlogListComponent implements OnInit {
    blogs: BlogPost[] = [];
    categories: BlogCategory[] = [];
    total = 0;
    page = 1;
    pageSize = 20;
    selectedCategoryId?: string;
    loading = false;
    searchQuery = '';
    filterType: 'all' | 'writing' = 'all';

    allBlogs: BlogPost[] = [];
    filteredBlogs: BlogPost[] = [];
    recentPosts: BlogPost[] = [];
    allPosts: BlogPost[] = [];

    readonly Math = Math;
    defaultImage = 'assets/images/blog-default.jpg';

    constructor(private blogService: BlogService) { }

    ngOnInit(): void {
        this.loadBlogs();
        this.loadCategories();
    }

    loadBlogs() {
        this.loading = true;
        this.blogService.getPosts(this.page, this.pageSize, this.selectedCategoryId).subscribe({
            next: (res) => {
                this.allBlogs = res.data.filter(blog => blog.published);
                this.total = res.total;
                this.applyFilters();
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

    applyFilters() {
        let filtered = [...this.allBlogs];

        // Apply search filter
        if (this.searchQuery.trim()) {
            const query = this.searchQuery.toLowerCase();
            filtered = filtered.filter(blog =>
                blog.title.toLowerCase().includes(query) ||
                blog.summary.toLowerCase().includes(query) ||
                blog.tags.some(tag => tag.toLowerCase().includes(query))
            );
        }

        // Apply category filter
        if (this.filterType === 'writing' && this.categories.length > 0) {
            const writingCategory = this.categories.find(c => c.name.toLowerCase().includes('writing'));
            if (writingCategory) {
                filtered = filtered.filter(blog => blog.categoryId === writingCategory.id);
            }
        }

        this.filteredBlogs = filtered;
        this.recentPosts = filtered.slice(0, 4);
        this.allPosts = filtered.slice(4);
    }

    onSearch() {
        this.applyFilters();
    }

    onFilterChange(type: 'all' | 'writing') {
        this.filterType = type;
        this.applyFilters();
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

    formatDate(dateString: string): string {
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    }
}
