import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Project } from '../models/Project';
import { environment } from '../../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductsService {

  constructor(private http: HttpClient) { }
  baseUrl = environment.apiUrl;

  loadProjects(userId: number) {
    return this.http.get<Project[]>(this.baseUrl + "Project/user-projects",{params: {userId: userId.toString()}})
  }
}
