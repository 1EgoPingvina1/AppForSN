import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environment/environment';

export interface TwoFactorSetupResponse {
  secretKey: string;
  qrCodeImage: string;
  manualEntryKey: string;
  backupCodes: string[];
}

export interface TwoFactorEnableRequest {
  code: string;
  secret: string;
}

export interface TwoFactorEnableResponse {
  success: boolean;
  message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class TwoFactorAuthService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  generateSetup(): Observable<TwoFactorSetupResponse> {
    return this.http.get<TwoFactorSetupResponse>(`${this.apiUrl}Account/2FA/setup`);
  }

  enable(request: TwoFactorEnableRequest): Observable<TwoFactorEnableResponse> {
    return this.http.post<TwoFactorEnableResponse>(`${this.apiUrl}Account/2FA/enable`, request, { headers: { 'Content-Type': 'application/json' } }
);
  }

  disable(): Observable<any> {
    return this.http.post(`${this.apiUrl}/disable`, {});
  }

  getStatus(): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}Account/2FA/status`);
  }

  generateBackupCodes(): Observable<{ backupCodes: string[] }> {
    return this.http.post<{ backupCodes: string[] }>(`${this.apiUrl}/generate-backup-codes`, {});
  }

  verifyBackupCode(backupCode: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/verify-backup`, { backupCode });
  }
}