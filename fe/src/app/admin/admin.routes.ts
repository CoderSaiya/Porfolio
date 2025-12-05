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
            }
        ]
    }
];
