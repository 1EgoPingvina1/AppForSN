import { Component, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { HeaderComponent } from "./shared/components/header/header.component";
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { AccountService } from './core/services/account.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    HeaderComponent,
    HttpClientModule,
    
    
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  constructor(private accountService:AccountService){}

  ngOnInit(): void {
    this.accountService.loadCurrentUser().subscribe({
      error: () => {
        this.accountService.logout().subscribe();
      }
    })
  }
}
