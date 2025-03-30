import api from './api';

interface LoginResponse {
  token: string;
  user: {
    id: string;
    email: string;
    name: string;
  };
}

interface RegisterResponse {
  token: string;
  user: {
    id: string;
    email: string;
    name: string;
  };
}

class AuthService {
  async login(email: string, password: string): Promise<LoginResponse> {
    const response = await api.post('/auth/login', { email, password });
    return response.data.data; // Ajuste conforme o formato de resposta da API
  }

  async register(name: string, email: string, password: string): Promise<RegisterResponse> {
    const response = await api.post('/auth/register', { name, email, password });
    return response.data.data; // Ajuste conforme o formato de resposta da API
  }

  async refreshToken(): Promise<string> {
    const response = await api.post('/auth/refresh-token');
    const { token } = response.data.data;
    localStorage.setItem('token', token);
    return token;
  }

  async logout(): Promise<void> {
    await api.post('/auth/logout');
    localStorage.removeItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}

export default new AuthService(); 