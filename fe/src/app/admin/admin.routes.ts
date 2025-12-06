import { Routes } from '@angular/router';

export const ADMIN_ROUTES: Routes = [
    {
        path: '',
        loadComponent: () => import('./layout/admin-layout.component').then(m => m.AdminLayoutComponent),
        children: [
            {
                path: '',
                redirectTo: 'dashboard',
                pathMatch: 'full'
            },
            {
                path: 'dashboard',
                loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent)
            },
            {
                path: 'projects',
                loadComponent: () => import('./pages/projects/projects-admin.component').then(m => m.ProjectsAdminComponent)
            },
            {
                path: 'projects/new',
                loadComponent: () => import('./pages/projects/project-form.component').then(m => m.ProjectFormComponent)
            },
            {
                path: 'projects/edit/:id',
                loadComponent: () => import('./pages/projects/project-form.component').then(m => m.ProjectFormComponent)
            },
            {
                path: 'skills',
                loadComponent: () => import('./pages/skills/skills-admin.component').then(m => m.SkillsAdminComponent)
            },
            {
                path: 'profile',
                loadComponent: () => import('./pages/profile/profile.component').then(m => m.ProfileComponent)
            },
            {
                path: 'messages',
                loadComponent: () => import('./pages/messages/messages.component').then(m => m.MessagesComponent)
            },
            {
                path: 'messages/:id',
                loadComponent: () => import('./pages/messages/message-detail/message-detail.component').then(m => m.MessageDetailComponent)
            },
            {
                path: 'blogs',
                loadComponent: () => import('./pages/blogs/blogs.component').then(m => m.BlogsComponent)
            },
            {
                path: 'blogs/new',
                loadComponent: () => import('./pages/blogs/blog-form.component').then(m => m.BlogFormComponent)
            },
            {
                path: 'blogs/edit/:id',
                loadComponent: () => import('./pages/blogs/blog-form.component').then(m => m.BlogFormComponent)
            }
        ]
    }
];
