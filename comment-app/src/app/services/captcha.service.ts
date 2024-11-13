import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import {environment} from '@env/environment';
@Injectable({
  providedIn: 'root'
})
export class CaptchaService {
  private apiUrl = `${environment.apiUrl}api/Captcha`;
  constructor(private http: HttpClient) { }


  getCaptchaImage(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/generate`, {
      responseType: 'blob',
      withCredentials: true
    });
  }
}
