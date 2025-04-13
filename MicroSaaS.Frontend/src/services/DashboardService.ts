import api from './api';
import { Dayjs } from 'dayjs';
import { ApiResponse } from './AuthService'; // Reutilizar ApiResponse se aplicável

// Definindo o Enum para Plataformas Sociais
export enum SocialMediaPlatform {
  Instagram = 'Instagram',
  YouTube = 'YouTube',
  TikTok = 'TikTok',
  Facebook = 'Facebook',
  Twitter = 'Twitter',
  LinkedIn = 'LinkedIn',
  Pinterest = 'Pinterest',
  Snapchat = 'Snapchat',
  All = 'All',
}

// Interface para os dados de métricas de performance
export interface PerformanceMetrics {
  id: string; // Guid é string em TS
  creatorId: string;
  platform: SocialMediaPlatform; // Usando o Enum
  date: string; // DateTime é string (ISO 8601)
  followers: number;
  followersGrowth: number;
  totalViews: number; // long pode ser number em TS se não exceder MAX_SAFE_INTEGER
  totalLikes: number;
  totalComments: number;
  totalShares: number;
  engagementRate: number; // decimal é number em TS
  estimatedRevenue: number;
  topPerformingContentIds: string[];
  createdAt: string;
  updatedAt: string;
}

class DashboardService {
  /**
   * Busca métricas de desempenho do dashboard com filtros opcionais.
   * NOTA: A API retorna o array diretamente, não encapsulado em ApiResponse.
   */
  async getDashboardMetrics(
    creatorId: string,
    startDate: Dayjs | null,
    endDate: Dayjs | null,
    platform: string | null
  ): Promise<PerformanceMetrics[]> { // Retorna o array diretamente
    // Inicializa com array vazio em caso de erro
    let metricsData: PerformanceMetrics[] = []; 
    try {
      const params = new URLSearchParams();

      if (startDate) {
        params.append('startDate', startDate.format('YYYY-MM-DD')); // Formato ISO
      }
      if (endDate) {
        params.append('endDate', endDate.format('YYYY-MM-DD')); // Formato ISO
      }
      // Só adiciona o parâmetro platform se não for 'Todos' ou nulo
      if (platform && platform !== 'Todos') {
        params.append('platform', platform);
      }

      console.log(`[DashboardService] Buscando métricas para creator ${creatorId} com params:`, params.toString());

      // Ajustar a URL para incluir o creatorId corretamente
      const url = `/v1/Dashboard/metrics/${creatorId}?${params.toString()}`;
      
      // Espera receber o array diretamente
      const response = await api.get<PerformanceMetrics[]>(url);

      console.log("[DashboardService] Resposta recebida (array direto):", response.data);
      
      // Verifica se a resposta é de fato um array
      if (Array.isArray(response.data)) {
        metricsData = response.data;
      } else {
         console.warn("[DashboardService] Resposta recebida não é um array como esperado.", response.data);
      }

    } catch (error: any) {
      console.error('[DashboardService] Erro ao buscar métricas:', error);
      // Em caso de erro, metricsData permanecerá como array vazio
      // Poderíamos lançar o erro para ser tratado no componente se quiséssemos:
      // throw error;
    }
    return metricsData; // Retorna o array (vazio em caso de erro)
  }
}

export default new DashboardService(); 