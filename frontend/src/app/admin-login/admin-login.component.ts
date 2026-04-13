import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../services/api.service';

@Component({
  selector: 'app-admin-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './admin-login.component.html',
  styleUrl: './admin-login.component.css'
})
export class AdminLoginComponent {
  error = '';

  form = this.fb.group({
    username: ['admin', Validators.required],
    password: ['', Validators.required]
  });

  constructor(private fb: FormBuilder, private api: ApiService, private router: Router) {}

  submit() {
    if (this.form.invalid) {
      this.error = 'Enter admin credentials.';
      return;
    }

    this.api.login(this.form.getRawValue() as { username: string; password: string }).subscribe({
      next: auth => {
        if (auth.role !== 'admin') {
          this.error = 'Only admin is allowed on this page.';
          return;
        }

        localStorage.setItem('session', JSON.stringify(auth));
        this.router.navigateByUrl('/dashboard');
      },
      error: err => (this.error = err.error ?? 'Admin login failed')
    });
  }
}
