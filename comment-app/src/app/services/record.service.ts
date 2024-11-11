import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { AuthService } from './auth.service';
import { IRecord, IComment } from '../comments/comments.component';

@Injectable({
  providedIn: 'root'
})
export class RecordService {
  private apiUrl = `${environment.apiUrl}api/Record`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  addRecord(formData: FormData, parentRecordId?: string): Observable<any> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${this.authService.getToken()}`);
    if (parentRecordId) {
      formData.append('parentRecordId', parentRecordId);
    }
    return this.http.post(`${this.apiUrl}/add`, formData, { headers, withCredentials: true });
  }


  getRecords(page: number, pageSize: number, sortField: string, sortDirection: 'asc' | 'desc'): Observable<IRecord[]> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())
      .set('sortField', sortField)
      .set('sortDirection', sortDirection);

    return this.http.get<IRecord[]>(`${this.apiUrl}/paged`, { params });
  }

  getRecordsCount(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/count`, { withCredentials: true });
  }

  getCaptchaImage(): Observable<Blob> {
    return this.http.get(`http://localhost:7092/api/Captcha/generate`, {
      responseType: 'blob',
      withCredentials: true
    });
  }
}
