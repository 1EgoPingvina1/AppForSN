import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, tap, throwError } from 'rxjs';
import { User } from '../models/User';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environment/environment';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient,private router:Router) { }

  login(user: any) {
    return this.http.post(
      this.baseUrl + "Account/login",
      user,
      { withCredentials: true }
    ).pipe(
      tap(() => {
        this.loadCurrentUser().subscribe({
          next: () => {
            this.router.navigate(['/library'])
          }
        });
      })
    );
  }

  register(user: any) {
    return this.http.post(
      this.baseUrl + "Account/register",
      user,
      { withCredentials: true }
    ).pipe(
      tap(() => {
        this.loadCurrentUser().subscribe({
          next: () => {
            this.router.navigate(['/library'])
          }
        });
      })
    );
  }

  refreshToken(): Observable<any> {
    console.log('🔐 Making refresh token request with cookies...');
    return this.http.post(`${this.baseUrl}Account/refresh-token`, {}, {
      withCredentials: true // Важно для httpOnly кук
    }).pipe(
      tap((response: any) => {
        console.log('✅ Refresh token response received');
        // Для httpOnly кук не нужно сохранять токен в JavaScript
        // Браузер автоматически обновит куки
      }),
      catchError(error => {
        console.log('❌ Refresh token error:', error);
        return throwError(() => error);
      })
    );
  }

  logout(): Observable<any> {
    console.log('🚪 Logging out...');
    return this.http.post(`${this.baseUrl}Account/logout`, {}, {
      withCredentials: true
    }).pipe(
      tap(() => {
        window.location.href = '/login';
      })
    );
  }

  loadCurrentUser() {
    return this.http.get<User>(
      this.baseUrl + "Account/me",
      { withCredentials: true }
    ).pipe(
      tap((user) => {
        this.currentUserSource.next(user);
      })
    );
  }

}
