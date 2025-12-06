import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BlogCategoryService, BlogCategory } from '../../services/blog-category.service';
import { LucideAngularModule, Plus, Edit, Trash2, Save, X } from 'lucide-angular';

@Component({
    selector: 'app-blog-categories',
    standalone: true,
    imports: [CommonModule, FormsModule, LucideAngularModule],
    templateUrl: './blog-categories.component.html',
    styleUrls: ['./blog-categories.component.scss']
})
export class BlogCategoriesComponent implements OnInit {
    categories: BlogCategory[] = [];
    loading = false;
    editingId: string | null = null;

    newCategory = {
        name: '',
        slug: '',
        description: '',
        order: 0
    };

    editCategory: any = null;

    readonly PlusIcon = Plus;
    readonly EditIcon = Edit;
    readonly TrashIcon = Trash2;
    readonly SaveIcon = Save;
    readonly XIcon = X;

    constructor(private categoryService: BlogCategoryService) { }

    ngOnInit(): void {
        this.loadCategories();
    }

    loadCategories() {
        this.loading = true;
        this.categoryService.getCategories().subscribe({
            next: (cats) => {
                this.categories = cats;
                this.loading = false;
            },
            error: (err) => {
                console.error('Failed to load categories', err);
                this.loading = false;
            }
        });
    }

    generateSlug(name: string): string {
        return name
            .toLowerCase()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .replace(/đ/g, 'd')
            .replace(/Đ/g, 'D')
            .replace(/[^a-z0-9]+/g, '-')
            .replace(/^-+|-+$/g, '');
    }

    onNameChange() {
        if (this.editingId) {
            this.editCategory.slug = this.generateSlug(this.editCategory.name);
        } else {
            this.newCategory.slug = this.generateSlug(this.newCategory.name);
        }
    }

    createCategory() {
        if (!this.newCategory.name) return;

        this.categoryService.createCategory(this.newCategory).subscribe({
            next: () => {
                this.resetForm();
                this.loadCategories();
            },
            error: (err) => console.error('Failed to create category', err)
        });
    }

    startEdit(category: BlogCategory) {
        this.editingId = category.id;
        this.editCategory = { ...category };
    }

    saveEdit() {
        if (!this.editCategory || !this.editingId) return;

        this.categoryService.updateCategory(this.editingId, this.editCategory).subscribe({
            next: () => {
                this.cancelEdit();
                this.loadCategories();
            },
            error: (err) => console.error('Failed to update category', err)
        });
    }

    cancelEdit() {
        this.editingId = null;
        this.editCategory = null;
    }

    deleteCategory(id: string, name: string) {
        if (confirm(`Bạn có chắc chắn muốn xóa danh mục "${name}"?`)) {
            this.categoryService.deleteCategory(id).subscribe({
                next: () => {
                    this.loadCategories();
                },
                error: (err) => console.error('Failed to delete category', err)
            });
        }
    }

    resetForm() {
        this.newCategory = {
            name: '',
            slug: '',
            description: '',
            order: 0
        };
    }
}
