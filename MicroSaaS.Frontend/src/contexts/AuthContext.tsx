import React, { createContext, useState, useEffect, ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';
import { useNavigate } from 'react-router-dom';
import AuthService from '../services/AuthService';

type User = {
  id: string;
  email: string;
  name: string;
  avatar?: string;
};

type AuthContextType = {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (name: string, email: string, password: string) => Promise<void>;
  logout: () => void;
};

export const AuthContext = createContext<AuthContextType>({
  user: null,
  isAuthenticated: false,
  isLoading: true,
  login: async () => {},
  register: async () => {},
  logout: () => {},
});

type AuthProviderProps = {
  children: ReactNode;
};

// Token simulado para desenvolvimento
const MOCK_TOKEN = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyMTIzIiwiZW1haWwiOiJ1c2VyQGV4YW1wbGUuY29tIiwibmFtZSI6IlVzdcOhcmlvIFRlc3RlIiwiaWF0IjoxNjE2MjM5MDIyLCJleHAiOjE2NDYyMzkwMjJ9.qMcqfHze2jJUz6O3GrfC2bXivLM71WLbEYDlrUNt8AI';

export const AuthProvider = ({ children }: AuthProviderProps) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const checkAuth = async () => {
      const token = localStorage.getItem('token');
      if (token) {
        try {
          // Em produção, validar o token com o backend
          const decoded: any = jwtDecode(token);
          setUser({
            id: decoded.sub || decoded.id || 'user-id',
            email: decoded.email || 'user@example.com',
            name: decoded.name || 'Usuário',
            avatar: decoded.avatar || `https://ui-avatars.com/api/?name=${encodeURIComponent(decoded.name || 'Usuário')}&background=random`,
          });
        } catch (error) {
          console.error('Erro ao decodificar token:', error);
          localStorage.removeItem('token');
        }
      }
      setIsLoading(false);
    };

    checkAuth();
  }, []);

  const login = async (email: string, password: string) => {
    setIsLoading(true);
    try {
      // Em ambiente de desenvolvimento, permitir login simulado
      if (process.env.NODE_ENV === 'development') {
        // Simular resposta do servidor
        localStorage.setItem('token', MOCK_TOKEN);
        
        const mockUser = {
          id: 'user123',
          email: email,
          name: 'Usuário Teste',
          avatar: 'https://ui-avatars.com/api/?name=Usuario+Teste&background=random',
        };
        
        setUser(mockUser);
        setIsLoading(false);
        
        // Redirecionar para o dashboard após login bem-sucedido
        navigate('/dashboard');
        return;
      }
      
      // Código para produção - conectar com o backend real
      const response = await AuthService.login(email, password);
      localStorage.setItem('token', response.token);
      
      const decoded: any = jwtDecode(response.token);
      setUser({
        id: decoded.sub || decoded.id,
        email: decoded.email,
        name: decoded.name,
        avatar: decoded.avatar || `https://ui-avatars.com/api/?name=${encodeURIComponent(decoded.name)}&background=random`,
      });
      
      navigate('/dashboard');
    } catch (error) {
      console.error('Erro durante login:', error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const register = async (name: string, email: string, password: string) => {
    setIsLoading(true);
    try {
      // Em ambiente de desenvolvimento, permitir registro simulado
      if (process.env.NODE_ENV === 'development') {
        // Simular resposta do servidor
        localStorage.setItem('token', MOCK_TOKEN);
        
        const mockUser = {
          id: 'user123',
          email: email,
          name: name,
          avatar: `https://ui-avatars.com/api/?name=${encodeURIComponent(name)}&background=random`,
        };
        
        setUser(mockUser);
        setIsLoading(false);
        
        // Redirecionar para o dashboard após registro bem-sucedido
        navigate('/dashboard');
        return;
      }
      
      // Código para produção - conectar com o backend real
      const response = await AuthService.register(name, email, password);
      localStorage.setItem('token', response.token);
      
      const decoded: any = jwtDecode(response.token);
      setUser({
        id: decoded.sub || decoded.id,
        email: decoded.email,
        name: decoded.name,
        avatar: decoded.avatar || `https://ui-avatars.com/api/?name=${encodeURIComponent(decoded.name)}&background=random`,
      });
      
      navigate('/dashboard');
    } catch (error) {
      console.error('Erro durante registro:', error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
    navigate('/login');
  };

  const value = {
    user,
    isAuthenticated: !!user,
    isLoading,
    login,
    register,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}; 