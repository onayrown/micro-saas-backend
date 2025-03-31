import api from './api';
import { SocialMediaPlatform } from '../types/common';

export interface ContentPost {
  id: string;
  title: string;
  description: string;
  contentType: string;
  status: 'draft' | 'scheduled' | 'published' | 'failed';
  mediaUrls: string[];
  platforms: SocialMediaPlatform[];
  scheduledDate?: string;
  publishedDate?: string;
  tags: string[];
  categories: string[];
  engagementMetrics?: {
    likes: number;
    comments: number;
    shares: number;
    views: number;
    clicks: number;
  };
  authorId: string;
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
  mediaUrls: string[];
  platforms: SocialMediaPlatform[];
  scheduledDate?: string;
  tags: string[];
  categories: string[];
  authorId: string;
}

export interface UpdatePostRequest {
  id: string;
  title?: string;
  description?: string;
  contentType?: string;
  mediaUrls?: string[];
  platforms?: SocialMediaPlatform[];
  scheduledDate?: string;
  tags?: string[];
  categories?: string[];
  status?: 'draft' | 'scheduled' | 'published' | 'failed';
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
      platforms: [SocialMediaPlatform.Instagram, SocialMediaPlatform.LinkedIn],
      publishedDate: '2024-03-12T15:30:00Z',
      tags: ['AI', 'MarketingDigital', 'Tecnologia'],
      categories: ['Tecnologia', 'Marketing'],
      engagementMetrics: {
        likes: 542,
        comments: 78,
        shares: 125,
        views: 3250,
        clicks: 820
      },
      authorId: '7f25d1e0-6e12-4b33-8f6b-d5cd3a3c0f7d'
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
      platforms: [SocialMediaPlatform.TikTok, SocialMediaPlatform.Instagram],
      scheduledDate: '2024-04-05T12:00:00Z',
      tags: ['TikTok', 'Engajamento', 'RedesSociais'],
      categories: ['Estratégia', 'Redes Sociais'],
      authorId: '7f25d1e0-6e12-4b33-8f6b-d5cd3a3c0f7d'
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
      platforms: [SocialMediaPlatform.YouTube, SocialMediaPlatform.LinkedIn],
      tags: ['Monetização', 'CriadoresDeConteúdo', 'Receita'],
      categories: ['Monetização', 'Negócios'],
      authorId: '7f25d1e0-6e12-4b33-8f6b-d5cd3a3c0f7d'
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
      platforms: [SocialMediaPlatform.Instagram, SocialMediaPlatform.Facebook],
      publishedDate: '2024-03-18T10:15:00Z',
      tags: ['Ecommerce', 'Tendências', 'Vendas'],
      categories: ['E-commerce', 'Negócios'],
      engagementMetrics: {
        likes: 328,
        comments: 42,
        shares: 87,
        views: 2180,
        clicks: 540
      },
      authorId: '7f25d1e0-6e12-4b33-8f6b-d5cd3a3c0f7d'
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
      platforms: [SocialMediaPlatform.LinkedIn, SocialMediaPlatform.Twitter],
      publishedDate: '2024-03-05T08:45:00Z',
      tags: ['Podcast', 'Áudio', 'Conteúdo'],
      categories: ['Podcast', 'Produção de Conteúdo'],
      engagementMetrics: {
        likes: 215,
        comments: 38,
        shares: 65,
        views: 1850,
        clicks: 370
      },
      authorId: '7f25d1e0-6e12-4b33-8f6b-d5cd3a3c0f7d'
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
      const response = await api.get(`/content/posts/${creatorId}`, {
        params: { status }
      });
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter posts, usando dados simulados:', error);
      
      // Filtrar os posts simulados com base no status, se fornecido
      if (status) {
        return mockData.posts.filter(post => post.status === status);
      }
      
      return mockData.posts;
    }
  }

  async getPostById(postId: string): Promise<ContentPost> {
    try {
      const response = await api.get(`/content/post/${postId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter post por ID, usando dados simulados:', error);
      const post = mockData.posts.find(p => p.id === postId);
      
      if (!post) {
        throw new Error('Post não encontrado');
      }
      
      return post;
    }
  }

  async createPost(data: CreatePostRequest): Promise<ContentPost> {
    try {
      const response = await api.post('/content/posts', data);
      return response.data;
    } catch (error) {
      console.warn('Erro ao criar post, usando resposta simulada:', error);
      
      // Simular a resposta de criação com dados simulados
      const newPost: ContentPost = {
        id: `sim-${Date.now()}`,
        ...data,
        status: 'draft',
        engagementMetrics: {
          likes: 0,
          comments: 0,
          shares: 0,
          views: 0,
          clicks: 0
        }
      };
      
      return newPost;
    }
  }

  async updatePost(data: UpdatePostRequest): Promise<ContentPost> {
    try {
      const response = await api.put(`/content/posts/${data.id}`, data);
      return response.data;
    } catch (error) {
      console.warn('Erro ao atualizar post, usando resposta simulada:', error);
      
      // Encontrar o post simulado para atualizar
      const existingPost = mockData.posts.find(p => p.id === data.id);
      
      if (!existingPost) {
        throw new Error('Post não encontrado');
      }
      
      // Simular a resposta com os dados atualizados
      const updatedPost: ContentPost = {
        ...existingPost,
        ...data
      };
      
      return updatedPost;
    }
  }

  async deletePost(postId: string): Promise<boolean> {
    try {
      await api.delete(`/content/posts/${postId}`);
      return true;
    } catch (error) {
      console.warn('Erro ao excluir post, usando resposta simulada:', error);
      return true; // Simular exclusão bem-sucedida
    }
  }

  async publishPost(postId: string): Promise<ContentPost> {
    try {
      const response = await api.post(`/content/posts/${postId}/publish`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao publicar post, usando resposta simulada:', error);
      
      // Encontrar o post simulado para publicar
      const existingPost = mockData.posts.find(p => p.id === postId);
      
      if (!existingPost) {
        throw new Error('Post não encontrado');
      }
      
      // Simular a resposta com os dados atualizados
      const publishedPost: ContentPost = {
        ...existingPost,
        status: 'published',
        publishedDate: new Date().toISOString()
      };
      
      return publishedPost;
    }
  }

  async getCategories(creatorId: string): Promise<ContentCategory[]> {
    try {
      const response = await api.get(`/content/categories/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter categorias, usando dados simulados:', error);
      return mockData.categories;
    }
  }

  async getTags(creatorId: string): Promise<ContentTag[]> {
    try {
      const response = await api.get(`/content/tags/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter tags, usando dados simulados:', error);
      return mockData.tags;
    }
  }

  async getChecklist(postId: string): Promise<ContentChecklistItem[]> {
    try {
      const response = await api.get(`/content/checklist/${postId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter checklist, usando dados simulados:', error);
      return mockData.checklists;
    }
  }

  async updateChecklistItem(postId: string, itemId: string, isCompleted: boolean): Promise<ContentChecklistItem> {
    try {
      const response = await api.put(`/content/checklist/${postId}/items/${itemId}`, {
        isCompleted
      });
      return response.data;
    } catch (error) {
      console.warn('Erro ao atualizar item do checklist, usando resposta simulada:', error);
      
      // Encontrar o item do checklist simulado para atualizar
      const item = mockData.checklists.find(c => c.id === itemId);
      
      if (!item) {
        throw new Error('Item do checklist não encontrado');
      }
      
      // Simular a resposta com os dados atualizados
      const updatedItem: ContentChecklistItem = {
        ...item,
        isCompleted
      };
      
      return updatedItem;
    }
  }

  async getTemplates(creatorId: string): Promise<ContentTemplate[]> {
    try {
      const response = await api.get(`/content/templates/${creatorId}`);
      return response.data;
    } catch (error) {
      console.warn('Erro ao obter templates, usando dados simulados:', error);
      return mockData.templates;
    }
  }
}

export default new ContentService(); 