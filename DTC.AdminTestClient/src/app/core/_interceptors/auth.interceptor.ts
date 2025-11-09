import { Injectable } from '@angular/core';
import {
  HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AccountService } from '../services/account.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject = new BehaviorSubject<boolean>(false);

  constructor(private accountService: AccountService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Для httpOnly кук просто добавляем withCredentials: true
    const authReq = this.addCredentials(req);
    
    return next.handle(authReq).pipe(
      catchError(err => {
        if (err instanceof HttpErrorResponse && err.status === 401) {
          return this.handle401(authReq, next);
        }
        return throwError(() => err);
      })
    );
  }

  private addCredentials(req: HttpRequest<any>): HttpRequest<any> {
    // Для httpOnly кук достаточно withCredentials: true
    // Браузер автоматически отправит куки
    if (!req.withCredentials) {
      console.log('🍪 Adding credentials to request');
      return req.clone({
        withCredentials: true
      });
    }
    console.log('🍪 Request already has credentials');
    return req;
  }

  private handle401(req: HttpRequest<any>, next: HttpHandler) {
    console.log('🔄 Handling 401 error for:', req.url);

    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(false);

      return this.accountService.refreshToken().pipe(
        switchMap((response: any) => {
          console.log('✅ Token refresh successful');
          this.isRefreshing = false;
          this.refreshTokenSubject.next(true);
          
          // После обновления токена повторяем исходный запрос
          // Куки автоматически отправятся браузером
          console.log('🔄 Retrying original request with updated cookies');
          return next.handle(this.addCredentials(req));
        }),
        catchError((refreshError: HttpErrorResponse) => {
          console.log('❌ Token refresh failed:', refreshError.status);
          this.isRefreshing = false;
          this.refreshTokenSubject.next(false);
          
          if (refreshError.status === 401) {
            console.log('🚨 Refresh token invalid, logging out...');
            this.accountService.logout().subscribe();
          }
          
          return throwError(() => refreshError);
        })
      );
    } else {
      console.log('⏳ Already refreshing, waiting...');
      
      return this.refreshTokenSubject.pipe(
        filter(done => done === true),
        take(1),
        switchMap(() => {
          console.log('🔄 Retrying queued request after refresh');
          return next.handle(this.addCredentials(req));
        })
      );
    }
  }
}