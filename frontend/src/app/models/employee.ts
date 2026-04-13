export interface Employee {
  id: number;
  userType: 'admin' | 'user';
  username: string;
  employeeName: string;
  age: number;
  dob: string;
  email: string;
  scannerId: string;
  salary?: number;
  certificateCode: string;
}

export interface EmployeeCreateRequest {
  userType: 'admin' | 'user';
  username: string;
  password: string;
  employeeName: string;
  age: number;
  dob: string;
  email: string;
  scannerId: string;
  salary?: number | null;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  role: 'admin' | 'user';
  userId: number;
  name: string;
  username: string;
}
