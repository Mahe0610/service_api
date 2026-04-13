import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../services/api.service';
import { EmployeeCreateRequest } from '../models/employee';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  mode: 'login' | 'register' = 'login';
  error = '';

  loginForm = this.fb.group({
    username: ['', Validators.required],
    password: ['', Validators.required]
  });

  registerForm = this.fb.group({
    username: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(6)]],
    employeeName: ['', Validators.required],
    age: [null as number | null, [Validators.required, Validators.min(18)]],
    dob: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    scannerId: ['', Validators.required]
  });

  constructor(private fb: FormBuilder, private api: ApiService, private router: Router) {}

  switchMode(mode: 'login' | 'register') {
    this.mode = mode;
    this.error = '';
  }

  submitLogin() {
    if (this.loginForm.invalid) {
      this.error = 'Enter valid username and password.';
      return;
    }

    this.api.login(this.loginForm.getRawValue() as { username: string; password: string }).subscribe({
      next: auth => {
        localStorage.setItem('session', JSON.stringify(auth));
        this.router.navigateByUrl('/dashboard');
      },
      error: err => (this.error = err.error ?? 'Login failed')
    });
  }

  submitRegister() {
    if (this.registerForm.invalid) {
      this.error = 'Please complete all registration fields.';
      return;
    }

    const payload: EmployeeCreateRequest = {
      userType: 'user',
      salary: null,
      ...(this.registerForm.getRawValue() as never)
    };

    this.api.register(payload).subscribe({
      next: auth => {
        localStorage.setItem('session', JSON.stringify(auth));
        this.router.navigateByUrl('/dashboard');
      },
      error: err => (this.error = err.error ?? 'Registration failed')
    });
  }
}
