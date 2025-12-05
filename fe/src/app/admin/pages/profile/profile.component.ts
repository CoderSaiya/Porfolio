import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormArray } from '@angular/forms';
import { ProfileAdminService, Profile } from '../../services/profile-admin.service';
import { LucideAngularModule, Save, Upload, Plus, Trash2 } from 'lucide-angular';

@Component({
    selector: 'app-profile',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
    profileForm: FormGroup;
    loading = false;
    avatarPreview: string | null = null;

    // Icons
    readonly SaveIcon = Save;
    readonly UploadIcon = Upload;
    readonly PlusIcon = Plus;
    readonly TrashIcon = Trash2;

    constructor(
        private fb: FormBuilder,
        private profileService: ProfileAdminService
    ) {
        this.profileForm = this.fb.group({
            fullName: ['', Validators.required],
            headline: ['', Validators.required],
            location: [''],
            about: [''],
            yearsExperience: [0],
            projectsCompleted: [0],
            coffees: [0],
            socialLinks: this.fb.array([])
        });
    }

    ngOnInit(): void {
        this.loadProfile();
    }

    get socialLinks() {
        return this.profileForm.get('socialLinks') as FormArray;
    }

    loadProfile() {
        this.loading = true;
        this.profileService.getProfile().subscribe({
            next: (profile) => {
                this.profileForm.patchValue({
                    fullName: profile.fullName,
                    headline: profile.headline,
                    location: profile.location,
                    about: profile.about,
                    yearsExperience: profile.yearsExperience,
                    projectsCompleted: profile.projectsCompleted,
                    coffees: profile.coffees
                });

                this.avatarPreview = profile.avatarUrl || null;

                this.socialLinks.clear();
                if (profile.socialLinks) {
                    profile.socialLinks.forEach(link => {
                        this.socialLinks.push(this.createSocialLinkGroup(link));
                    });
                }
                this.loading = false;
            },
            error: (err) => {
                console.error('Failed to load profile', err);
                this.loading = false;
            }
        });
    }

    createSocialLinkGroup(link?: any): FormGroup {
        return this.fb.group({
            platform: [link?.platform || 0, Validators.required],
            url: [link?.url || '', Validators.required],
            order: [link?.order || 0]
        });
    }

    addSocialLink() {
        this.socialLinks.push(this.createSocialLinkGroup());
    }

    removeSocialLink(index: number) {
        this.socialLinks.removeAt(index);
    }

    onFileSelected(event: any) {
        const file = event.target.files[0];
        if (file) {
            this.loading = true;
            this.profileService.uploadAvatar(file).subscribe({
                next: (res) => {
                    this.avatarPreview = res.url;
                    this.loading = false;
                },
                error: (err) => {
                    console.error('Avatar upload failed', err);
                    this.loading = false;
                }
            });
        }
    }

    onSubmit() {
        if (this.profileForm.valid) {
            this.loading = true;
            this.profileService.updateProfile(this.profileForm.value).subscribe({
                next: (res) => {
                    console.log('Profile updated', res);
                    this.loading = false;
                },
                error: (err) => {
                    console.error('Update failed', err);
                    this.loading = false;
                }
            });
        }
    }
}
