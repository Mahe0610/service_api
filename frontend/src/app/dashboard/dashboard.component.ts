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
    this.api.listEmployees(this.isAdmin ? this.search : undefined).subscribe({
      next: data => {
        this.employees = this.isAdmin
          ? data
          : data.filter(employee => employee.id === this.session?.userId);
      },
      error: err => (this.error = err.error ?? 'Unable to load dashboard data')
    });
  }

  openEdit(record: Employee) {
    if (!this.isAdmin) return;
    this.editRecord = { ...record };
  }

  saveEdit() {
    if (!this.editRecord || !this.isAdmin) return;

    const payload: EmployeeCreateRequest = {
      userType: this.editRecord.userType,
      username: this.editRecord.username,
      password: '',
      employeeName: this.editRecord.employeeName,
      age: this.editRecord.age,
      dob: this.editRecord.dob,
      address: this.editRecord.address,
      email: this.editRecord.email,
      scannerId: this.editRecord.scannerId,
      salary: this.editRecord.salary ?? null
    };

    this.api.updateEmployee(this.editRecord.id, payload).subscribe({
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

  logout() {
    localStorage.removeItem('session');
    this.router.navigateByUrl('/');
  }
}
