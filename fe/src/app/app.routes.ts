import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';

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
];
