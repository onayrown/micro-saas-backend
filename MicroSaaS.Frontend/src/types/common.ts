/**
 * Tipos comuns utilizados em toda a aplicação
 */

/**
 * Enum para os tipos de plataformas de mídia social
 * Deve estar sincronizado com o backend (MicroSaaS.Shared.Enums.SocialMediaPlatform)
 */
export enum SocialMediaPlatform {
  Instagram = 'Instagram',
  YouTube = 'YouTube',
  TikTok = 'TikTok',
  Facebook = 'Facebook',
  Twitter = 'Twitter',
  LinkedIn = 'LinkedIn',
  Pinterest = 'Pinterest',
  Snapchat = 'Snapchat',
  All = 'All'
}

/**
 * Interface para dados do usuário
 */
export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
}

/**
 * Interface para dados do criador de conteúdo
 */
export interface ContentCreator {
  id: string;
  userId: string;
  name: string;
  bio: string;
  niche: string;
  imageUrl?: string;
}

/**
 * Interface para resposta padrão da API
 */
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors?: string[];
} 