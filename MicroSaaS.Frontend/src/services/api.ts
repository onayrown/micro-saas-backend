import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from 'axios';
import AuthService from './AuthService';

// Configuração do ambiente
let API_BASE_URL = process.env.REACT_APP_API_URL;

// Verificar se a URL está definida
if (!API_BASE_URL) {
  console.error('❌ ERRO: Variável de ambiente REACT_APP_API_URL não definida!');
  console.warn('Usando URL padrão: https://localhost:7171/api');
  API_BASE_URL = 'https://localhost:7171/api';
}

// Flag para controlar se já existe um refresh em andamento
let isRefreshingToken = false;
// Fila de requisições pendentes aguardando o refresh do token
let failedQueue: { resolve: (value: unknown) => void; reject: (reason?: any) => void }[] = [];

// Processamento da fila de requisições após refresh de token
const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });

  failedQueue = [];
};

console.log('🔌 Inicializando API com URL base:', API_BASE_URL);

// Configurar axios para ambiente de desenvolvimento
if (process.env.NODE_ENV === 'development') {
  console.log('🔒 Modo de desenvolvimento: Configurando para ambiente de desenvolvimento');

  // Adicionar log para debug
  console.log('Variáveis de ambiente:', {
    NODE_ENV: process.env.NODE_ENV,
    REACT_APP_API_URL: process.env.REACT_APP_API_URL,
    REACT_APP_IGNORE_SSL: process.env.REACT_APP_IGNORE_SSL
  });
}

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Interceptor para adicionar o token de autenticação nas requisições
api.interceptors.request.use(
  (config) => {
    // Obter o token do localStorage
    const token = localStorage.getItem('token');
    if (token) {
      config.headers = config.headers || {};
      config.headers.Authorization = `Bearer ${token}`;
    }

    // Log detalhado em ambiente de desenvolvimento
    if (process.env.NODE_ENV === 'development' || process.env.REACT_APP_DEBUG === 'true') {
      const fullUrl = `${config.baseURL || ''}${config.url || ''}`;
      console.log(`🚀 Enviando requisição para: ${fullUrl} (${config.method?.toUpperCase() || 'GET'})`);
      console.log('Detalhes da requisição:', {
        url: config.url,
        baseURL: config.baseURL,
        method: config.method,
        headers: config.headers,
        data: config.data,
        params: config.params,
        timeout: config.timeout,
        withCredentials: config.withCredentials
      });
    }

    return config;
  },
  (error) => {
    console.error('Erro ao preparar requisição:', error);
    return Promise.reject(error);
  }
);

// Interceptor para lidar com erros e renovação automática de tokens
api.interceptors.response.use(
  (response: AxiosResponse) => {
    // Log apenas em ambiente de desenvolvimento
    if (process.env.NODE_ENV === 'development') {
      console.log(`✅ Resposta recebida de ${response.config.url}: Status ${response.status}`);
    }
    return response;
  },
  async (error: AxiosError) => {
    // Obter informações da requisição que falhou
    const originalRequest = error.config as AxiosRequestConfig & { _retry?: boolean };
    if (!originalRequest) {
      return Promise.reject(error);
    }

    // Log de erro apenas em desenvolvimento
    if (process.env.NODE_ENV === 'development') {
      const requestUrl = originalRequest.url || 'desconhecido';
      console.warn(`❌ Erro na API (${requestUrl}):`, error.message);
    }

    // Se não houver resposta (servidor indisponível)
    if (!error.response) {
      const networkError = {
        message: error.message,
        friendlyMessage: 'Servidor não disponível. Verifique sua conexão ou tente novamente mais tarde.',
        isNetworkError: true
      };

      // Log detalhado em desenvolvimento
      if (process.env.NODE_ENV === 'development') {
        console.warn('📵 API não disponível ou CORS bloqueando a requisição:', error.message);
        console.error('Detalhes do erro:', {
          url: originalRequest.url,
          method: originalRequest.method,
          baseURL: originalRequest.baseURL,
          headers: originalRequest.headers,
          message: error.message,
          code: error.code,
          stack: error.stack
        });

        // Verificar se é um erro de certificado SSL
        if (error.message.includes('SSL') || error.message.includes('certificate')) {
          console.error('🔒 Erro de certificado SSL detectado. Verifique a configuração de SSL ou use HTTP em desenvolvimento.');
        }
      }

      return Promise.reject(networkError);
    }

    // Tratamento específico para erro de autenticação (401)
    if (
      error.response.status === 401 &&
      !originalRequest._retry &&
      !originalRequest.url?.includes('auth/login') &&
      !originalRequest.url?.includes('auth/register') &&
      !originalRequest.url?.includes('auth/refresh-token')
    ) {
      // Se já estiver fazendo refresh, adiciona a requisição atual à fila
      if (isRefreshingToken) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then(token => {
            originalRequest.headers = {
              ...originalRequest.headers,
              Authorization: `Bearer ${token}`
            };
            return axios(originalRequest);
          })
          .catch(err => {
            return Promise.reject(err);
          });
      }

      // Marca como retry e inicia o processo de refresh
      originalRequest._retry = true;
      isRefreshingToken = true;

      try {
        // Tentar atualizar o token via AuthService
        const newToken = await AuthService.refreshToken();

        if (newToken) {
          // Informa todas as requisições na fila que o token foi atualizado
          processQueue(null, newToken);

          // Refaz a requisição original com o novo token
          originalRequest.headers = {
            ...originalRequest.headers,
            Authorization: `Bearer ${newToken}`
          };

          return axios(originalRequest);
        } else {
          // Falha ao obter novo token, rejeita todas as requisições na fila
          const refreshError = new Error('Falha ao renovar token de autenticação');
          processQueue(refreshError);

          // Deslogar o usuário
          await AuthService.logout();

          // Se estiver em produção, redireciona para login
          if (process.env.NODE_ENV === 'production') {
            window.location.href = '/login';
          }

          return Promise.reject({
            message: 'Sessão expirada',
            friendlyMessage: 'Sua sessão expirou. Por favor, faça login novamente.',
            isAuthError: true
          });
        }
      } catch (refreshError) {
        // Erro no refresh token, rejeita todas as requisições na fila
        processQueue(refreshError);

        // Deslogar o usuário
        await AuthService.logout();

        // Se estiver em produção, redireciona para login
        if (process.env.NODE_ENV === 'production') {
          window.location.href = '/login';
        }

        return Promise.reject({
          message: refreshError instanceof Error ? refreshError.message : 'Erro ao atualizar token',
          friendlyMessage: 'Sua sessão expirou. Por favor, faça login novamente.',
          isAuthError: true
        });
      } finally {
        isRefreshingToken = false;
      }
    }

    // Tratamento para outros códigos de erro HTTP comuns
    if (error.response.status === 403) {
      return Promise.reject({
        message: error.message,
        friendlyMessage: 'Você não tem permissão para acessar este recurso.',
        isAccessError: true
      });
    } else if (error.response.status === 404) {
      return Promise.reject({
        message: error.message,
        friendlyMessage: 'O recurso solicitado não foi encontrado.',
        isNotFoundError: true
      });
    } else if (error.response.status >= 500) {
      return Promise.reject({
        message: error.message,
        friendlyMessage: 'Ocorreu um erro no servidor. Por favor, tente novamente mais tarde.',
        isServerError: true
      });
    }

    // Para outros erros, rejeita a promessa com o erro original
    return Promise.reject(error);
  }
);

export default api;