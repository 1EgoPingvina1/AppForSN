import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { TwoFactorAuthService } from '../../../../core/services/TwoFactorAuthService.service';
import { ToastrService } from 'ngx-toastr';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { MatError, MatFormField, MatFormFieldControl, MatHint, MatLabel } from '@angular/material/form-field';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { QRCodeModule } from 'angularx-qrcode';
import { MatInputModule } from '@angular/material/input';

interface TwoFactorSetupResponse{
  secretKey:string;
  qrCodeImage:string;
  manualEntryKey: string;
  backupCodes:string[];
}


@Component({
  selector: 'app-two-factor-setup',
  standalone:true,
  imports:[
    ReactiveFormsModule,
    CommonModule,
    MatIcon,
    MatButton,
    MatError,
    MatHint,
    MatFormField,
    MatLabel,
    MatInputModule,
    MatProgressSpinnerModule,
    QRCodeModule
  ],
  templateUrl: './two-factor-setup.component.html',
  styleUrl: './two-factor-setup.component.css'
})

export class TwoFactorSetupComponent implements OnInit,OnDestroy {

  setupForm!: FormGroup;
  verificationForm!: FormGroup;
  currentStep: 'setup' | 'verification' | 'backup' = 'setup';
  
  twoFactorData: any | null;
  backupCodes: string[] = [];
  isGenerating = false;
  isVerifying = false;
  isEnabled? = false;

  private destroy = new Subject<void>();
  
  constructor(
    private fb: FormBuilder,
    private twoFactorService: TwoFactorAuthService,
    private notificationService: ToastrService,
    private router: Router
  ) {
    this.setupForm = this.createSetupForm();
    this.verificationForm = this.createVerificationForm();
  }

  ngOnInit() {
    this.checkCurrentStatus();
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  createSetupForm(): FormGroup {
    return this.fb.group({
      // Может быть дополнительные настройки в будущем
    });
  }

  createVerificationForm(): FormGroup {
    return this.fb.group({
      verificationCode: ['', [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(6),
        Validators.pattern('^[0-9]*$')
      ]]
    });
  }

  async checkCurrentStatus() {
    try {
      this.isEnabled = await this.twoFactorService.getStatus().toPromise();
      if (this.isEnabled) {
        this.notificationService.error('Двухфакторная аутентификация уже включена');
        this.router.navigate(['/profile/security']);
      }
    } catch (error) {
      this.notificationService.error('Ошибка проверки статуса 2FA');
    }
  }

  async generateSetup() {
    this.isGenerating = true;
    
    try {
      this.twoFactorData = await this.twoFactorService.generateSetup().toPromise();
      this.currentStep = 'verification';
      this.notificationService.success('Данные для настройки получены');
    } catch (error) {
      console.error('Setup generation error:', error);
      this.notificationService.error('Ошибка подключения');
    } finally {
      this.isGenerating = false;
    }
  }

  async verifyAndEnable() {
    if (this.verificationForm.invalid) {
      this.markFormGroupTouched(this.verificationForm);
      return;
    }

    this.isVerifying = true;
    const code = this.verificationForm.get('verificationCode')?.value;

    try {
      const result = await this.twoFactorService.enable({
        code: this.verificationForm.get("verificationCode")?.value,
        secret: this.twoFactorData.secretKey || this.twoFactorData.manualEntryKey
      }).toPromise();
      
      if (result?.success) {
        this.backupCodes = this.twoFactorData?.backupCodes || [];
        this.currentStep = 'backup';
        this.isEnabled = true;
        this.notificationService.success('Двухфакторная аутентификация включена!');
      } else {
        this.notificationService.error('Неверный код проверки');
        this.verificationForm.get('verificationCode')?.setErrors({ invalidCode: true });
      }
    } catch (error: any) {
      console.error('Verification error:', error);
      const message = error.error?.message || 'Ошибка при включении 2FA';
      this.notificationService.error(message);
      this.verificationForm.get('verificationCode')?.setErrors({ invalidCode: true });
    } finally {
      this.isVerifying = false;
    }
  }

  async generateNewBackupCodes() {
    try {
      const result = await this.twoFactorService.generateBackupCodes().toPromise();
      this.backupCodes = result!.backupCodes;
      this.notificationService.success('Новые резервные коды сгенерированы');
    } catch (error) {
      this.notificationService.error('Ошибка генерации резервных кодов');
    }
  }

  copyToClipboard(text: string, type: string) {
    navigator.clipboard.writeText(text).then(() => {
      this.notificationService.success(`${type} скопирован в буфер обмена`);
    }).catch(err => {
      this.notificationService.error('Ошибка копирования в буфер');
    });
  }

  downloadBackupCodes() {
    if (!this.backupCodes.length) return;

    const content = `DTC Social Network - Резервные коды 2FA\n\n` +
                   `Сохраните эти коды в безопасном месте. Каждый код можно использовать только один раз.\n\n` +
                   this.backupCodes.join('\n') +
                   `\n\nСгенерировано: ${new Date().toLocaleDateString('ru-RU')}`;

    const blob = new Blob([content], { type: 'text/plain;charset=utf-8' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'dtc-2fa-backup-codes.txt';
    link.click();
    window.URL.revokeObjectURL(url);
    
    this.notificationService.success('Резервные коды скачаны');
  }

  completeSetup() {
    this.router.navigate(['/profile/security']);
  }

  previousStep() {
    switch (this.currentStep) {
      case 'verification':
        this.currentStep = 'setup';
        break;
      case 'backup':
        this.currentStep = 'verification';
        break;
    }
  }

  private markFormGroupTouched(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  get verificationCode() {
    return this.verificationForm.get('verificationCode');
  }
}
