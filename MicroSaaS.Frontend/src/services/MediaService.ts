import api from './api';
import { ApiResponse } from '../types/common';

export interface UploadedMedia {
  id: string;
  url: string;
  fileName: string;
  fileType: string;
  fileSize: number;
  width?: number;
  height?: number;
  duration?: number;
  createdAt: string;
}

class MediaService {
  async uploadMedia(files: File[], creatorId: string): Promise<UploadedMedia[]> {
    try {
      const formData = new FormData();
      
      // Adicionar cada arquivo ao FormData
      files.forEach((file, index) => {
        formData.append(`files`, file);
      });
      
      // Adicionar o ID do criador
      formData.append('creatorId', creatorId);
      
      const response = await api.post<ApiResponse<UploadedMedia[]>>(
        '/v1/Media/upload',
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data'
          },
          // Configuração para acompanhar o progresso do upload
          onUploadProgress: (progressEvent) => {
            const percentCompleted = Math.round(
              (progressEvent.loaded * 100) / (progressEvent.total || 100)
            );
            console.log(`Upload progress: ${percentCompleted}%`);
            // Aqui você pode atualizar um estado de progresso se necessário
          }
        }
      );
      
      if (response.data?.success && response.data.data) {
        return response.data.data;
      }
      
      throw new Error(response.data?.message || 'Falha ao fazer upload de mídia');
    } catch (error: any) {
      console.error('Erro ao fazer upload de mídia:', error);
      
      if (error.response && error.response.data) {
        const apiError = error.response.data as ApiResponse<any>;
        throw new Error(apiError.message || 'Erro ao fazer upload de mídia');
      }
      
      throw error;
    }
  }

  async deleteMedia(mediaId: string): Promise<boolean> {
    try {
      const response = await api.delete<ApiResponse<boolean>>(`/v1/Media/${mediaId}`);
      
      if (response.data?.success) {
        return true;
      }
      
      throw new Error(response.data?.message || 'Falha ao excluir mídia');
    } catch (error: any) {
      console.error('Erro ao excluir mídia:', error);
      
      if (error.response && error.response.data) {
        const apiError = error.response.data as ApiResponse<any>;
        throw new Error(apiError.message || 'Erro ao excluir mídia');
      }
      
      throw error;
    }
  }

  async getMediaByCreator(creatorId: string): Promise<UploadedMedia[]> {
    try {
      const response = await api.get<ApiResponse<UploadedMedia[]>>(`/v1/Media/creator/${creatorId}`);
      
      if (response.data?.success && response.data.data) {
        return response.data.data;
      }
      
      return [];
    } catch (error: any) {
      console.error('Erro ao obter mídias do criador:', error);
      return [];
    }
  }
}

export default new MediaService();
