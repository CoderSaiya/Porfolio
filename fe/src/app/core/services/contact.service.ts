import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {environment} from '../../../enviroments/enviroment';

export interface ContactCreateReq {
  name: string;
  email: string;
  subject: string;
  message: string;
}

export interface SubmitResp {
  ok: boolean;
  id: string;
}

@Injectable({ providedIn: 'root' })
export class ContactService {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  submit(req: ContactCreateReq): Observable<SubmitResp> {
    return this.http.post<SubmitResp>(`${this.base}/api/contact`, req);
  }
}
