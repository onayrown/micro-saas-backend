import React, { useState } from 'react';
import { Box, Button, Container, Typography, Paper, CircularProgress, Alert } from '@mui/material';
import MediaUpload from '../components/content/MediaUpload';
import MediaService from '../services/MediaService';
import { useAuth } from '../hooks/useAuth';

const MediaUploadTest: React.FC = () => {
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const [uploadedUrls, setUploadedUrls] = useState<string[]>([]);
  const [isUploading, setIsUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const { user } = useAuth();

  const handleMediaSelected = (files: File[]) => {
    setSelectedFiles([...selectedFiles, ...files]);
    setError(null);
  };

  const handleMediaRemoved = (index: number) => {
    const newFiles = [...selectedFiles];
    newFiles.splice(index, 1);
    setSelectedFiles(newFiles);
  };

  const handleUpload = async () => {
    if (!user?.id) {
      setError('Usuário não autenticado. Faça login para continuar.');
      return;
    }

    if (selectedFiles.length === 0) {
      setError('Selecione pelo menos um arquivo para enviar.');
      return;
    }

    setIsUploading(true);
    setError(null);
    setSuccess(null);

    try {
      const uploadedMedia = await MediaService.uploadMedia(selectedFiles, user.id);

      setUploadedUrls(uploadedMedia.map(media => media.url));
      setSelectedFiles([]);
      setSuccess(`${uploadedMedia.length} arquivo(s) enviado(s) com sucesso!`);
    } catch (err: any) {
      console.error('Erro ao fazer upload:', err);
      setError(err.message || 'Erro ao fazer upload dos arquivos.');
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Paper elevation={3} sx={{ p: 3 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Teste de Upload de Mídia
        </Typography>

        <Typography variant="body1" paragraph>
          Esta página permite testar o upload de arquivos de mídia para o servidor.
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {success && (
          <Alert severity="success" sx={{ mb: 2 }}>
            {success}
          </Alert>
        )}

        <MediaUpload
          onMediaSelected={handleMediaSelected}
          onMediaRemoved={handleMediaRemoved}
          selectedFiles={selectedFiles}
          uploadedUrls={uploadedUrls}
          isUploading={isUploading}
          error={error}
          maxFiles={5}
          acceptedFileTypes="image/*,video/*"
        />

        <Box sx={{ mt: 3, display: 'flex', justifyContent: 'center' }}>
          <Button
            variant="contained"
            color="primary"
            onClick={handleUpload}
            disabled={isUploading || selectedFiles.length === 0}
            startIcon={isUploading ? <CircularProgress size={20} color="inherit" /> : null}
          >
            {isUploading ? 'Enviando...' : 'Enviar Arquivos'}
          </Button>
        </Box>

        {uploadedUrls.length > 0 && (
          <Box sx={{ mt: 4 }}>
            <Typography variant="h6" gutterBottom>
              Arquivos Enviados:
            </Typography>

            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
              {uploadedUrls.map((url, index) => (
                <Box key={index} sx={{ width: 150, textAlign: 'center' }}>
                  {url.match(/\.(jpeg|jpg|gif|png)$/i) ? (
                    <img
                      src={url}
                      alt={`Uploaded ${index}`}
                      style={{ width: '100%', height: 'auto', borderRadius: 4 }}
                    />
                  ) : (
                    <Box
                      sx={{
                        width: '100%',
                        height: 100,
                        bgcolor: 'grey.200',
                        borderRadius: 1,
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center'
                      }}
                    >
                      <Typography variant="body2">
                        Arquivo não visualizável
                      </Typography>
                    </Box>
                  )}
                  <Typography variant="caption" display="block" sx={{ mt: 1 }}>
                    <a href={url} target="_blank" rel="noopener noreferrer">
                      Ver arquivo
                    </a>
                  </Typography>
                </Box>
              ))}
            </Box>
          </Box>
        )}
      </Paper>
    </Container>
  );
};

export default MediaUploadTest;
