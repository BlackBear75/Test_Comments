import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { AuthService } from './auth.service';
import { IRecord, IComment } from '../comments/comments.component';
import {RecordRequest} from '../add-record/add-record.component';

@Injectable({
  providedIn: 'root'
})
export class RecordService {
  private apiUrl = `${environment.apiUrl}api/Record`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  addRecord(record: RecordRequest): Observable<any> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${this.authService.getToken()}`);
    return this.http.post(`${this.apiUrl}/add`, record, { headers, withCredentials: true });
  }


  getRecords(page: number, pageSize: number): Observable<IRecord[]> {
    return this.http.get<IRecord[]>(`${this.apiUrl}/paged`, {
      params: {
        page: page.toString(),
        pageSize: pageSize.toString()
      },
      withCredentials: true
    });
  }

  getCaptchaImage(): Observable<Blob> {
    return this.http.get(`https://localhost:7092/api/Captcha/generate`, {
      responseType: 'blob',
      withCredentials: true
    });
  }

  getRecordsCount(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/count`, { withCredentials: true });
  }

  addComment(recordId: number, comment: IComment): Observable<IComment> {
    return this.http.post<IComment>(`${this.apiUrl}/${recordId}/add-comment`, comment, { withCredentials: true });
  }
}
