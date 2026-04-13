import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../services/api.service';
import { AuthResponse, Employee, EmployeeCreateRequest } from '../models/employee';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  employees: Employee[] = [];
  search = '';
  selectedDownload = '';
  editRecord: Employee | null = null;
  error = '';
  session: AuthResponse | null = null;

  constructor(private api: ApiService, private router: Router) {}

  ngOnInit(): void {
    const raw = localStorage.getItem('session');
    if (!raw) {
      this.router.navigateByUrl('/');
      return;
    }

    this.session = JSON.parse(raw) as AuthResponse;
    this.fetch();
  }

  get isAdmin() {
    return this.session?.role === 'admin';
  }

  fetch() {
    this.api.listEmployees(this.search).subscribe(data => (this.employees = data));
  }

  openEdit(record: Employee) {
    this.editRecord = { ...record };
  }

  saveEdit() {
    if (!this.editRecord) return;
    const update = this.editRecord;
    const payload: EmployeeCreateRequest = {
      userType: update.userType,
      username: update.username,
      password: '',
      employeeName: update.employeeName,
      age: update.age,
      dob: update.dob,
      email: update.email,
      scannerId: update.scannerId,
      salary: update.salary ?? null
    };

    this.api.updateEmployee(update.id, payload).subscribe({
      next: () => {
        this.editRecord = null;
        this.fetch();
      },
      error: err => (this.error = err.error ?? 'Update failed')
    });
  }

  deleteUser(id: number) {
    if (!this.isAdmin) return;
    this.api.deleteEmployee(id).subscribe({
      next: () => this.fetch(),
      error: err => (this.error = err.error ?? 'Delete failed')
    });
  }

  onDownloadChange(id: number, format: string) {
    if (format === 'pdf') this.api.exportPdf(id);
    if (format === 'excel') this.api.exportExcel(id);
  }

  logout() {
    localStorage.removeItem('session');
    this.router.navigateByUrl('/');
  }
}
