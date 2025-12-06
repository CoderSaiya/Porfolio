import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BlogAdminService, BlogPost, BlogFilter } from '../../services/blog-admin.service';
import { BlogCategoryService, BlogCategory } from '../../services/blog-category.service';
import { LucideAngularModule, Search, Plus, Edit, Trash2, Eye } from 'lucide-angular';

@Component({
    selector: 'app-blogs',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule, LucideAngularModule],
    templateUrl: './blogs.component.html',
    styleUrls: ['./blogs.component.scss']
})
export class BlogsComponent implements OnInit {
    blogs: BlogPost[] = [];
    categories: BlogCategory[] = [];
    total = 0;
    loading = false;

    filter: BlogFilter = {
        page: 1,
        pageSize: 10,
        search: '',
        published: undefined,
        categoryId: undefined
    };

    readonly Math = Math;

    // Icons
    readonly SearchIcon = Search;
    readonly PlusIcon = Plus;
    readonly EditIcon = Edit;
    readonly TrashIcon = Trash2;
    readonly EyeIcon = Eye;

    constructor(
        private blogService: BlogAdminService,
        private categoryService: BlogCategoryService
    ) { }

    ngOnInit(): void {
        this.loadBlogs();
        this.loadCategories();
    }

    loadBlogs() {
        this.loading = true;
        this.blogService.getBlogs(this.filter).subscribe({
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
        this.categoryService.getCategories().subscribe({
            next: (cats) => {
                this.categories = cats;
            },
            error: (err) => console.error('Failed to load categories', err)
        });
    }

    getCategoryName(categoryId: string | undefined): string {
        if (!categoryId) {
            return '-';
        }

        const cat = this.categories.find(c => c.id === categoryId);
        return cat?.name ?? '-';
    }

    onSearch() {
        this.filter.page = 1;
        this.loadBlogs();
    }

    onFilterChange() {
        this.filter.page = 1;
        this.loadBlogs();
    }

    onPageChange(page: number) {
        this.filter.page = page;
        this.loadBlogs();
    }

    deleteBlog(id: string, title: string) {
        if (confirm(`Bạn có chắc chắn muốn xóa bài viết "${title}"?`)) {
            this.blogService.deleteBlog(id).subscribe(() => {
                this.loadBlogs();
            });
        }
    }

    togglePublish(blog: BlogPost) {
        this.blogService.updateBlog(blog.id, { published: !blog.published }).subscribe(() => {
            this.loadBlogs();
        });
    }

    get totalPages(): number {
        return Math.ceil(this.total / this.filter.pageSize);
    }

    get pages(): number[] {
        return Array.from({ length: this.totalPages }, (_, i) => i + 1);
    }
}
