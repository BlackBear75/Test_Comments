import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import {AuthService} from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class RecordService {
  private apiUrl = `${environment.apiUrl}api/Record`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  addRecord(record: any): Observable<any> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${this.authService.getToken()}`);
    return this.http.post(`${this.apiUrl}/add`, record, { headers });
  }
}
