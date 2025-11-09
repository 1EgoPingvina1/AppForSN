import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { HTTP_INTERCEPTORS, HttpClientModule, provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { ErrorInterceptor } from './core/_interceptors/error.interceptor';
import { HtmlParser } from '@angular/compiler';
import { provideToastr } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { AuthInterceptor } from './core/_interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes), 
    provideAnimationsAsync(), 
    provideToastr(),
    provideHttpClient(
      withInterceptorsFromDi(),
    ),
    DatePipe,
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  {provide: HTTP_INTERCEPTORS,useClass: ErrorInterceptor,multi:true},]
};
