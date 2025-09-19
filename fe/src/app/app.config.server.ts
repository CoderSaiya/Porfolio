import { ApplicationConfig, mergeApplicationConfig } from '@angular/core';
import { provideServerRendering } from '@angular/platform-server';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { appConfig } from './app.config';

export const appServerConfig: ApplicationConfig = mergeApplicationConfig(
  appConfig,
  {
    providers: [
      provideServerRendering(),
      provideNoopAnimations(),
    ],
  }
);
