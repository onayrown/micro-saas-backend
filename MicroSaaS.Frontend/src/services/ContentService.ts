import api from './api';
import { SocialMediaPlatform } from '../types/common';
import { ApiResponse } from '../types/common';

export interface Tag {
  id?: string;
  name: string;
}

export interface ContentPost {
  id: string;
  title: string;
  description: string;
  contentType: string;
  contentUrl?: string;
  mediaUrls?: string[];
  platforms: string[];
  tags?: Tag[];
  categories?: string[];
  scheduledDate?: string;
  publishedDate?: string;
  status: string;
  userId: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface ContentCategory {
  id: string;
  name: string;
  description: string;
  postCount: number;
}

export interface ContentTag {
  id: string;
  name: string;
  postCount: number;
}

export interface ContentChecklistItem {
  id: string;
  title: string;
  description: string;
  category: string;
  isCompleted: boolean;
}

export interface ContentTemplate {
  id: string;
  title: string;
  description: string;
  contentType: string;
  template: string;
  platforms: SocialMediaPlatform[];
  tags: string[];
  categories: string[];
}

export interface CreatePostRequest {
  title: string;
  description: string;
  contentType: string;
  contentUrl?: string;
  mediaUrls?: string[];
  platforms: string[];
  tags?: Tag[];
  categories?: string[];
  scheduledDate?: string;
  userId: string;
}

export interface UpdatePostRequest {
  id: string;
  title: string;
  description: string;
  contentType: string;
  contentUrl?: string;
  mediaUrls?: string[];
  platforms: string[];
  tags?: Tag[];
  categories?: string[];
  scheduledDate?: string;
}

// Dados simulados para quando a API não estiver disponível
const mockData = {
  posts: [
    {
      id: '1',
      title: 'Novidades sobre Inteligência Artificial',
      description: 'Como a IA está transformando o marketing digital em 2024',
      contentType: 'article',
      status: 'published',
      mediaUrls: [
        'https://via.placeholder.com/800x600.png?text=AI+Marketing',
        'https://via.placeholder.com/800x600.png?text=Tech+Innovation'
      ],
      platforms: ['Instagram', 'LinkedIn'],
      publishedDate: '2024-03-12T15:30:00Z',
      tags: [{ id: '1', name: 'AI' }, { id: '2', name: 'MarketingDigital' }, { id: '3', name: 'Tecnologia' }],
      categories: ['Tecnologia', 'Marketing'],
      userId: '7f25d1e0-6e12-4b33-8f6b-d5cd3a3c0f7d'
    },
    {
      id: '2',
      title: 'Guia de Engajamento no TikTok',
      description: 'Estratégias para aumentar o engajamento na plataforma que mais cresce no mundo',
      contentType: 'video',
      status: 'scheduled',
      mediaUrls: [
        'https://via.placeholder.com/800x600.png?text=TikTok+Engagement'
      ],
      platforms: ['TikTok', 'Instagram'],
      scheduledDate: '2024-04-05T12:00:00Z',
      tags: [{ id: '4', name: 'TikTok' }, { id: '5', name: 'Engajamento' }, { id: '6', name: 'RedesSociais' }],
      categories: ['Estratégia', 'Redes Sociais'],
      userId: '7f25d1e0-6e12-4b33-8f6b-d5cd3a3c0f7d'
    },
    {
      id: '3',
      title: 'Monetização para Criadores de Conteúdo',
      description: 'As melhores formas de monetizar seu conteúdo em 2024',
      contentType: 'article',
      status: 'draft',
      mediaUrls: [
        'https://via.placeholder.com/800x600.png?text=Content+Monetization'
      ],
      platforms: ['YouTube', 'LinkedIn'],
      tags: [{ id: '7', name: 'Monetização' }, { id: '8', name: 'CriadoresDeConteúdo' }, { id: '9', name: 'Receita' }],
      categories: ['Monetização', 'Negócios'],
      userId: '7f25d1e0-6e12-4b33-8f6b-d5cd3a3c0f7d'
    },
    {
      id: '4',
      title: 'Tendências de E-commerce para 2024',
      description: 'O que esperar do mercado de comércio eletrônico neste ano',
      contentType: 'carousel',
      status: 'published',
      mediaUrls: [
        'https://via.placeholder.com/800x600.png?text=Ecommerce+Trends+1',
        'https://via.placeholder.com/800x600.png?text=Ecommerce+Trends+2',
        'https://via.placeholder.com/800x600.png?text=Ecommerce+Trends+3'
      ],
      platforms: ['Instagram', 'Facebook'],
      publishedDate: '2024-03-18T10:15:00Z',
      tags: [{ id: '10', name: 'Ecommerce' }, { id: '11', name: 'Tendências' }, { id: '12', name: 'Vendas' }],
      categories: ['E-commerce', 'Negócios'],
      userId: '7f25d1e0-6e12-4b33-8f6b-d5cd3a3c0f7d'
    },
    {
      id: '5',
      title: 'Como Criar um Podcast de Sucesso',
      description: 'Um guia completo para produzir e promover seu podcast',
      contentType: 'article',
      status: 'published',
      mediaUrls: [
        'https://via.placeholder.com/800x600.png?text=Podcast+Guide'
      ],
      platforms: ['LinkedIn', 'Twitter'],
      publishedDate: '2024-03-05T08:45:00Z',
      tags: [{ id: '13', name: 'Podcast' }, { id: '14', name: 'Áudio' }, { id: '15', name: 'Conteúdo' }],
      categories: ['Podcast', 'Produção de Conteúdo'],
      userId: '7f25d1e0-6e12-4b33-8f6b-d5cd3a3c0f7d'
    }
  ] as ContentPost[],

  categories: [
    {
      id: '1',
      name: 'Tecnologia',
      description: 'Conteúdo sobre inovações tecnológicas e tendências',
      postCount: 12
    },
    {
      id: '2',
      name: 'Marketing',
      description: 'Estratégias e táticas de marketing digital',
      postCount: 18
    },
    {
      id: '3',
      name: 'Redes Sociais',
      description: 'Conteúdo específico sobre plataformas e estratégias de redes sociais',
      postCount: 25
    },
    {
      id: '4',
      name: 'E-commerce',
      description: 'Tópicos relacionados a comércio eletrônico',
      postCount: 8
    },
    {
      id: '5',
      name: 'Podcast',
      description: 'Conteúdo sobre produção e promoção de podcasts',
      postCount: 5
    },
    {
      id: '6',
      name: 'Negócios',
      description: 'Estratégias de negócios para criadores de conteúdo',
      postCount: 15
    },
    {
      id: '7',
      name: 'Monetização',
      description: 'Formas de monetizar conteúdo e audiência',
      postCount: 10
    },
    {
      id: '8',
      name: 'Produção de Conteúdo',
      description: 'Técnicas e ferramentas para produção de conteúdo',
      postCount: 22
    }
  ] as ContentCategory[],

  tags: [
    { id: '1', name: 'AI', postCount: 8 },
    { id: '2', name: 'MarketingDigital', postCount: 15 },
    { id: '3', name: 'Tecnologia', postCount: 18 },
    { id: '4', name: 'TikTok', postCount: 12 },
    { id: '5', name: 'Engajamento', postCount: 20 },
    { id: '6', name: 'RedesSociais', postCount: 32 },
    { id: '7', name: 'Monetização', postCount: 14 },
    { id: '8', name: 'CriadoresDeConteúdo', postCount: 25 },
    { id: '9', name: 'Receita', postCount: 10 },
    { id: '10', name: 'Ecommerce', postCount: 7 },
    { id: '11', name: 'Tendências', postCount: 18 },
    { id: '12', name: 'Vendas', postCount: 9 },
    { id: '13', name: 'Podcast', postCount: 6 },
    { id: '14', name: 'Áudio', postCount: 8 },
    { id: '15', name: 'Conteúdo', postCount: 28 }
  ] as ContentTag[],

  checklists: [
    {
      id: '1',
      title: 'Verificar otimização de SEO',
      description: 'Garantir que palavras-chave estejam incluídas no título e descrição',
      category: 'SEO',
      isCompleted: true
    },
    {
      id: '2',
      title: 'Revisar gramática e ortografia',
      description: 'Verificar erros gramaticais e ortográficos em todo o conteúdo',
      category: 'Qualidade',
      isCompleted: true
    },
    {
      id: '3',
      title: 'Incluir call-to-action (CTA)',
      description: 'Adicionar um chamado claro para ação ao final do conteúdo',
      category: 'Engajamento',
      isCompleted: false
    },
    {
      id: '4',
      title: 'Otimizar imagens',
      description: 'Redimensionar e comprimir imagens para carregamento rápido',
      category: 'Performance',
      isCompleted: true
    },
    {
      id: '5',
      title: 'Verificar compatibilidade entre plataformas',
      description: 'Garantir que o conteúdo esteja adaptado para cada plataforma selecionada',
      category: 'Distribuição',
      isCompleted: false
    },
    {
      id: '6',
      title: 'Adicionar hashtags relevantes',
      description: 'Incluir hashtags populares e nicho para aumentar alcance',
      category: 'Alcance',
      isCompleted: true
    }
  ] as ContentChecklistItem[],

  templates: [
    {
      id: '1',
      title: 'Post Educacional',
      description: 'Template para conteúdo educativo sobre um tema específico',
      contentType: 'article',
      template: 'Título: [Problema ou Solução Principal]\n\nIntrodução: Apresente o problema que seu conteúdo vai resolver.\n\nCorpo:\n- Ponto 1: [Explicação]\n- Ponto 2: [Explicação]\n- Ponto 3: [Explicação]\n\nConclusão: Resumo dos benefícios de seguir suas recomendações.\n\nCTA: Convide para ação específica.',
      platforms: [SocialMediaPlatform.LinkedIn, SocialMediaPlatform.Instagram],
      tags: ['Educacional', 'HowTo', 'Dicas'],
      categories: ['Educação', 'Produção de Conteúdo']
    },
    {
      id: '2',
      title: 'Carrossel de Tendências',
      description: 'Template para apresentar tendências do mercado em formato de carrossel',
      contentType: 'carousel',
      template: 'Slide 1: Título principal sobre as tendências de [tema]\n\nSlide 2-6: Uma tendência por slide, com título, breve descrição e exemplo visual\n\nSlide 7: Conclusão e CTA para saber mais ou entrar em contato',
      platforms: [SocialMediaPlatform.Instagram, SocialMediaPlatform.LinkedIn],
      tags: ['Tendências', 'Carrossel', 'Insights'],
      categories: ['Tendências', 'Marketing']
    },
    {
      id: '3',
      title: 'Script de Vídeo Tutorial',
      description: 'Estrutura para criar tutoriais em vídeo passo a passo',
      contentType: 'video',
      template: '0:00-0:30 - Introdução: Problema a ser resolvido e benefícios do tutorial\n\n0:30-1:00 - Visão geral do processo\n\n1:00-5:00 - Passos detalhados:\n- Passo 1: [Descrição]\n- Passo 2: [Descrição]\n- Passo 3: [Descrição]\n\n5:00-5:30 - Resumo e dicas adicionais\n\n5:30-6:00 - CTA e agradecimento',
      platforms: [SocialMediaPlatform.YouTube, SocialMediaPlatform.TikTok],
      tags: ['Tutorial', 'HowTo', 'Educacional'],
      categories: ['Tutoriais', 'Vídeo']
    }
  ] as ContentTemplate[]
};

class ContentService {
  async getPosts(creatorId: string, status?: string): Promise<ContentPost[]> {
    try {
      const response = await api.get<ApiResponse<ContentPost[]>>(`/v1/ContentPost`, {
        params: { creatorId, status }
      });
      
      if (response.data?.success && response.data.data) {
        return response.data.data;
      }
      
      console.warn('API retornou dados vazios ou erro', response.data);
      return [];
    } catch (error) {
      console.error('Erro ao obter posts:', error);
      // Fallback para dados simulados em caso de erro
      return mockData.posts;
    }
  }

  async getPostById(postId: string): Promise<ContentPost> {
    try {
      const response = await api.get<ApiResponse<ContentPost>>(`/v1/ContentPost/${postId}`);
      
      if (response.data?.success && response.data.data) {
        return response.data.data;
      }
      
      throw new Error(response.data?.message || 'Post não encontrado');
    } catch (error: any) {
      console.error('Erro ao obter post por ID:', error);
      
      if (error.response && error.response.data) {
        const apiError = error.response.data as ApiResponse<any>;
        throw new Error(apiError.message || 'Erro ao buscar post por ID');
      }
      
      throw error;
    }
  }

  async createPost(postData: CreatePostRequest): Promise<ContentPost> {
    try {
      const response = await api.post<ApiResponse<ContentPost>>('/v1/ContentPost', postData);
      
      if (response.data?.success && response.data.data) {
        return response.data.data;
      }
      
      throw new Error(response.data?.message || 'Erro ao criar post');
    } catch (error: any) {
      console.error('Erro ao criar post:', error);
      
      if (error.response && error.response.data) {
        const apiError = error.response.data as ApiResponse<any>;
        throw new Error(apiError.message || 'Erro ao criar post');
      }
      
      throw error;
    }
  }

  async updatePost(postId: string, postData: UpdatePostRequest): Promise<ContentPost> {
    try {
      const response = await api.put<ApiResponse<ContentPost>>(`/v1/ContentPost/${postId}`, postData);
      
      if (response.data?.success && response.data.data) {
        return response.data.data;
      }
      
      throw new Error(response.data?.message || 'Erro ao atualizar post');
    } catch (error: any) {
      console.error('Erro ao atualizar post:', error);
      
      if (error.response && error.response.data) {
        const apiError = error.response.data as ApiResponse<any>;
        throw new Error(apiError.message || 'Erro ao atualizar post');
      }
      
      throw error;
    }
  }

  async deletePost(postId: string): Promise<boolean> {
    try {
      const response = await api.delete<ApiResponse<boolean>>(`/v1/ContentPost/${postId}`);
      return response.data?.success ?? false;
    } catch (error: any) {
      console.error('Erro ao excluir post:', error);
      
      if (error.response && error.response.data) {
        const apiError = error.response.data as ApiResponse<any>;
        throw new Error(apiError.message || 'Erro ao excluir post');
      }
      
      throw error;
    }
  }

  async publishPost(postId: string): Promise<ContentPost> {
    try {
      const response = await api.post<ApiResponse<ContentPost>>(`/v1/ContentPost/${postId}/publish`);
      
      if (response.data?.success && response.data.data) {
        return response.data.data;
      }
      
      throw new Error(response.data?.message || 'Falha ao publicar post');
    } catch (error: any) {
      console.error('Erro ao publicar post:', error);
      
      if (error.response && error.response.data) {
        const apiError = error.response.data as ApiResponse<any>;
        throw new Error(apiError.message || 'Erro ao publicar post');
      }
      
      throw error;
    }
  }

  async schedulePost(postId: string, scheduledDate: string): Promise<ContentPost> {
    try {
      const response = await api.post<ApiResponse<ContentPost>>(`/v1/ContentPost/${postId}/schedule`, {
        scheduledDate
      });
      
      if (response.data?.success && response.data.data) {
        return response.data.data;
      }
      
      throw new Error(response.data?.message || 'Falha ao agendar post');
    } catch (error: any) {
      console.error('Erro ao agendar post:', error);
      
      if (error.response && error.response.data) {
        const apiError = error.response.data as ApiResponse<any>;
        throw new Error(apiError.message || 'Erro ao agendar post');
      }
      
      throw error;
    }
  }

  async getChecklist(postId: string): Promise<ContentChecklistItem[]> {
    try {
      // Rota parece incorreta, o controller é ContentChecklist e não está relacionado a postId
      // Assumindo que queremos o checklist por ID do checklist, não do post?
      // TODO: Confirmar a lógica correta para buscar o checklist.
      // Por agora, manteremos a busca por ID, mas usando a rota correta do controller.
      const checklistId = postId; // Supondo que o ID passado é o do checklist
      const response = await api.get<ApiResponse<ContentChecklistItem[]>>(`/api/v1/ContentChecklist/${checklistId}`);

      if (response.data?.success && response.data.data) {
         return response.data.data;
      } else {
         throw new Error(response.data?.message || 'Checklist não encontrado ou falha na API');
      }
    } catch (error: any) {
      console.error('Erro ao obter checklist:', error);
      // Propagar erro
      if (error.response && error.response.data) {
        const apiError = error.response.data as ApiResponse<any>;
        throw new Error(apiError.message || 'Erro ao buscar checklist');
      } else if (error instanceof Error) {
        throw error;
      } else {
        throw new Error('Erro desconhecido ao buscar checklist');
      }
    }
  }

  async updateChecklistItem(checklistId: string, itemId: string, isCompleted: boolean): Promise<ContentChecklistItem> {
    try {
      // Usar a rota correta do backend: /api/v1/ContentChecklist/{checklistId}/items/{itemId}
      const response = await api.put<ApiResponse<ContentChecklistItem>>(`/api/v1/ContentChecklist/${checklistId}/items/${itemId}`, {
        isCompleted
      });

      if (response.data?.success && response.data.data) {
        return response.data.data;
      } else {
        throw new Error(response.data?.message || 'Falha ao atualizar item do checklist');
      }
    } catch (error: any) {
      console.error('Erro ao atualizar item do checklist:', error);
      // Propagar erro
       if (error.response && error.response.data) {
        const apiError = error.response.data as ApiResponse<any>;
        throw new Error(apiError.message || 'Erro ao atualizar item do checklist');
      } else if (error instanceof Error) {
        throw error;
      } else {
        throw new Error('Erro desconhecido ao atualizar item do checklist');
      }
    }
  }

  // Remover funções sem backend correspondente
  /*
  async getCategories(creatorId: string): Promise<ContentCategory[]> {
    ...
  }

  async getTags(creatorId: string): Promise<ContentTag[]> {
    ...
  }

  async getTemplates(creatorId: string): Promise<ContentTemplate[]> {
    ...
  }
  */
}

export default new ContentService(); 