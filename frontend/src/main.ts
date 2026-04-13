import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter, Routes } from '@angular/router';
import { AppComponent } from './app/app.component';
import { LoginComponent } from './app/login/login.component';
import { DashboardComponent } from './app/dashboard/dashboard.component';
import { AdminLoginComponent } from './app/admin-login/admin-login.component';

const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'admin-login', component: AdminLoginComponent },
  { path: 'dashboard', component: DashboardComponent }
];

bootstrapApplication(AppComponent, {
  providers: [provideRouter(routes), provideHttpClient()]
}).catch(err => console.error(err));
