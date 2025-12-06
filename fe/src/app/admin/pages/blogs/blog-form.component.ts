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
    selectedFile: File | null = null;
    imagePreview: string | null = null;

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

                if (blog.featuredImage) {
                    this.imagePreview = blog.featuredImage;
                }

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

    onImageSelected(event: Event) {
        const input = event.target as HTMLInputElement;
        if (input.files && input.files.length > 0) {
            this.selectedFile = input.files[0];

            // Preview image
            const reader = new FileReader();
            reader.onload = (e: any) => {
                this.imagePreview = e.target.result;
            };
            reader.readAsDataURL(this.selectedFile);
        }
    }

    onSubmit() {
        // if (this.form.invalid) return;

        this.loading = true;
        this.error = null;

        const formData = new FormData();

        formData.append("Title", this.form.value.title);
        formData.append("Slug", this.form.value.slug);
        formData.append("Summary", this.form.value.summary);
        formData.append("Content", this.form.value.content);
        formData.append("Published", this.form.value.published);
        formData.append("CategoryId", this.form.value.categoryId || "");

        // Append tags
        this.tags.value.forEach((t: string) => formData.append("Tags", t));

        // Append file
        if (this.selectedFile) {
            formData.append("File", this.selectedFile);
        }

        const request$ = this.isEdit
            ? this.blogService.updateBlogFormData(this.blogId!, formData)
            : this.blogService.createBlog(formData);

        request$.subscribe({
            next: () => {
                this.router.navigate(["/admin/blogs"]);
            },
            error: err => {
                console.error(err);
                this.error = "Không thể lưu blog";
                this.loading = false;
            }
        });
    }
}
