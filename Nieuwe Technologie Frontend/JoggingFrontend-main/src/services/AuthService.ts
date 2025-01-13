import axios, { AxiosError } from 'axios';
import { Person } from '@/types';

const API_URL = 'http://localhost:5187';

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

interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
}

interface ErrorResponse {
  message: string;
  [key: string]: any;
}

function handleError(error: AxiosError<ErrorResponse>): Error {
  if (error.response) {
    console.error('Data:', error.response.data);
    console.error('Status:', error.response.status);
    console.error('Headers:', error.response.headers);

    if (error.response.status === 404) {
      return new Error('Resource not found');
    }

    if (error.response.status === 409) {
      return new Error(error.response.data.message || 'Conflict error');
    }

    return new Error(error.response.data.message || 'An unknown error occurred');
  } else if (error.request) {
    console.error('No response received:', error.request);
    return new Error('No response from server');
  } else {
    console.error('Error message:', error.message);
    return new Error(error.message);
  }
}

class AuthService {
  static async login(credentials: LoginRequest): Promise<AuthResponse> {
    try {
      const response = await axios.post<AuthResponse>(
        `${API_URL}/login`,
        credentials
      );
      // The cookie should be handled automatically
      return response.data;
    } catch (error) {
      throw handleError(error as AxiosError<ErrorResponse>);
    }
  }

  static async logout(): Promise<void> {
    try {
      await axios.post(`${API_URL}/logout`);
      // The cookie should be invalidated automatically
    } catch (error) {
      throw handleError(error as AxiosError<ErrorResponse>);
    }
  }

  static async checkEmail(email: string): Promise<void> {
    try {
      const response = await axios.post(`${API_URL}/check-email`, { email });
      console.log('Email check response:', response.data);
    } catch (error) {
      throw handleError(error as AxiosError<ErrorResponse>);
    }
  }

  static async register(request: RegisterRequest): Promise<AuthResponse> {
    try {
      const response = await axios.post<AuthResponse>(
        `${API_URL}/register`,
        request
      );
      return response.data;
    } catch (error) {
      throw handleError(error as AxiosError<ErrorResponse>);
    }
  }

  static async changePassword(request: ChangePasswordRequest): Promise<void> {
    try {
      await axios.post(`${API_URL}/change-password`, request);
    } catch (error) {
      throw handleError(error as AxiosError<ErrorResponse>);
    }
  }

  static async verifyToken(): Promise<AuthResponse> {
    try {
      const response = await axios.get<AuthResponse>(`${API_URL}/verify-token`, {
        withCredentials: true,
      });
      return response.data;
    } catch (error) {
      throw handleError(error as AxiosError<ErrorResponse>);
    }
  }
}

export default AuthService;