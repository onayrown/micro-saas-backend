import api from './api';
import { SocialMediaPlatform } from '../types/common';

export interface PerformanceMetric {
  id: string;
  label: string;
  value: number;
  previousValue: number;
  percentageChange: number;
  trend: 'up' | 'down' | 'stable';
  period: string;
}

export interface ContentPerformance {
  id: string;
  title: string;
  platform: SocialMediaPlatform;
  type: string;
  publishDate: string;
  impressions: number;
  engagement: number;
  clickRate: number;
  conversionRate: number;
  revenueGenerated: number;
  engagementRate: number;
}

export interface EngagementData {
  date: string;
  engagement: number;
  impressions: number;
  clicks: number;
  shares: number;
}

export interface PlatformPerformance {
  platform: SocialMediaPlatform;
  followers: number;
  engagement: number;
  growth: number;
  averagePostReach: number;
  postFrequency: number;
}

export interface PerformanceByContent {
  contentType: string;
  engagementRate: number;
  impressions: number;
  interactions: number;
  conversionRate: number;
}

export interface PerformanceByTime {
  dayOfWeek: number;
  hourOfDay: number;
  engagement: number;
  reach: number;
}

export interface AudienceDemographic {
  ageGroup: string;
  percentage: number;
  engagement: number;
}

// Dados simulados para quando a API não estiver disponível
const mockData = {
  dashboardMetrics: [
    {
      id: '1',
      label: 'Engajamento Total',
      value: 48750,
      previousValue: 42300,
      percentageChange: 15.25,
      trend: 'up',
      period: 'último mês'
    },
    {
      id: '2',
      label: 'Seguidores',
      value: 125600,
      previousValue: 118400,
      percentageChange: 6.08,
      trend: 'up',
      period: 'último mês'
    },
    {
      id: '3',
      label: 'Taxa de Conversão',
      value: 3.8,
      previousValue: 4.2,
      percentageChange: -9.52,
      trend: 'down',
      period: 'último mês'
    },
    {
      id: '4',
      label: 'Receita',
      value: 12490,
      previousValue: 10850,
      percentageChange: 15.12,
      trend: 'up',
      period: 'último mês'
    }
  ] as PerformanceMetric[],
  
  contentPerformance: [
    {
      id: '1',
      title: 'Como Aumentar seu Engajamento no Instagram',
      platform: SocialMediaPlatform.Instagram,
      type: 'Vídeo',
      publishDate: '2024-03-15',
      impressions: 28500,
      engagement: 4320,
      clickRate: 4.2,
      conversionRate: 1.8,
      revenueGenerated: 840,
      engagementRate: 15.2
    },
    {
      id: '2',
      title: 'Guia Completo para Criação de Conteúdo',
      platform: SocialMediaPlatform.YouTube,
      type: 'Vídeo',
      publishDate: '2024-03-10',
      impressions: 45200,
      engagement: 6800,
      clickRate: 3.8,
      conversionRate: 2.1,
      revenueGenerated: 1250,
      engagementRate: 15.0
    },
    {
      id: '3',
      title: '5 Dicas para Crescer no TikTok',
      platform: SocialMediaPlatform.TikTok,
      type: 'Vídeo Curto',
      publishDate: '2024-03-22',
      impressions: 52300,
      engagement: 8900,
      clickRate: 5.1,
      conversionRate: 2.5,
      revenueGenerated: 720,
      engagementRate: 17.0
    },
    {
      id: '4',
      title: 'Estratégias de Monetização para Criadores',
      platform: SocialMediaPlatform.YouTube,
      type: 'Vídeo',
      publishDate: '2024-03-05',
      impressions: 18700,
      engagement: 3200,
      clickRate: 4.5,
      conversionRate: 3.2,
      revenueGenerated: 1860,
      engagementRate: 17.1
    },
    {
      id: '5',
      title: 'Como Fazer Lives Atraentes',
      platform: SocialMediaPlatform.Instagram,
      type: 'Live',
      publishDate: '2024-03-18',
      impressions: 12500,
      engagement: 2800,
      clickRate: 2.8,
      conversionRate: 1.5,
      revenueGenerated: 520,
      engagementRate: 22.4
    }
  ] as ContentPerformance[],
  
  engagementData: [
    { date: '2024-01-01', engagement: 3200, impressions: 18000, clicks: 850, shares: 320 },
    { date: '2024-01-15', engagement: 3800, impressions: 21000, clicks: 920, shares: 410 },
    { date: '2024-02-01', engagement: 4100, impressions: 23500, clicks: 980, shares: 450 },
    { date: '2024-02-15', engagement: 4600, impressions: 26200, clicks: 1150, shares: 520 },
    { date: '2024-03-01', engagement: 5200, impressions: 28800, clicks: 1320, shares: 580 },
    { date: '2024-03-15', engagement: 5800, impressions: 32500, clicks: 1480, shares: 640 }
  ] as EngagementData[],
  
  platformPerformance: [
    {
      platform: SocialMediaPlatform.Instagram,
      followers: 58200,
      engagement: 5.2,
      growth: 3.8,
      averagePostReach: 12500,
      postFrequency: 4.2
    },
    {
      platform: SocialMediaPlatform.YouTube,
      followers: 32400,
      engagement: 6.5,
      growth: 2.7,
      averagePostReach: 8700,
      postFrequency: 1.5
    },
    {
      platform: SocialMediaPlatform.TikTok,
      followers: 24800,
      engagement: 8.9,
      growth: 8.2,
      averagePostReach: 15300,
      postFrequency: 5.8
    },
    {
      platform: SocialMediaPlatform.Facebook,
      followers: 42500,
      engagement: 3.1,
      growth: 1.2,
      averagePostReach: 5800,
      postFrequency: 2.1
    }
  ] as PlatformPerformance[],
  
  performanceByContent: [
    {
      contentType: 'Vídeo',
      engagementRate: 15.8,
      impressions: 125000,
      interactions: 19750,
      conversionRate: 2.8
    },
    {
      contentType: 'Imagem',
      engagementRate: 12.2,
      impressions: 95000,
      interactions: 11590,
      conversionRate: 1.5
    },
    {
      contentType: 'Carrossel',
      engagementRate: 18.5,
      impressions: 65000,
      interactions: 12025,
      conversionRate: 3.2
    },
    {
      contentType: 'Story',
      engagementRate: 8.3,
      impressions: 185000,
      interactions: 15355,
      conversionRate: 0.9
    },
    {
      contentType: 'Live',
      engagementRate: 22.5,
      impressions: 35000,
      interactions: 7875,
      conversionRate: 4.1
    }
  ] as PerformanceByContent[],
  
  bestPerformingTimes: [
    { dayOfWeek: 1, hourOfDay: 18, engagement: 22.5, reach: 12800 },
    { dayOfWeek: 2, hourOfDay: 12, engagement: 18.2, reach: 10500 },
    { dayOfWeek: 3, hourOfDay: 20, engagement: 19.8, reach: 11400 },
    { dayOfWeek: 5, hourOfDay: 19, engagement: 23.6, reach: 14200 },
    { dayOfWeek: 0, hourOfDay: 15, engagement: 21.2, reach: 13600 }
  ] as PerformanceByTime[],
  
  demographicData: [
    { ageGroup: '18-24', percentage: 28, engagement: 18.5 },
    { ageGroup: '25-34', percentage: 35, engagement: 16.2 },
    { ageGroup: '35-44', percentage: 22, engagement: 14.8 },
    { ageGroup: '45-54', percentage: 10, engagement: 8.7 },
    { ageGroup: '55+', percentage: 5, engagement: 5.2 }
  ] as AudienceDemographic[]
};

class AnalyticsService {
  async getDashboardMetrics(creatorId: string): Promise<PerformanceMetric[]> {
    try {
      const response = await api.get(`/analytics/dashboard/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter métricas do dashboard, usando dados simulados:', error);
      return mockData.dashboardMetrics;
    }
  }

  async getContentPerformance(creatorId: string): Promise<ContentPerformance[]> {
    try {
      const response = await api.get(`/analytics/content-performance/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter performance de conteúdo, usando dados simulados:', error);
      return mockData.contentPerformance;
    }
  }

  async getEngagementData(creatorId: string, startDate?: string, endDate?: string): Promise<EngagementData[]> {
    try {
      const response = await api.get(`/analytics/engagement/${creatorId}`, {
        params: { startDate, endDate }
      });
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter dados de engajamento, usando dados simulados:', error);
      return mockData.engagementData;
    }
  }

  async getPlatformPerformance(creatorId: string): Promise<PlatformPerformance[]> {
    try {
      const response = await api.get(`/analytics/platform-performance/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter performance por plataforma, usando dados simulados:', error);
      return mockData.platformPerformance;
    }
  }

  async getPerformanceByContentType(creatorId: string): Promise<PerformanceByContent[]> {
    try {
      const response = await api.get(`/analytics/content-types/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter performance por tipo de conteúdo, usando dados simulados:', error);
      return mockData.performanceByContent;
    }
  }

  async getBestPerformingTimes(creatorId: string, platform?: SocialMediaPlatform): Promise<PerformanceByTime[]> {
    try {
      const response = await api.get(`/analytics/best-times/${creatorId}`, {
        params: { platform }
      });
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter melhores horários de performance, usando dados simulados:', error);
      return mockData.bestPerformingTimes;
    }
  }

  async getAudienceDemographics(creatorId: string): Promise<AudienceDemographic[]> {
    try {
      const response = await api.get(`/analytics/demographics/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter dados demográficos, usando dados simulados:', error);
      return mockData.demographicData;
    }
  }

  async generateReport(creatorId: string, startDate: string, endDate: string, platforms: SocialMediaPlatform[]): Promise<string> {
    try {
      const response = await api.post(`/analytics/generate-report/${creatorId}`, {
        startDate,
        endDate,
        platforms
      });
      return response.data.reportUrl;
    } catch (error) {
      console.warn('Erro ao gerar relatório, usando resposta simulada:', error);
      return 'https://exemplo.com/relatorios/relatório-simulado.pdf';
    }
  }
}

export default new AnalyticsService(); 