import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormArray } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { BlogAdminService } from '../../services/blog-admin.service';
import { BlogCategoryService, BlogCategory } from '../../services/blog-category.service';
import { LucideAngularModule, Save, X } from 'lucide-angular';

@Component({
    selector: 'app-blog-form',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule, LucideAngularModule],
    templateUrl: './blog-form.component.html',
    styleUrls: ['./blog-form.component.scss']
})
export class BlogFormComponent implements OnInit {
    form!: FormGroup;
    isEdit = false;
    blogId: string | null = null;
    loading = false;
    categories: BlogCategory[] = [];
    error: string | null = null;

    readonly SaveIcon = Save;
    readonly XIcon = X;

    constructor(
        private fb: FormBuilder,
        private blogService: BlogAdminService,
        private categoryService: BlogCategoryService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.blogId = this.route.snapshot.params['id'];
        this.isEdit = !!this.blogId;

        this.form = this.fb.group({
            title: ['', Validators.required],
            slug: ['', Validators.required],
            summary: ['', Validators.required],
            content: ['', Validators.required],
            featuredImage: [''],
            categoryId: [''],
            tags: this.fb.array([]),
            published: [false]
        });

        this.loadCategories();

        if (this.isEdit && this.blogId) {
            this.loadBlog();
        }
    }

    get tags() {
        return this.form.get('tags') as FormArray;
    }

    addTag() {
        this.tags.push(this.fb.control('', Validators.required));
    }

    removeTag(index: number) {
        this.tags.removeAt(index);
    }

    generateSlug() {
        const title = this.form.get('title')?.value || '';
        const slug = title
            .toLowerCase()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .replace(/đ/g, 'd')
            .replace(/Đ/g, 'D')
            .replace(/[^a-z0-9]+/g, '-')
            .replace(/^-+|-+$/g, '');
        this.form.patchValue({ slug });
    }

    loadCategories() {
        this.categoryService.getCategories().subscribe({
            next: (cats) => {
                this.categories = cats;
            },
            error: (err) => console.error('Failed to load categories', err)
        });
    }

    loadBlog() {
        if (!this.blogId) return;

        this.loading = true;
        this.blogService.getBlog(this.blogId).subscribe({
            next: (blog) => {
                this.tags.clear();

                this.form.patchValue({
                    title: blog.title,
                    slug: blog.slug,
                    summary: blog.summary,
                    content: blog.content,
                    featuredImage: blog.featuredImage,
                    categoryId: blog.categoryId,
                    published: blog.published
                });

                if (blog.tags && blog.tags.length > 0) {
                    blog.tags.forEach((tag: string) => {
                        this.tags.push(this.fb.control(tag, Validators.required));
                    });
                }

                this.loading = false;
            },
            error: (err) => {
                this.error = 'Không thể tải dữ liệu bài viết';
                this.loading = false;
                console.error(err);
            }
        });
    }

    onSubmit() {
        if (this.form.invalid) return;

        this.loading = true;
        this.error = null;

        const formValue = this.form.value;
        const blogData = {
            ...formValue,
            tags: this.tags.value.filter((t: string) => t.trim())
        };

        const request$ = this.isEdit
            ? this.blogService.updateBlog(this.blogId!, blogData)
            : this.blogService.createBlog(blogData);

        request$.subscribe({
            next: () => {
                this.router.navigate(['/admin/blogs']);
            },
            error: (err) => {
                this.error = 'Không thể lưu bài viết';
                this.loading = false;
                console.error(err);
            }
        });
    }
}
