import api from './api';

export interface RegisterRequest {
  email: string;
  password: string;
  name: string;
  username: string;
}

export interface LoginRequest {
  email: string;
  password: string;
  username?: string;
}

export interface AuthResponse {
  success: boolean;
  message: string | null;
  user?: {
    id: string;
    username: string;
    email: string;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
  };
  token?: string;
  errors?: Record<string, string>;
}

export interface UserProfile {
  id: string;
  name: string;
  email: string;
  username: string;
  bio?: string;
  profileImageUrl?: string;
  websiteUrl?: string;
  createdAt: string;
  updatedAt: string;
  totalFollowers?: number;
  totalPosts?: number;
  socialMediaAccounts?: any[]; // Ajustar tipo se necessário
}

export interface ApiResponse<T> {
  success: boolean;
  message?: string | null;
  data?: T;
  errors?: Record<string, string>;
}

/**
 * Classe responsável por gerenciar autenticação e usuários
 */
class AuthService {
  private tokenKey = 'token';
  // Não armazenamos dados do usuário no localStorage por questões de segurança
  private refreshLock = false;
  private refreshPromise: Promise<string | null> | null = null;

  /**
   * Registra um novo usuário
   * @param data Dados de registro
   * @returns Resposta da API
   */
  async register(data: RegisterRequest): Promise<AuthResponse> {
    try {
      const response = await api.post<AuthResponse>('/v1/Auth/register', data);

      // Se o registro for bem-sucedido, salva apenas o token
      if (response.data.success && response.data.token) {
        this.setToken(response.data.token);
      }

      return response.data;
    } catch (error: any) {
      console.error('Erro ao registrar usuário:', error);

      // Formatar resposta de erro consistente
      if (error.response && error.response.data) {
        return error.response.data as AuthResponse;
      }

      return {
        success: false,
        message: error.message || 'Erro ao registrar usuário',
      };
    }
  }

  /**
   * Autentica um usuário existente
   * @param data Credenciais de login
   * @returns Resposta da API
   */
  async login(data: LoginRequest): Promise<AuthResponse> {
    try {
      // Cria payload apenas com email e password, garantindo compatibilidade com a API
      const loginPayload = {
        email: data.email,
        password: data.password
      };

      const response = await api.post<AuthResponse>('/v1/Auth/login', loginPayload);

      // Se o login for bem-sucedido, salva apenas o token
      if (response.data.success && response.data.token) {
        this.setToken(response.data.token);
      }

      return response.data;
    } catch (error: any) {
      console.error('Erro ao fazer login:', error);

      // Tratar formato de erro simples { message: string } ou AuthResponse
      if (error.response && error.response.data) {
         // Se a resposta tiver 'success' e 'message', assume AuthResponse
        if (typeof error.response.data.success !== 'undefined') {
           return error.response.data as AuthResponse;
        }
        // Senão, assume o formato { message: string } e cria um AuthResponse
        return {
          success: false,
          message: error.response.data.message || 'Credenciais inválidas ou erro desconhecido',
        };
      }

      // Erro sem resposta da API (ex: rede)
      return {
        success: false,
        message: error.message || 'Erro ao fazer login',
      };
    }
  }

  /**
   * Renova o token de autenticação
   * @returns Novo token ou null em caso de erro
   */
  async refreshToken(): Promise<string | null> {
    // Se já existir uma operação de refresh em andamento, retorna a mesma promise
    if (this.refreshLock && this.refreshPromise) {
      return this.refreshPromise;
    }

    // Obtém o token atual
    const currentToken = this.getToken();
    if (!currentToken) {
      return null;
    }

    // Configura o lock e cria uma nova promise de refresh
    this.refreshLock = true;
    this.refreshPromise = this.executeRefreshToken();

    try {
      // Aguarda o resultado do refresh
      return await this.refreshPromise;
    } finally {
      // Libera o lock quando finalizar
      this.refreshLock = false;
      this.refreshPromise = null;
    }
  }

  /**
   * Executa a operação de refresh token na API
   * @private
   */
  private async executeRefreshToken(): Promise<string | null> {
    try {
      // Certifica-se de que o token atual está no cabeçalho
      const currentToken = this.getToken();
      if (!currentToken) {
        console.error('Nenhum token disponível para refresh');
        return null;
      }

      // Configura o token no cabeçalho de autorização
      api.defaults.headers.common['Authorization'] = `Bearer ${currentToken}`;

      // Faz a requisição para o endpoint de refresh token
      console.log('Tentando renovar token...');
      const response = await api.post<AuthResponse>('/v1/Auth/refresh-token');

      if (response.data.success && response.data.token) {
        console.log('Token renovado com sucesso');
        // Salva o novo token
        this.setToken(response.data.token);

        // Atualiza o cabeçalho de autorização
        api.defaults.headers.common['Authorization'] = `Bearer ${response.data.token}`;

        return response.data.token;
      }

      // Falha no refresh sem erro explícito
      console.warn('Refresh token falhou sem erro explícito:', response.data.message);
      return null;
    } catch (error: any) {
      console.error('Erro ao renovar token:', error);

      // Tratar formato de erro simples { message: string }
      if (error.response && error.response.data && error.response.data.message) {
         console.error('Falha no refresh token (API):', error.response.data.message);
      } else {
         console.error('Erro inesperado ao renovar token:', error.message);
      }

      return null;
    }
  }

  /**
   * Revoga o token de autenticação atual (logout)
   */
  async logout(): Promise<boolean> {
    try {
      const token = this.getToken();

      // Só tenta revogar o token se ele existir
      if (token) {
        // Certifica-se de que o token está no cabeçalho
        api.defaults.headers.common['Authorization'] = `Bearer ${token}`;

        try {
          await api.post('/v1/Auth/revoke-token');
        } catch (error) {
          // Apenas loga o erro, mas continua o processo de logout
          console.warn('Erro ao revogar token no servidor:', error);
        }
      }

      // Limpa os dados locais independentemente do resultado da revogação
      this.clearAuth();
      return true;
    } catch (error) {
      console.error('Erro ao fazer logout:', error);

      // Limpa os dados mesmo se falhar
      this.clearAuth();
      return false;
    }
  }

  /**
   * Verifica se o usuário está autenticado
   */
  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  /**
   * Obtém o token atual
   */
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  /**
   * Define o token de autenticação
   */
  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);

    // Atualiza o cabeçalho de autorização
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  }

  /**
   * Obtém os dados do usuário atual
   * @deprecated Não use este método. Os dados do usuário devem ser obtidos da API.
   */
  getUser(): any {
    console.warn('getUser() está obsoleto. Use o contexto de autenticação para obter dados do usuário.');
    return null;
  }

  /**
   * Limpa os dados de autenticação
   */
  private clearAuth(): void {
    localStorage.removeItem(this.tokenKey);

    // Remove o token dos cabeçalhos da API
    delete api.defaults.headers.common['Authorization'];
  }

  /**
   * Busca os dados do perfil do usuário autenticado
   * @returns Resposta da API com os dados do perfil
   */
  async getProfile(): Promise<ApiResponse<UserProfile>> {
    try {
      // Verificar se o token está disponível
      const token = this.getToken();
      if (!token) {
        console.warn('Token não disponível ao tentar obter perfil do usuário');
        return {
          success: false,
          message: 'Usuário não autenticado'
        };
      }

      // Garantir que o token está no cabeçalho
      api.defaults.headers.common['Authorization'] = `Bearer ${token}`;

      // Endpoint para obter o perfil do usuário autenticado
      console.log('Buscando perfil do usuário autenticado...');
      const response = await api.get<ApiResponse<UserProfile>>('/v1/creators/me');

      // Retorna os dados do perfil sem armazená-los localmente
      // Os dados do usuário são gerenciados pelo contexto de autenticação

      return response.data;
    } catch (error: any) {
      console.error('Erro ao buscar perfil do usuário:', error);

      // Formatar resposta de erro consistente
      if (error.response && error.response.data) {
        // Tentar retornar o formato ApiResponse
        if (typeof error.response.data.success !== 'undefined') {
          return error.response.data as ApiResponse<UserProfile>;
        }
        // Tentar retornar o formato { message: string }
        if (error.response.data.message) {
          return { success: false, message: error.response.data.message };
        }
      }

      return {
        success: false,
        message: error.message || 'Erro ao buscar perfil do usuário',
      };
    }
  }
}

export default new AuthService();