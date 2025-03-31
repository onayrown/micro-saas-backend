import axios from 'axios';

const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5000/api',
  timeout: 5000, // Adicionando timeout para evitar esperas longas
  headers: {
    'Content-Type': 'application/json'
  }
});

// Interceptor para adicionar o token de autenticação nas requisições
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor para lidar com erros de autenticação
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    // Se não houver resposta (servidor indisponível), apenas rejeita a promessa
    if (!error.response) {
      console.warn('API não disponível:', error.message);
      return Promise.reject(error);
    }
    
    const originalRequest = error.config;
    
    // Se o erro for 401 (Unauthorized) e não for uma tentativa de refresh token
    if (error.response.status === 401 && 
        !originalRequest._retry && 
        !originalRequest.url.includes('auth/refresh-token')) {
      
      originalRequest._retry = true;
      
      try {
        // Tentar atualizar o token
        const response = await axios.post(
          `${process.env.REACT_APP_API_URL || 'http://localhost:5000/api'}/auth/refresh-token`,
          {},
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem('token')}`
            }
          }
        );
        
        const { token } = response.data.data;
        localStorage.setItem('token', token);
        
        // Atualizar o token na requisição original e tentar novamente
        originalRequest.headers.Authorization = `Bearer ${token}`;
        return axios(originalRequest);
      } catch (refreshError) {
        // Se não conseguir atualizar o token, redirecionar para o login
        console.error('Erro ao atualizar token:', refreshError);
        localStorage.removeItem('token');
        
        // Apenas redireciona para login se estiver em ambiente de produção
        if (process.env.NODE_ENV === 'production') {
          window.location.href = '/login';
        }
        return Promise.reject(refreshError);
      }
    }
    
    return Promise.reject(error);
  }
);

export default api; 