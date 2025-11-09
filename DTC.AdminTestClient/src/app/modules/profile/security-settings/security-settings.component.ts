import { Component, OnInit } from '@angular/core';
import { TwoFactorAuthService } from '../../../core/services/TwoFactorAuthService.service';
import { ToastrService } from 'ngx-toastr';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-security-settings',
  standalone: true,
  imports:[
    RouterModule,
    CommonModule,
    MatIcon,
    MatButton,
  ],
  templateUrl: './security-settings.component.html',
  styleUrls: ['./security-settings.component.css']
})
export class SecuritySettingsComponent implements OnInit {
  isTwoFactorEnabled? = false;
  isDisabling = false;

  constructor(
    private twoFactorService: TwoFactorAuthService,
    private notificationService: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadTwoFactorStatus();
  }

  async loadTwoFactorStatus(): Promise<void> {
    try {
      this.isTwoFactorEnabled = await this.twoFactorService.getStatus().toPromise();
    } catch (error) {
      this.notificationService.error('Ошибка загрузки статуса 2FA');
    }
  }

  async disableTwoFactor(): Promise<void> {
    const confirmed = confirm('Вы уверены, что хотите отключить двухфакторную аутентификацию? Это снизит безопасность вашего аккаунта.');
    
    if (!confirmed) return;

    this.isDisabling = true;
    try {
      await this.twoFactorService.disable().toPromise();
      this.isTwoFactorEnabled = false;
      this.notificationService.success('Двухфакторная аутентификация отключена');
    } catch (error) {
      this.notificationService.error('Ошибка при отключении 2FA');
    } finally {
      this.isDisabling = false;
    }
  }
}