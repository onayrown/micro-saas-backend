import React, { createContext, useEffect, useState, ReactNode, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import AuthService from '../services/AuthService';
import api from '../services/api';

export interface AuthUser {
  id: string;
  name: string;
  email: string;
  role: string;
}

export interface AuthContextType {
  isAuthenticated: boolean;
  isInitialized: boolean;
  user: AuthUser | null;
  loading: boolean;
  error: string | null;
  login: (email: string, password: string) => Promise<void>;
  register: (name: string, email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  clearError: () => void;
}

export const AuthContext = createContext<AuthContextType | null>(null);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [isInitialized, setIsInitialized] = useState<boolean>(false);
  const navigate = useNavigate();

  // Função para limpar mensagens de erro
  const clearError = useCallback(() => {
    setError(null);
  }, []);

  // Função para carregar o perfil do usuário usando AuthService
  const loadUserProfile = useCallback(async () => {
    try {
      console.log('[AuthContext] Chamando AuthService.getProfile...');
      const response = await AuthService.getProfile();
      console.log('[AuthContext] Resposta de getProfile recebida:', response);

      if (response.success && response.data) {
        console.log('[AuthContext] Sucesso! Dados recebidos em response.data:', response.data);
        
        const userData = {
          id: response.data.id,
          name: response.data.name || response.data.username,
          email: response.data.email,
          role: 'user'
        };
        console.log('[AuthContext] Preparando para chamar setUser com:', userData);
        
        setUser(userData);
        console.log('[AuthContext] setUser chamado com sucesso.');
        return true;
      } else {
         console.warn('[AuthContext] Falha ao carregar perfil ou dados ausentes:', response.message);
      }
      return false;
    } catch (err) {
      console.error('[AuthContext] Erro CRÍTICO ao carregar perfil:', err);
      return false;
    }
  }, []);

  // Inicialização do contexto de autenticação
  useEffect(() => {
    const initAuth = async () => {
      try {
        const token = localStorage.getItem('token');
        
        if (token) {
          // Configura o token nas requisições
          api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
          
          // Tenta carregar o perfil do usuário
          const profileLoaded = await loadUserProfile();
          
          if (!profileLoaded) {
            // Se falhar ao carregar o perfil, tenta renovar o token
            const newToken = await AuthService.refreshToken();
            
            if (newToken) {
              // Se conseguir renovar o token, tenta carregar o perfil novamente
              await loadUserProfile();
            } else {
              // Se não conseguir renovar o token, limpa a autenticação
              AuthService.logout();
            }
          }
        }
      } catch (err) {
        console.error('Falha ao inicializar autenticação:', err);
        localStorage.removeItem('token');
      } finally {
        setIsInitialized(true);
      }
    };

    initAuth();
  }, [loadUserProfile]);

  const login = async (email: string, password: string) => {
    setLoading(true);
    setError(null);

    try {
      const response = await AuthService.login({ email, password });
      
      if (!response.success) {
        throw new Error(response.message || 'Falha ao fazer login');
      }
      
      // Se chegou aqui, o login foi bem-sucedido
      // O token já foi salvo pelo AuthService
      
      // Carrega o perfil do usuário
      await loadUserProfile();
      
      // Redireciona para o dashboard
      navigate('/dashboard');
    } catch (err: any) {
      console.error('Erro de login:', err);
      setError(err.message || 'Falha ao fazer login. Verifique suas credenciais.');
    } finally {
      setLoading(false);
    }
  };

  const register = async (name: string, email: string, password: string) => {
    setLoading(true);
    setError(null);

    try {
      const response = await AuthService.register({ 
        name, 
        email, 
        password,
        username: name
      });
      
      if (!response.success) {
        throw new Error(response.message || 'Falha ao registrar');
      }
      
      // Se chegou aqui, o registro foi bem-sucedido
      // O token já foi salvo pelo AuthService
      
      // Carrega o perfil do usuário
      await loadUserProfile();
      
      // Redireciona para o dashboard
      navigate('/dashboard');
    } catch (err: any) {
      console.error('Erro de registro:', err);
      setError(err.message || 'Falha ao registrar. Verifique os dados informados.');
    } finally {
      setLoading(false);
    }
  };

  const logout = async () => {
    try {
      // Utiliza o AuthService para fazer logout de forma segura
      await AuthService.logout();
      
      // Limpa o estado do usuário
      setUser(null);
      
      // Redireciona para a página de login
      navigate('/login');
    } catch (err) {
      console.error('Erro durante logout:', err);
      // Mesmo com erro, limpa o estado e redireciona
      setUser(null);
      navigate('/login');
    }
  };

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated: !!user,
        isInitialized,
        user,
        loading,
        error,
        login,
        register,
        logout,
        clearError
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}; 