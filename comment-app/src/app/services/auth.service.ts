// auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7092/api/Auth';

  constructor(private http: HttpClient) {}

  register(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data);
  }

  getProfile(): Observable<any> {
    return this.http.get(`${this.apiUrl}/user/profile`);
  }

  login(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, data);
  }
  setLoggedIn(loggedIn: boolean) {
    localStorage.setItem('isLoggedIn', String(loggedIn));
  }
  isLoggedIn(): boolean {
    return localStorage.getItem('isLoggedIn') === 'true';
  }
  logout() {
    localStorage.removeItem('isLoggedIn');
  }
}
