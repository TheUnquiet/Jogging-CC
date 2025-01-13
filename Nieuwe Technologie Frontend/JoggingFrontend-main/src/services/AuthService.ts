import axios, { AxiosError } from 'axios';
import { Person } from '@/types';

const API_URL = 'http://localhost:5187/api';

interface AuthResponse {
  person: Person;
}

interface RegisterRequest {
  email: string;
  password: string;
  person: Person;
}

interface CheckEmailRequest {
  email: string;
}

interface LoginRequest {
  email: string;
  password: string;
}

interface ErrorResponse {
  message: string;
  [key: string]: any;
}

// Set Axios to include cookies with requests
axios.defaults.withCredentials = true;

function handleError(error: AxiosError<ErrorResponse>): Error | ErrorResponse {
  if (error.response) {
    const data: ErrorResponse = error.response.data;
    console.error('Data:', data);
    console.error('Status:', error.response.status);
    console.error('Headers:', error.response.headers);

    if (error.response.status === 404) {
      return new Error('Resource not found');
    }

    if (error.response.status === 409) {
      return data;
    }

    return new Error(data.message || 'An unknown error occurred');
  } else if (error.request) {
    console.error('No response received:', error.request);
    return new Error('No response from server. Please check if the server is running.');
  } else {
    console.error('Error message:', error.message);
    return new Error(error.message);
  }
}

export default class AuthService {
  static async verifyToken(): Promise<void> {
    try {
      await axios.get(`${API_URL}/auth/verify-token`);
    } catch (error) {
      throw handleError(error as AxiosError<ErrorResponse>);
    }
  }

  static async checkEmail(data: CheckEmailRequest): Promise<void> {
    try {
      await axios.post(`${API_URL}/auth/check-email`, {
        email: data.email
      });
    } catch (error) {
      throw handleError(error as AxiosError<ErrorResponse>);
    }
  }

  static async login(data: LoginRequest): Promise<AuthResponse> {
    try {
      const response = await axios.post<AuthResponse>(`${API_URL}/auth/login`, data);
      return response.data;
    } catch (error) {
      throw handleError(error as AxiosError<ErrorResponse>);
    }
  }

  static async register(data: RegisterRequest): Promise<AuthResponse> {
    try {
      const response = await axios.post<AuthResponse>(`${API_URL}/auth/register`, data);
      return response.data;
    } catch (error) {
      throw handleError(error as AxiosError<ErrorResponse>);
    }
  }

  static async logout(): Promise<void> {
    try {
      await axios.post(`${API_URL}/auth/logout`);
    } catch (error) {
      throw handleError(error as AxiosError<ErrorResponse>);
    }
  }
}