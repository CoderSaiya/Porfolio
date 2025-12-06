import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: "",
    component: HomeComponent,
  },
  {
    path: "projects",
    loadComponent: () => import('./pages/projects-list/projects-list.component').then(m => m.ProjectsListComponent),
  },
  {
    path: "projects/:slug",
    loadComponent: () => import('./pages/project-detail/project-detail.component').then(m => m.ProjectDetailComponent),
  },
  {
    path: "blog",
    loadComponent: () => import('./pages/blog-list/blog-list.component').then(m => m.BlogListComponent),
  },
  {
    path: "blog/:slug",
    loadComponent: () => import('./pages/blog-detail/blog-detail.component').then(m => m.BlogDetailComponent),
  },
  {
    path: "login",
    loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent),
  },
  {
    path: "admin",
    canActivate: [authGuard],
    loadChildren: () => import('./admin/admin.routes').then(m => m.ADMIN_ROUTES),
  },
];
