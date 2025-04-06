import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import AuthService, { UserProfile } from '../services/AuthService';
// Ajustar importações de tipo conforme necessário
// import { User, ContentCreator } from '../types/common'; 

// Usar UserProfile como base para o tipo do usuário no contexto
interface UserContextData {
  user: UserProfile | null;
  // Manter creator separado ou mesclar com user se a estrutura for a mesma
  creator: UserProfile | null; // Ou tipo específico se diferente
  loading: boolean;
  setUser: (user: UserProfile | null) => void;
  setCreator: (creator: UserProfile | null) => void;
  logout: () => Promise<void>;
}

const UserContext = createContext<UserContextData>({} as UserContextData);

export const UserProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<UserProfile | null>(null);
  const [creator, setCreator] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const checkAuthAndFetchProfile = async () => {
      setLoading(true);
      try {
        if (AuthService.isAuthenticated()) {
          console.log('Usuário autenticado, buscando perfil...');
          const response = await AuthService.getProfile();
          
          if (response.success && response.data) {
            console.log('Perfil do usuário carregado:', response.data);
            const userProfile = response.data;
            
            // Definir o usuário e o criador com os dados da API
            setUser(userProfile);
            setCreator(userProfile); // Assumindo que User e Creator são os mesmos dados aqui
                                  // Ajustar se forem entidades diferentes

            // Opcional: Poderia salvar dados mais completos no localStorage também
            // AuthService.setUser(userProfile); // Sobrescreve o user salvo no login/registro

          } else {
            console.warn('Falha ao buscar perfil:', response.message);
            // Token pode ser inválido, deslogar
            await AuthService.logout();
            setUser(null);
            setCreator(null);
          }
        } else {
          console.log('Nenhum token encontrado, usuário não autenticado.');
          // Garantir que não há usuário/criador no estado
          setUser(null);
          setCreator(null);
        }
      } catch (error) {
        console.error('Erro no checkAuthAndFetchProfile:', error);
        // Em caso de erro na API, deslogar
        await AuthService.logout();
        setUser(null);
        setCreator(null);
      } finally {
        setLoading(false);
      }
    };

    checkAuthAndFetchProfile();
  }, []);

  const handleLogout = async () => {
    setLoading(true);
    try {
      await AuthService.logout();
    } catch (error) {
      console.error('Erro ao fazer logout:', error);
    } finally {
      setUser(null);
      setCreator(null);
      setLoading(false);
      // Redirecionamento pode ser tratado pelo Router agora
      // window.location.href = '/login'; 
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