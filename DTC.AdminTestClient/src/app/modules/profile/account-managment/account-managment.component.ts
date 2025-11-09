import { Component } from '@angular/core';
import { AccountService } from '../../../core/services/account.service';
import { User } from '../../../core/models/User';
import { Project } from '../../../core/models/Project';
import { CommonModule } from '@angular/common';
import { ProductsService } from '../../../core/services/products.service';
import { RouterLink } from "@angular/router";
import { MatIcon } from "@angular/material/icon";

@Component({
  selector: 'app-account-managment',
  standalone: true,
  imports: [CommonModule, RouterLink, MatIcon],
  templateUrl: './account-managment.component.html',
  styleUrl: './account-managment.component.css'
})
export class AccountManagmentComponent {
  user: User | null = null;
  projects: Project[] = [];

  constructor(private accountService: AccountService,private projectService:ProductsService) {}

  ngOnInit(): void {
    this.accountService.currentUser$.subscribe(user => {
      this.user = user;
      if (user) {
        this.projectService.loadProjects(user.id).subscribe(project => {
          this.projects = project
        }
        )
      }
    });
  }

  logout() {
    this.accountService.logout().subscribe(user =>{
      this.user = user
    });
  }
}
