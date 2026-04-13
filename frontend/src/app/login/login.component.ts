import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../services/api.service';
import { EmployeeCreateRequest } from '../models/employee';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  mode: 'login' | 'register' = 'login';
  error = '';

  loginForm = this.fb.group({
    identifier: ['', Validators.required],
    password: ['', Validators.required]
  });

  registerForm = this.fb.group({
    password: ['', [Validators.required, Validators.minLength(6)]],
    employeeName: ['', Validators.required],
    dob: ['', Validators.required],
    address: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    salary: [null as number | null, [Validators.required, Validators.min(0)]]
  });

  constructor(private fb: FormBuilder, private api: ApiService, private router: Router) {}

  switchMode(mode: 'login' | 'register') {
    this.mode = mode;
    this.error = '';
  }

  submitLogin() {
    if (this.loginForm.invalid) {
      this.error = 'Enter valid email/username and password.';
      return;
    }

    const payload = this.loginForm.getRawValue();
    const identifier = payload.identifier?.trim() ?? '';

    this.api.login({
      username: identifier.includes('@') ? '' : identifier,
      email: identifier.includes('@') ? identifier : '',
      password: payload.password ?? ''
    }).subscribe({
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

    const formValue = this.registerForm.getRawValue();
    const payload: EmployeeCreateRequest = {
      userType: 'user',
      salary: formValue.salary!,
      username: formValue.email!,
      password: formValue.password!,
      employeeName: formValue.employeeName!,
      age: 18,
      dob: formValue.dob!,
      address: formValue.address!,
      email: formValue.email!,
      scannerId: 'self-service'
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
