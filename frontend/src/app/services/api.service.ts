import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthResponse, Employee, EmployeeCreateRequest, LoginRequest } from '../models/employee';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly api = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  register(payload: EmployeeCreateRequest) {
    return this.http.post<AuthResponse>(`${this.api}/auth/register`, payload);
  }

  login(payload: LoginRequest) {
    return this.http.post<AuthResponse>(`${this.api}/auth/login`, payload);
  }

  listEmployees(search?: string) {
    return this.http.get<Employee[]>(`${this.api}/employees`, { params: search ? { search } : {} });
  }

  updateEmployee(id: number, payload: EmployeeCreateRequest) {
    return this.http.put<Employee>(`${this.api}/employees/${id}`, payload);
  }

  deleteEmployee(id: number) {
    return this.http.delete(`${this.api}/employees/${id}`);
  }

  exportPdf(id: number) {
    window.open(`${this.api}/employees/${id}/certificate/pdf`, '_blank');
  }

  exportExcel(id: number) {
    window.open(`${this.api}/employees/${id}/certificate/excel`, '_blank');
  }
}
