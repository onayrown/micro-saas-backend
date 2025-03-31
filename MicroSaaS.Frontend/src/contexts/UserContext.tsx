import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import AuthService from '../services/AuthService';
import { User, ContentCreator } from '../types/common';

interface UserContextData {
  user: User | null;
  creator: ContentCreator | null;
  loading: boolean;
  setUser: (user: User | null) => void;
  setCreator: (creator: ContentCreator | null) => void;
  logout: () => Promise<void>;
}

const UserContext = createContext<UserContextData>({} as UserContextData);

export const UserProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [creator, setCreator] = useState<ContentCreator | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Verificar se o usuário já está autenticado (pelo token)
    const checkAuth = async () => {
      try {
        if (AuthService.isAuthenticated()) {
          // Aqui normalmente faríamos uma chamada à API para obter o perfil do usuário
          // Por enquanto, vamos simular com dados de exemplo
          const mockUser: User = {
            id: '123',
            name: 'Usuário Teste',
            email: 'usuario@teste.com',
            role: 'creator'
          };

          const mockCreator: ContentCreator = {
            id: '7f25d1e0-6e12-4b33-8f6b-d5cd3a3c0f7d',
            userId: '123',
            name: 'Criador de Conteúdo',
            bio: 'Especialista em marketing digital',
            niche: 'Marketing',
            imageUrl: 'https://via.placeholder.com/150'
          };

          setUser(mockUser);
          setCreator(mockCreator);
        }
      } catch (error) {
        console.error('Erro ao verificar autenticação:', error);
        // Em caso de erro, deslogar o usuário
        await AuthService.logout();
        setUser(null);
        setCreator(null);
      } finally {
        setLoading(false);
      }
    };

    checkAuth();
  }, []);

  const handleLogout = async () => {
    try {
      await AuthService.logout();
    } catch (error) {
      console.error('Erro ao fazer logout:', error);
    } finally {
      setUser(null);
      setCreator(null);
      window.location.href = '/login';
    }
  };

  return (
    <UserContext.Provider
      value={{ 
        user, 
        creator, 
        loading, 
        setUser, 
        setCreator, 
        logout: handleLogout 
      }}
    >
      {children}
    </UserContext.Provider>
  );
};

export const useUser = () => useContext(UserContext);

export default UserContext; 