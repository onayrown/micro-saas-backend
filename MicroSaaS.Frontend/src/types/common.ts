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
 * Enum para os status de um post
 */
export enum PostStatus {
  Draft = 'draft',
  Scheduled = 'scheduled',
  Processing = 'processing',
  Published = 'published',
  Failed = 'failed'
}

/**
 * Enum para os tipos de conteúdo
 */
export enum ContentType {
  Text = 'text',
  Image = 'image',
  Video = 'video',
  Link = 'link'
}

/**
 * Interface para dados do usuário
 */
export interface User {
  id: string;
  name: string;
  email: string;
  username: string;
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

export interface Tag {
  id: string;
  name: string;
} 