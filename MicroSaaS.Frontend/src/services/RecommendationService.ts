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
  // Padrão para processar respostas da API
  private processApiResponse<T>(response: any): T {
    // Verificar se a resposta segue o padrão { success: boolean, data: T, message: string }
    if (response.success && response.data) {
      return response.data;
    }
    return response; // Se não seguir o padrão, retorna a resposta direta
  }

  async getBestTimeToPost(creatorId: string, platform: SocialMediaPlatform): Promise<PostTimeRecommendation[]> {
    try {
      const response = await api.get(`/v1/Recommendation/best-times/${creatorId}`, {
        params: { platform }
      });
      return this.processApiResponse(response.data);
    } catch (error) {
      console.warn('Erro ao obter melhores horários para postagem, usando dados simulados:', error);
      // Retornar dados simulados em caso de erro
      return mockData.bestTimes;
    }
  }

  async getBestTimeToPostAllPlatforms(creatorId: string): Promise<Record<SocialMediaPlatform, PostTimeRecommendation[]>> {
    try {
      const response = await api.get(`/v1/Recommendation/best-times/${creatorId}/all-platforms`);
      return this.processApiResponse(response.data);
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
      const response = await api.get(`/v1/Recommendation/content-recommendations/${creatorId}`);
      return this.processApiResponse(response.data);
    } catch (error) {
      console.warn('Erro ao obter recomendações de conteúdo, usando dados simulados:', error);
      return mockData.contentRecommendations;
    }
  }

  async getTopicRecommendations(creatorId: string): Promise<ContentRecommendation[]> {
    try {
      const response = await api.get(`/v1/Recommendation/topic-suggestions/${creatorId}`);
      return this.processApiResponse(response.data);
    } catch (error) {
      console.warn('Erro ao obter recomendações de tópicos, usando dados simulados:', error);
      return mockData.contentRecommendations.filter(rec => rec.recommendationType === 'TOPIC');
    }
  }

  async getFormatRecommendations(creatorId: string): Promise<ContentRecommendation[]> {
    try {
      const response = await api.get(`/v1/Recommendation/format-suggestions/${creatorId}`);
      return this.processApiResponse(response.data);
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
      const response = await api.get(`/v1/Recommendation/hashtags/${creatorId}`, {
        params: { contentDescription, platform }
      });
      return this.processApiResponse(response.data);
    } catch (error) {
      console.warn('Erro ao obter recomendações de hashtags, usando dados simulados:', error);
      return ['#ConteúdoDigital', '#CriadorDeConteúdo', '#MarketingDigital', 
              '#RedesSociais', '#Engajamento', '#Crescimento', platform.toString()];
    }
  }

  async getTrendingTopics(platform: SocialMediaPlatform): Promise<TrendTopic[]> {
    try {
      const response = await api.get('/v1/Recommendation/trends', {
        params: { platform }
      });
      return this.processApiResponse(response.data);
    } catch (error) {
      console.warn('Erro ao obter tópicos em tendência, usando dados simulados:', error);
      return mockData.trendingTopics;
    }
  }

  async getNicheTrendingTopics(creatorId: string): Promise<TrendTopic[]> {
    try {
      const response = await api.get(`/v1/Recommendation/trends/${creatorId}/niche`);
      return this.processApiResponse(response.data);
    } catch (error) {
      console.warn('Erro ao obter tópicos em tendência para o nicho, usando dados simulados:', error);
      return mockData.trendingTopics;
    }
  }

  async getMonetizationRecommendations(creatorId: string): Promise<ContentRecommendation[]> {
    try {
      const response = await api.get(`/v1/Recommendation/monetization/${creatorId}`);
      return this.processApiResponse(response.data);
    } catch (error) {
      console.warn('Erro ao obter recomendações de monetização, usando dados simulados:', error);
      return [
        {
          id: '4',
          title: 'Produtos Digitais',
          description: 'Crie um kit de edição para seus seguidores',
          score: 85,
          category: 'Monetização',
          recommendationType: 'MONETIZATION',
          implementationDifficulty: 'Médio',
          potentialImpact: 'Alto'
        },
        {
          id: '5',
          title: 'Parcerias com Marcas',
          description: 'Identifique 3-5 marcas alinhadas com seu conteúdo',
          score: 90,
          category: 'Monetização',
          recommendationType: 'MONETIZATION',
          implementationDifficulty: 'Médio',
          potentialImpact: 'Alto'
        }
      ];
    }
  }

  async getAudienceGrowthRecommendations(creatorId: string): Promise<ContentRecommendation[]> {
    try {
      const response = await api.get(`/v1/Recommendation/audience-growth/${creatorId}`);
      return this.processApiResponse(response.data);
    } catch (error) {
      console.warn('Erro ao obter recomendações de crescimento de audiência, usando dados simulados:', error);
      return [
        {
          id: '6',
          title: 'Consistência no Instagram',
          description: 'Poste 4-5 vezes por semana para maximizar alcance',
          score: 90,
          category: 'Crescimento',
          recommendationType: 'GROWTH',
          implementationDifficulty: 'Médio',
          potentialImpact: 'Alto'
        },
        {
          id: '7',
          title: 'Colaborações Estratégicas',
          description: 'Parcerias com criadores similares podem trazer 30% de novos seguidores',
          score: 85,
          category: 'Crescimento',
          recommendationType: 'GROWTH',
          implementationDifficulty: 'Difícil',
          potentialImpact: 'Alto'
        }
      ];
    }
  }

  async getEngagementRecommendations(creatorId: string): Promise<ContentRecommendation[]> {
    try {
      const response = await api.get(`/v1/Recommendation/engagement/${creatorId}`);
      return this.processApiResponse(response.data);
    } catch (error) {
      console.warn('Erro ao obter recomendações de engajamento, usando dados simulados:', error);
      return [
        {
          id: '8',
          title: 'Perguntas Estratégicas',
          description: 'Inclua uma pergunta no final dos seus posts para aumentar 60% de comentários',
          score: 85,
          category: 'Engajamento',
          recommendationType: 'ENGAGEMENT',
          implementationDifficulty: 'Fácil',
          potentialImpact: 'Médio'
        },
        {
          id: '9',
          title: 'Stories Interativos',
          description: 'Use enquetes e caixas de perguntas para aumentar interações',
          score: 80,
          category: 'Engajamento',
          recommendationType: 'ENGAGEMENT',
          implementationDifficulty: 'Fácil',
          potentialImpact: 'Médio'
        }
      ];
    }
  }

  async analyzeContent(contentId: string): Promise<ContentAnalysis> {
    try {
      const response = await api.get(`/v1/Recommendation/analyze/${contentId}`);
      return this.processApiResponse(response.data);
    } catch (error) {
      console.warn('Erro ao analisar conteúdo, usando dados simulados:', error);
      return {
        id: 'analysis-1',
        contentId,
        strengths: ['Título atrativo', 'Imagens de alta qualidade', 'Mensagem clara'],
        weaknesses: ['Call-to-action fraco', 'Pouca originalidade'],
        improvementSuggestions: ['Adicione um CTA mais forte', 'Inclua uma história pessoal'],
        performanceScore: 78,
        engagementPrediction: 4.2,
        reachPrediction: 2800,
        viralityPotential: 65
      };
    }
  }

  async refreshRecommendations(creatorId: string): Promise<void> {
    try {
      await api.post(`/v1/Recommendation/refresh/${creatorId}`);
      console.log('Recomendações atualizadas com sucesso');
    } catch (error) {
      console.error('Erro ao atualizar recomendações:', error);
      throw error;
    }
  }
}

export default new RecommendationService(); 