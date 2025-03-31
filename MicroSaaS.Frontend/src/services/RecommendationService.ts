import api from './api';
import { SocialMediaPlatform } from '../types/common';

interface PostTimeRecommendation {
  dayOfWeek: number;
  timeOfDay: string;
  engagementScore: number;
  confidence: number;
  recommendationReason: string;
}

interface ContentRecommendation {
  id: string;
  title: string;
  description: string;
  score: number;
  category: string;
  recommendationType: string;
  implementationDifficulty: string;
  potentialImpact: string;
}

interface TrendTopic {
  id: string;
  name: string;
  category: string;
  platform: SocialMediaPlatform;
  score: number;
  growthRate: number;
  relatedHashtags: string[];
}

interface ContentAnalysis {
  id: string;
  contentId: string;
  strengths: string[];
  weaknesses: string[];
  improvementSuggestions: string[];
  performanceScore: number;
  engagementPrediction: number;
  reachPrediction: number;
  viralityPotential: number;
}

// Dados simulados para quando a API não estiver disponível
const mockData = {
  bestTimes: [
    {
      dayOfWeek: 1, // Segunda
      timeOfDay: '18:30',
      engagementScore: 85,
      confidence: 0.92,
      recommendationReason: 'Baseado em alto engajamento histórico'
    },
    {
      dayOfWeek: 3, // Quarta
      timeOfDay: '12:15',
      engagementScore: 78,
      confidence: 0.85,
      recommendationReason: 'Momento de pico de atividade do público'
    },
    {
      dayOfWeek: 5, // Sexta
      timeOfDay: '20:00',
      engagementScore: 90,
      confidence: 0.94,
      recommendationReason: 'Horário de maior conversão'
    },
    {
      dayOfWeek: 6, // Sábado
      timeOfDay: '11:00',
      engagementScore: 75,
      confidence: 0.82,
      recommendationReason: 'Maior tempo médio de visualização'
    },
    {
      dayOfWeek: 0, // Domingo
      timeOfDay: '16:45',
      engagementScore: 80,
      confidence: 0.88,
      recommendationReason: 'Alta taxa de compartilhamento'
    }
  ],
  contentRecommendations: [
    {
      id: '1',
      title: 'Aumente Engajamento com Vídeos Curtos',
      description: 'Vídeos de 15-30 segundos têm 3x mais engajamento que conteúdos longos',
      score: 95,
      category: 'Formato',
      recommendationType: 'FORMAT',
      implementationDifficulty: 'Fácil',
      potentialImpact: 'Alto'
    },
    {
      id: '2',
      title: 'Histórias de Bastidores',
      description: 'Compartilhe o processo criativo para aumentar conexão com seguidores',
      score: 85,
      category: 'Conteúdo',
      recommendationType: 'TOPIC',
      implementationDifficulty: 'Médio',
      potentialImpact: 'Médio'
    },
    {
      id: '3',
      title: 'Responda a Comentários',
      description: 'Interagir com 50% dos comentários pode aumentar engajamento em 70%',
      score: 75,
      category: 'Engajamento',
      recommendationType: 'ENGAGEMENT',
      implementationDifficulty: 'Baixo',
      potentialImpact: 'Alto'
    }
  ],
  trendingTopics: [
    {
      id: '1',
      name: 'AI no Cotidiano',
      category: 'Tecnologia',
      platform: SocialMediaPlatform.Instagram,
      score: 92,
      growthRate: 1.8,
      relatedHashtags: ['#AI', '#InteligenciaArtificial', '#FuturoDaTecnologia']
    },
    {
      id: '2',
      name: 'Receitas Rápidas',
      category: 'Culinária',
      platform: SocialMediaPlatform.TikTok,
      score: 88,
      growthRate: 1.5,
      relatedHashtags: ['#ComidaRápida', '#ReceitasFáceis', '#CozinhaExpress']
    },
    {
      id: '3',
      name: 'Fitness em Casa',
      category: 'Saúde',
      platform: SocialMediaPlatform.YouTube,
      score: 85,
      growthRate: 1.3,
      relatedHashtags: ['#FitnessEmCasa', '#TreinoRápido', '#SaúdeBem']
    }
  ]
};

class RecommendationService {
  async getBestTimeToPost(creatorId: string, platform: SocialMediaPlatform): Promise<PostTimeRecommendation[]> {
    try {
      const response = await api.get(`/recommendation/best-times/${creatorId}`, {
        params: { platform }
      });
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter melhores horários para postagem, usando dados simulados:', error);
      // Retornar dados simulados em caso de erro
      return mockData.bestTimes;
    }
  }

  async getBestTimeToPostAllPlatforms(creatorId: string): Promise<Record<SocialMediaPlatform, PostTimeRecommendation[]>> {
    try {
      const response = await api.get(`/recommendation/best-times/${creatorId}/all-platforms`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter melhores horários para todas plataformas, usando dados simulados:', error);
      // Criar dados simulados para todas as plataformas
      const platforms = Object.values(SocialMediaPlatform).filter(p => p !== SocialMediaPlatform.All);
      const result: Record<SocialMediaPlatform, PostTimeRecommendation[]> = {} as any;
      
      platforms.forEach(platform => {
        result[platform] = mockData.bestTimes;
      });
      
      return result;
    }
  }

  async getContentRecommendations(creatorId: string): Promise<ContentRecommendation[]> {
    try {
      const response = await api.get(`/recommendation/content/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter recomendações de conteúdo, usando dados simulados:', error);
      return mockData.contentRecommendations;
    }
  }

  async getTopicRecommendations(creatorId: string): Promise<ContentRecommendation[]> {
    try {
      const response = await api.get(`/recommendation/topics/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter recomendações de tópicos, usando dados simulados:', error);
      return mockData.contentRecommendations.filter(rec => rec.recommendationType === 'TOPIC');
    }
  }

  async getFormatRecommendations(creatorId: string): Promise<ContentRecommendation[]> {
    try {
      const response = await api.get(`/recommendation/formats/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter recomendações de formatos, usando dados simulados:', error);
      return mockData.contentRecommendations.filter(rec => rec.recommendationType === 'FORMAT');
    }
  }

  async getHashtagRecommendations(
    creatorId: string, 
    contentDescription: string, 
    platform: SocialMediaPlatform
  ): Promise<string[]> {
    try {
      const response = await api.get(`/recommendation/hashtags/${creatorId}`, {
        params: { contentDescription, platform }
      });
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter recomendações de hashtags, usando dados simulados:', error);
      return ['#ConteúdoDigital', '#CriadorDeConteúdo', '#MarketingDigital', 
              '#RedesSociais', '#Engajamento', '#Crescimento', platform.toString()];
    }
  }

  async getTrendingTopics(platform: SocialMediaPlatform): Promise<TrendTopic[]> {
    try {
      const response = await api.get('/recommendation/trends', {
        params: { platform }
      });
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter tópicos em tendência, usando dados simulados:', error);
      return mockData.trendingTopics;
    }
  }

  async getNicheTrendingTopics(creatorId: string): Promise<TrendTopic[]> {
    try {
      const response = await api.get(`/recommendation/trends/${creatorId}/niche`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter tópicos em tendência para o nicho, usando dados simulados:', error);
      return mockData.trendingTopics;
    }
  }

  async getMonetizationRecommendations(creatorId: string): Promise<ContentRecommendation[]> {
    try {
      const response = await api.get(`/recommendation/monetization/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter recomendações de monetização, usando dados simulados:', error);
      return [
        {
          id: '4',
          title: 'Parcerias com Marcas',
          description: 'Seu perfil tem potencial para parcerias pagas com marcas do setor',
          score: 90,
          category: 'Monetização',
          recommendationType: 'MONETIZATION',
          implementationDifficulty: 'Médio',
          potentialImpact: 'Alto'
        },
        {
          id: '5',
          title: 'Produtos Digitais',
          description: 'Crie um e-book ou curso online baseado no seu conteúdo mais popular',
          score: 85,
          category: 'Monetização',
          recommendationType: 'MONETIZATION',
          implementationDifficulty: 'Alto',
          potentialImpact: 'Alto'
        }
      ];
    }
  }

  async getAudienceGrowthRecommendations(creatorId: string): Promise<ContentRecommendation[]> {
    try {
      const response = await api.get(`/recommendation/audience-growth/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter recomendações de crescimento de audiência, usando dados simulados:', error);
      return [
        {
          id: '6',
          title: 'Colaborações Cruzadas',
          description: 'Colabore com outros criadores para expandir seu alcance',
          score: 88,
          category: 'Crescimento',
          recommendationType: 'GROWTH',
          implementationDifficulty: 'Médio',
          potentialImpact: 'Alto'
        },
        {
          id: '7',
          title: 'Constância de Publicação',
          description: 'Aumente para 3-4 publicações semanais para melhorar alcance',
          score: 92,
          category: 'Crescimento',
          recommendationType: 'GROWTH',
          implementationDifficulty: 'Baixo',
          potentialImpact: 'Médio'
        }
      ];
    }
  }

  async getEngagementRecommendations(creatorId: string): Promise<ContentRecommendation[]> {
    try {
      const response = await api.get(`/recommendation/engagement/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter recomendações de engajamento, usando dados simulados:', error);
      return [
        {
          id: '8',
          title: 'Enquetes e Perguntas',
          description: 'Use recursos interativos para aumentar o tempo de visualização',
          score: 84,
          category: 'Engajamento',
          recommendationType: 'ENGAGEMENT',
          implementationDifficulty: 'Baixo',
          potentialImpact: 'Médio'
        }
      ];
    }
  }

  async analyzeContent(contentId: string): Promise<ContentAnalysis> {
    try {
      const response = await api.get(`/recommendation/analyze/${contentId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao analisar conteúdo, usando dados simulados:', error);
      return {
        id: '1',
        contentId: contentId,
        strengths: ['Boa narrativa', 'Edição de qualidade', 'Tema relevante'],
        weaknesses: ['Duração muito longa', 'Call to action fraco'],
        improvementSuggestions: ['Reduzir para 2-3 minutos', 'Adicionar call to action mais claro'],
        performanceScore: 75,
        engagementPrediction: 68,
        reachPrediction: 82,
        viralityPotential: 65
      };
    }
  }

  async refreshRecommendations(creatorId: string): Promise<void> {
    try {
      await api.post(`/recommendation/refresh/${creatorId}`);
    } catch (error) {
      console.warn('Erro ao atualizar recomendações:', error);
      // Simulando um atraso para dar feedback ao usuário
      await new Promise(resolve => setTimeout(resolve, 1000));
    }
  }
}

export default new RecommendationService(); 