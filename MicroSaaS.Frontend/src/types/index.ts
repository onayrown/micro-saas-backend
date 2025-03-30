// Tipos para autenticação
export interface User {
  id: string;
  name: string;
  email: string;
  avatar?: string;
  role: string;
}

export interface AuthState {
  isAuthenticated: boolean;
  user: User | null;
  loading: boolean;
  error: string | null;
  token: string | null;
}

// Tipos para dashboard
export interface DashboardData {
  stats: {
    totalRevenue: number;
    followers: number;
    engagement: number;
    posts: number;
  };
  revenueChart: Array<{
    date: string;
    revenue: number;
  }>;
  engagementByPlatform: Array<{
    platform: string;
    engagement: number;
  }>;
  revenueBySource: Array<{
    name: string;
    value: number;
  }>;
  recommendedTimes: Array<{
    day: string;
    time: string;
    confidence: number;
  }>;
  performingContent: Array<{
    id: number;
    title: string;
    platform: string;
    views: number;
    likes: number;
    engagement: number;
  }>;
}

// Tipos para conteúdo
export enum PostStatus {
  Draft = 'Draft',
  Scheduled = 'Scheduled',
  Published = 'Published',
  Failed = 'Failed',
  Deleted = 'Deleted',
  Cancelled = 'Cancelled',
  Processing = 'Processing'
}

// Plataformas de mídia social suportadas
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

export interface Post {
  id: string;
  title: string;
  content: string;
  platform: string;
  status: PostStatus;
  scheduledDate?: string;
  createdAt: string;
  updatedAt?: string;
  author?: User;
  metrics?: {
    views: number;
    likes: number;
    shares: number;
    comments: number;
  };
}

// Tipos para contas sociais
export interface SocialAccount {
  id: string;
  platform: string;
  handle: string;
  followers: number;
  profileUrl: string;
  isConnected: boolean;
  lastSynced?: string;
  metrics?: {
    engagement: number;
    growth: number;
    reachPerPost: number;
  };
}

// Tipo para contas de mídia social
export interface SocialMediaAccount {
  id: string;
  platform: SocialMediaPlatform;
  handle: string;
  followers: number;
  profileUrl?: string;
  isConnected?: boolean;
  lastSynced?: string;
  creatorId?: string;
  isVerified?: boolean;
  createdAt: string;
  updatedAt?: string;
  metrics?: {
    engagement: number;
    growth: number;
    reachPerPost: number;
  };
}

// Tipo para post de conteúdo
export interface ContentPost {
  id: string;
  title: string;
  content: string;
  platform: SocialMediaPlatform;
  status: PostStatus;
  scheduledDate?: string;
  createdAt: string;
  updatedAt?: string;
  author?: User;
  creatorId?: string;
  metrics?: {
    views: number;
    likes: number;
    shares: number;
    comments: number;
  };
}

// Tipos para análises
export interface RevenueSummary {
  totalRevenue: number;
  currency: string;
  adSenseRevenue: number;
  sponsorshipsRevenue: number;
  affiliateRevenue: number;
  revenueByPlatform: PlatformRevenue[];
  previousPeriodRevenue: number;
  revenueGrowth: number;
  projectedRevenue: number;
}

export interface PlatformRevenue {
  platform: string;
  totalRevenue: number;
  adRevenue: number;
  sponsorshipRevenue: number;
  affiliateRevenue: number;
  revenuePerPost: number;
  revenuePerFollower: number;
  previousPeriodRevenue: number;
  revenueGrowth: number;
}

export interface DailyRevenue {
  date: string;
  totalRevenue: number;
  adRevenue: number;
  sponsorshipRevenue: number;
  affiliateRevenue: number;
  platformBreakdown: {
    [platform: string]: number;
  };
}

// Tipos para agendamento
export interface ScheduleDay {
  date: string;
  posts: Post[];
}

// Tipos para API 
export interface ApiResponse<T> {
  data?: T;
  error?: string;
  status: number;
  message?: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// Tipos para perfil
export interface ProfileUpdateData {
  name?: string;
  email?: string;
  avatar?: string;
  bio?: string;
  timeZone?: string;
}

export interface PasswordChangeData {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

// Tipos para notificações
export interface Notification {
  id: string;
  type: string;
  message: string;
  isRead: boolean;
  createdAt: string;
  data?: any;
}

// Tipos para preferências
export interface UserPreferences {
  emailNotifications: boolean;
  darkMode: boolean;
  language: string;
  timeZone: string;
  currency: string;
} 