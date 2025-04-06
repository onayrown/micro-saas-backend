import React, { useState, useEffect, useCallback } from 'react';
import dayjs, { Dayjs } from 'dayjs';
import {
  Box,
  TextField,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Chip,
  Typography,
  Grid,
  Paper,
  Stack,
  FormHelperText,
  SelectChangeEvent,
  OutlinedInput,
  CircularProgress,
  Alert,
  AlertTitle,
  Divider,
  Autocomplete
} from '@mui/material';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { ContentPost, CreatePostRequest, UpdatePostRequest } from '../../services/ContentService';
import { SocialMediaPlatform } from '../../types/common';
import { useAuth } from '../../hooks/useAuth';
import MediaUpload from './MediaUpload';
import MediaService, { UploadedMedia } from '../../services/MediaService';

export type ContentFormMode = 'create' | 'edit';

export interface ContentFormProps {
  mode: ContentFormMode;
  post?: ContentPost;
  onSave: (data: CreatePostRequest | UpdatePostRequest) => Promise<void>;
  onCancel: () => void;
  isLoading?: boolean;
  error?: string | null;
}

const initialFormData = {
  title: '',
  description: '',
  contentType: 'text',
  mediaUrls: [],
  platforms: [] as SocialMediaPlatform[],
  scheduledDate: '',
  tags: [],
  categories: []
};

const ContentForm: React.FC<ContentFormProps> = ({
  mode,
  post,
  onSave,
  onCancel,
  isLoading = false,
  error = null
}) => {
  const { user } = useAuth();
  const [formData, setFormData] = useState<any>(initialFormData);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [schedulePost, setSchedulePost] = useState<boolean>(false);
  const [tagInput, setTagInput] = useState<string>('');
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(null);

  // Estados para gerenciamento de mídia
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const [uploadedMediaUrls, setUploadedMediaUrls] = useState<string[]>([]);
  const [isUploading, setIsUploading] = useState<boolean>(false);
  const [uploadError, setUploadError] = useState<string | null>(null);

  // Preencher o formulário com dados existentes quando editando
  useEffect(() => {
    if (mode === 'edit' && post) {
      setFormData({
        id: post.id,
        title: post.title,
        description: post.description,
        contentType: post.contentType,
        mediaUrls: post.mediaUrls || [],
        platforms: post.platforms || [],
        scheduledDate: post.scheduledDate || '',
        tags: post.tags ? post.tags.map(tag => typeof tag === 'string' ? tag : tag.name) : [],
        categories: post.categories || []
      });

      if (post.scheduledDate) {
        setSchedulePost(true);
        setSelectedDate(dayjs(post.scheduledDate));
      }

      // Carregar URLs de mídia existentes
      if (post.mediaUrls && post.mediaUrls.length > 0) {
        setUploadedMediaUrls(post.mediaUrls);
      }
    }
  }, [mode, post]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.title.trim()) {
      newErrors.title = 'O título é obrigatório';
    }

    if (!formData.description.trim()) {
      newErrors.description = 'A descrição é obrigatória';
    }

    if (formData.platforms.length === 0) {
      newErrors.platforms = 'Selecione pelo menos uma plataforma';
    }

    if (schedulePost && !selectedDate) {
      newErrors.scheduledDate = 'Selecione uma data e hora para agendamento';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData((prev: any) => ({
      ...prev,
      [name]: value
    }));

    // Limpar erro do campo quando o usuário digita
    if (errors[name]) {
      setErrors({ ...errors, [name]: '' });
    }
  };

  const handlePlatformChange = (event: SelectChangeEvent<typeof formData.platforms>) => {
    const { value } = event.target;
    setFormData((prev: any) => ({
      ...prev,
      platforms: typeof value === 'string' ? value.split(',') : value
    }));

    if (errors.platforms) {
      setErrors({ ...errors, platforms: '' });
    }
  };

  const handleContentTypeChange = (event: SelectChangeEvent) => {
    setFormData((prev: any) => ({
      ...prev,
      contentType: event.target.value
    }));
  };

  const handleDateChange = (newDate: Dayjs | null) => {
    setSelectedDate(newDate);
    if (newDate) {
      setFormData((prev: any) => ({
        ...prev,
        scheduledDate: newDate.toISOString()
      }));

      if (errors.scheduledDate) {
        setErrors({ ...errors, scheduledDate: '' });
      }
    }
  };

  const handleScheduleToggle = () => {
    setSchedulePost(!schedulePost);
    if (!schedulePost) {
      // Se está ativando o agendamento, manter a data selecionada
    } else {
      // Se está desativando o agendamento, limpar a data
      setSelectedDate(null);
      setFormData((prev: any) => ({
        ...prev,
        scheduledDate: ''
      }));
    }
  };

  const handleAddTag = () => {
    if (tagInput.trim() !== '' && !formData.tags.includes(tagInput.trim())) {
      setFormData((prev: any) => ({
        ...prev,
        tags: [...prev.tags, tagInput.trim()]
      }));
      setTagInput('');
    }
  };

  const handleRemoveTag = (tagToRemove: string) => {
    setFormData((prev: any) => ({
      ...prev,
      tags: prev.tags.filter((tag: string) => tag !== tagToRemove)
    }));
  };

  // Manipuladores para upload de mídia
  const handleMediaSelected = (files: File[]) => {
    setSelectedFiles([...selectedFiles, ...files]);
    setUploadError(null);
  };

  const handleMediaRemoved = (index: number) => {
    const newFiles = [...selectedFiles];
    newFiles.splice(index, 1);
    setSelectedFiles(newFiles);
  };

  const handleUploadMedia = async (): Promise<string[]> => {
    if (selectedFiles.length === 0) {
      return uploadedMediaUrls; // Retorna as URLs já existentes se não houver novos arquivos
    }

    setIsUploading(true);
    setUploadError(null);

    try {
      if (!user?.id) {
        throw new Error('Usuário não identificado');
      }

      const uploadedMedia = await MediaService.uploadMedia(selectedFiles, user.id);
      const newMediaUrls = uploadedMedia.map(media => media.url);

      // Combinar URLs existentes com as novas
      const allMediaUrls = [...uploadedMediaUrls, ...newMediaUrls];
      setUploadedMediaUrls(allMediaUrls);
      setSelectedFiles([]); // Limpar arquivos selecionados após o upload

      return allMediaUrls;
    } catch (error: any) {
      setUploadError(error.message || 'Erro ao fazer upload de mídia');
      return uploadedMediaUrls; // Retorna apenas as URLs existentes em caso de erro
    } finally {
      setIsUploading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    // Fazer upload de mídia primeiro, se houver arquivos selecionados
    let mediaUrls = uploadedMediaUrls;
    if (selectedFiles.length > 0) {
      mediaUrls = await handleUploadMedia();
    }

    const submitData = {
      ...formData,
      authorId: user?.id,
      mediaUrls: mediaUrls
    };

    // Se não estiver agendando, remover a data de agendamento
    if (!schedulePost) {
      delete submitData.scheduledDate;
    }

    try {
      await onSave(submitData);
    } catch (error) {
      console.error('Erro ao salvar post:', error);
    }
  };

  return (
    <Paper elevation={0} sx={{ p: 3, mb: 3 }}>
      <Box component="form" onSubmit={handleSubmit} noValidate>
        <Typography variant="h5" gutterBottom>
          {mode === 'create' ? 'Criar Novo Conteúdo' : 'Editar Conteúdo'}
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 3 }}>
            <AlertTitle>Erro</AlertTitle>
            {error}
          </Alert>
        )}

        <Grid container spacing={3} sx={{ mb: 3 }}>
          <Grid item xs={12}>
            <TextField
              fullWidth
              required
              label="Título"
              name="title"
              value={formData.title}
              onChange={handleChange}
              error={!!errors.title}
              helperText={errors.title}
              disabled={isLoading}
            />
          </Grid>

          <Grid item xs={12}>
            <TextField
              fullWidth
              required
              label="Descrição"
              name="description"
              multiline
              rows={4}
              value={formData.description}
              onChange={handleChange}
              error={!!errors.description}
              helperText={errors.description}
              disabled={isLoading}
            />
          </Grid>

          <Grid item xs={12} sm={6}>
            <FormControl fullWidth required error={!!errors.platforms}>
              <InputLabel id="platforms-label">Plataformas</InputLabel>
              <Select
                labelId="platforms-label"
                multiple
                value={formData.platforms}
                onChange={handlePlatformChange}
                input={<OutlinedInput label="Plataformas" />}
                renderValue={(selected) => (
                  <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                    {selected.map((value: string) => (
                      <Chip key={value} label={value} />
                    ))}
                  </Box>
                )}
                disabled={isLoading}
              >
                {Object.values(SocialMediaPlatform).map((platform) => (
                  <MenuItem key={platform} value={platform}>
                    {platform}
                  </MenuItem>
                ))}
              </Select>
              {errors.platforms && <FormHelperText>{errors.platforms}</FormHelperText>}
            </FormControl>
          </Grid>

          <Grid item xs={12} sm={6}>
            <FormControl fullWidth>
              <InputLabel id="content-type-label">Tipo de Conteúdo</InputLabel>
              <Select
                labelId="content-type-label"
                value={formData.contentType}
                onChange={handleContentTypeChange}
                label="Tipo de Conteúdo"
                disabled={isLoading}
              >
                <MenuItem value="text">Texto</MenuItem>
                <MenuItem value="image">Imagem</MenuItem>
                <MenuItem value="video">Vídeo</MenuItem>
                <MenuItem value="link">Link</MenuItem>
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={12}>
            <Typography variant="subtitle1" gutterBottom>
              Upload de Mídia
            </Typography>
            <MediaUpload
              onMediaSelected={handleMediaSelected}
              onMediaRemoved={handleMediaRemoved}
              selectedFiles={selectedFiles}
              uploadedUrls={uploadedMediaUrls}
              isUploading={isUploading}
              error={uploadError}
              maxFiles={10}
              acceptedFileTypes="image/*,video/*"
            />
          </Grid>

          <Grid item xs={12}>
            <Button
              variant="outlined"
              color={schedulePost ? "primary" : "inherit"}
              onClick={handleScheduleToggle}
              sx={{ mb: 2, mt: 2 }}
              disabled={isLoading}
            >
              {schedulePost ? "Agendamento Ativado" : "Agendar Publicação"}
            </Button>

            {schedulePost && (
              <DateTimePicker
                label="Data e Hora de Publicação"
                value={selectedDate}
                onChange={handleDateChange}
                disabled={isLoading}
                slotProps={{
                  textField: {
                    fullWidth: true,
                    error: !!errors.scheduledDate,
                    helperText: errors.scheduledDate,
                  }
                }}
              />
            )}
          </Grid>

          <Grid item xs={12}>
            <Divider sx={{ mb: 2 }}>Tags</Divider>
            <Stack direction="row" spacing={1} sx={{ mb: 2 }}>
              <TextField
                label="Adicionar Tag"
                value={tagInput}
                onChange={(e) => setTagInput(e.target.value)}
                disabled={isLoading}
                size="small"
              />
              <Button
                variant="outlined"
                onClick={handleAddTag}
                disabled={isLoading || !tagInput.trim()}
              >
                Adicionar
              </Button>
            </Stack>

            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
              {formData.tags.map((tag: string) => (
                <Chip
                  key={tag}
                  label={tag}
                  onDelete={() => handleRemoveTag(tag)}
                  disabled={isLoading}
                />
              ))}
            </Box>
          </Grid>
        </Grid>

        <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 2 }}>
          <Button
            variant="outlined"
            onClick={onCancel}
            disabled={isLoading}
          >
            Cancelar
          </Button>
          <Button
            type="submit"
            variant="contained"
            disabled={isLoading}
            startIcon={isLoading ? <CircularProgress size={20} /> : null}
          >
            {mode === 'create' ? 'Criar' : 'Salvar'}
          </Button>
        </Box>
      </Box>
    </Paper>
  );
};

export default ContentForm;