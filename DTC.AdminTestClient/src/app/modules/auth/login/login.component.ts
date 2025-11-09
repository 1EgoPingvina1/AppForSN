import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';
import { ToastrModule, ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, 
    FormsModule, 
    RouterModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  constructor(private accountService: AccountService, private router: Router, private toastr: ToastrService) {}

  isLoginMode = true;
  isLoading = false;

  model: any = {
    firstName: '',
    secondName: '',
    lastName: '',
    gender: 'male', // значение по умолчанию
    isAuthor: false,
    birthday: '',
    username: '',
    password: '',
    confirmPassword: '',
    remember: false
  };

  toggleMode() {
    this.isLoginMode = !this.isLoginMode;
    this.model = {
      firstName: '',
      secondName: '',
      lastName: '',
      gender: 'male', 
      isAuthor: false,
      birthday: '',
      username: '',
      password: '',
      confirmPassword: '',
      remember: false
    };
  }
  
  isRegistrationFormValid(): boolean {
    return !!this.model.firstName && 
           !!this.model.lastName && 
           !!this.model.gender && 
           !!this.model.birthday && 
           !!this.model.username && 
           !!this.model.password && 
           this.model.password === this.model.confirmPassword;
  }

  onSubmit() {
    this.isLoading = true;
    if (this.isLoginMode) {
      this.login();
    } else {
      this.register();
    }
  }

  login() {
    const loginData = {
      username: this.model.username,
      password: this.model.password,
      remember: this.model.remember
    };

    this.accountService.login(loginData).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigateByUrl("/");
        this.toastr.success('Вход выполнен успешно!');
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Ошибка входа:', error);
        this.toastr.error('Ошибка входа: ' + (error.error?.message || 'Неизвестная ошибка'));
      }
    });
  }

  register() {
    // Проверка совпадения паролей
    if (this.model.password !== this.model.confirmPassword) {
      this.toastr.error('Пароли не совпадают!');
      this.isLoading = false;
      return;
    }

    const registerData = {
      firstName: this.model.firstName,
      secondName: this.model.secondName,
      lastName: this.model.lastName,
      gender: this.model.gender,
      isAuthor: this.model.isAuthor,
      birthday: this.model.birthday,
      username: this.model.username,
      password: this.model.password
    };

    this.accountService.register(registerData).subscribe({
      next: () => {
        this.isLoading = false;
        this.toastr.success(`Добро пожаловать,${registerData.username}!`);
        this.router.navigateByUrl("/");
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Ошибка регистрации:', error);
        this.toastr.error('Ошибка регистрации: ' + (error.error?.message || 'Неизвестная ошибка'));
      }
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl("/");
    this.toastr.info('Вы вышли из системы');
  }
}