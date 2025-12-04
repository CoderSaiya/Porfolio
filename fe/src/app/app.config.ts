import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import {
  LucideAngularModule,
  Calendar, MapPin, Award, Coffee,
  Mail, Phone, Send, Github, Linkedin, Twitter,
  ArrowDown, Menu, X, Code, User, Briefcase,
  ExternalLink, Users,
  Server, Database, Cloud, Wrench, GitBranch, CheckCircle2, Loader2, MailPlus, Home,
  Shield, AlertCircle, ArrowRight
} from 'lucide-angular';
import { authInterceptor } from './core/interceptors/auth.interceptor';


export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    provideHttpClient(withInterceptors([authInterceptor])),
    importProvidersFrom(
      LucideAngularModule.pick({
        Calendar, MapPin, Award, Coffee,
        Mail, Phone, Send, Github, Linkedin, Twitter,
        ArrowDown, Menu, X, Code, User, Briefcase,
        ExternalLink, Users,
        Server, Database, Cloud, Wrench, GitBranch,
        Loader2, CheckCircle2, MailPlus, Home,
        Shield, AlertCircle, ArrowRight
      })
    ),
  ],
};
