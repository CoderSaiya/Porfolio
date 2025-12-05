import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SkillAdminService } from '../../services/skill-admin.service';
import { SkillCategoryAdmin } from '../../../core/models/skill-admin.model';
import { LucideAngularModule } from 'lucide-angular';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';

@Component({
    selector: 'app-skills-admin',
    standalone: true,
    imports: [CommonModule, RouterModule, LucideAngularModule, DragDropModule],
    templateUrl: './skills-admin.component.html',
    styleUrls: ['./skills-admin.component.scss']
})
export class SkillsAdminComponent implements OnInit {
    private skillService = inject(SkillAdminService);

    categories: SkillCategoryAdmin[] = [];
    loading = false;
    error: string | null = null;
    categoryToDelete: SkillCategoryAdmin | null = null;

    ngOnInit() {
        this.loadCategories();
    }

    loadCategories() {
        this.loading = true;
        this.error = null;

        this.skillService.getAllCategories().subscribe({
            next: (categories) => {
                this.categories = categories.sort((a, b) => a.order - b.order);
                this.loading = false;
            },
            error: (err) => {
                this.error = 'Failed to load skill categories';
                this.loading = false;
                console.error(err);
            }
        });
    }

    drop(event: CdkDragDrop<SkillCategoryAdmin[]>) {
        moveItemInArray(this.categories, event.previousIndex, event.currentIndex);

        // Update orders
        const reorderRequest = {
            categories: this.categories.map((cat, index) => ({
                id: cat.id,
                order: index + 1
            }))
        };

        this.skillService.reorderCategories(reorderRequest).subscribe({
            error: (err) => {
                this.error = 'Failed to reorder categories';
                console.error(err);
                this.loadCategories(); // Reload on error
            }
        });
    }

    confirmDelete(category: SkillCategoryAdmin) {
        this.categoryToDelete = category;
    }

    cancelDelete() {
        this.categoryToDelete = null;
    }

    deleteCategory() {
        if (!this.categoryToDelete) return;

        const id = this.categoryToDelete.id;
        this.skillService.deleteCategory(id).subscribe({
            next: () => {
                this.categories = this.categories.filter(c => c.id !== id);
                this.categoryToDelete = null;
            },
            error: (err) => {
                this.error = 'Failed to delete category';
                console.error(err);
            }
        });
    }
}
