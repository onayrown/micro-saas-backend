import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Box,
  Tabs,
  Tab,
  Button,
  Grid,
  Card,
  CardContent,
  CardActions,
  Chip,
  Divider,
  IconButton,
  CircularProgress,
  Alert
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Instagram as InstagramIcon,
  YouTube as YouTubeIcon,
  Facebook as FacebookIcon,
  Twitter as TwitterIcon,
  LinkedIn as LinkedInIcon,
  Instagram,
  YouTube,
  Facebook,
  Twitter,
  LinkedIn,
  MusicNote,
  Pinterest,
  Public
} from '@mui/icons-material';
import ContentService, { ContentPost } from '../../services/ContentService';
import { PostStatus, SocialMediaPlatform } from '../../types/common';
import { formatDate } from '../../utils/dateUtils';
import ContentFormDialog from '../../components/content/ContentFormDialog';
import { useAuth } from '../../hooks/useAuth';
import { ContentFormMode } from '../../components/content/ContentForm';

const ContentPage = () => {
  const { user } = useAuth();
  const [tabValue, setTabValue] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [posts, setPosts] = useState<ContentPost[]>([]);
  const [selectedPost, setSelectedPost] = useState<ContentPost | undefined>(undefined);
  const [formMode, setFormMode] = useState<ContentFormMode>('create');
  const [formOpen, setFormOpen] = useState(false);
  
  const fetchPosts = async () => {
    if (!user?.id) return;
    
    setLoading(true);
    setError(null);
    
    try {
      let status;
      switch (tabValue) {
        case 0:
          status = 'draft';
          break;
        case 1:
          status = 'scheduled';
          break;
        case 2:
          status = 'published';
          break;
        default:
          status = undefined;
      }
      
      const data = await ContentService.getPosts(user.id, status);
      setPosts(data);
    } catch (error: any) {
      console.error('Erro ao carregar posts:', error);
      setError(error.message || 'Ocorreu um erro ao carregar os posts');
    } finally {
      setLoading(false);
    }
  };
  
  useEffect(() => {
    fetchPosts();
  }, [tabValue, user]);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const handleCreatePost = () => {
    setFormMode('create');
    setSelectedPost(undefined);
    setFormOpen(true);
  };

  const handleEditPost = (post: ContentPost) => {
    setFormMode('edit');
    setSelectedPost(post);
    setFormOpen(true);
  };

  const handleDeletePost = async (postId: string) => {
    if (!window.confirm('Tem certeza que deseja excluir este post?')) {
      return;
    }
    
    setLoading(true);
    setError(null);
    
    try {
      await ContentService.deletePost(postId);
      fetchPosts(); // Recarregar a lista após excluir
    } catch (error: any) {
      console.error('Erro ao excluir post:', error);
      setError(error.message || 'Ocorreu um erro ao excluir o post');
      setLoading(false);
    }
  };

  const handleFormSuccess = () => {
    fetchPosts();
  };

  const getPlatformIcon = (platform: string) => {
    switch (platform) {
      case 'Instagram':
        return <Instagram />;
      case 'YouTube':
        return <YouTube />;
      case 'Facebook':
        return <Facebook />;
      case 'Twitter':
        return <Twitter />;
      case 'LinkedIn':
        return <LinkedIn />;
      case 'TikTok':
        return <MusicNote />;
      case 'Pinterest':
        return <Pinterest />;
      default:
        return <Public />;
    }
  };

  const getStatusColor = (status: string) => {
    switch(status) {
      case 'draft':
        return 'default';
      case 'scheduled':
        return 'primary';
      case 'published':
        return 'success';
      case 'failed':
        return 'error';
      case 'processing':
        return 'warning';
      default:
        return 'default';
    }
  };

  const renderPosts = () => {
    if (loading) {
      return (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 5 }}>
          <CircularProgress />
        </Box>
      );
    }
    
    if (error) {
      return (
        <Alert severity="error" sx={{ mt: 2, mb: 2 }}>
          {error}
        </Alert>
      );
    }
    
    if (posts.length === 0) {
      return (
        <Box sx={{ p: 5, textAlign: 'center' }}>
          <Typography variant="subtitle1" color="text.secondary">
            Nenhum conteúdo encontrado nesta categoria
          </Typography>
        </Box>
      );
    }
    
    return (
      <Grid container spacing={3}>
        {posts.map((post) => (
          <Grid item xs={12} sm={6} md={4} key={post.id}>
            <Card variant="outlined">
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                  {post.platforms && post.platforms.length > 0 && 
                    getPlatformIcon(post.platforms[0])}
                  <Chip 
                    label={post.status} 
                    size="small" 
                    color={getStatusColor(post.status)}
                    sx={{ ml: 1 }}
                  />
                </Box>
                <Typography variant="h6" component="div" gutterBottom noWrap>
                  {post.title}
                </Typography>
                <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                  {post.description?.substring(0, 100) || 'Sem descrição'}
                  {post.description && post.description.length > 100 ? '...' : ''}
                </Typography>
                {post.scheduledDate && (
                  <Typography variant="caption" display="block">
                    {post.status === 'published' ? 'Publicado em: ' : 'Agendado para: '}
                    {formatDate(post.scheduledDate)}
                  </Typography>
                )}
              </CardContent>
              <Divider />
              <CardActions sx={{ justifyContent: 'flex-end' }}>
                <IconButton size="small" onClick={() => handleEditPost(post)}>
                  <EditIcon />
                </IconButton>
                <IconButton size="small" onClick={() => handleDeletePost(post.id)}>
                  <DeleteIcon />
                </IconButton>
              </CardActions>
            </Card>
          </Grid>
        ))}
      </Grid>
    );
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Gerenciamento de Conteúdo</Typography>
        <Button 
          variant="contained" 
          startIcon={<AddIcon />}
          onClick={handleCreatePost}
          disabled={loading}
        >
          Criar Conteúdo
        </Button>
      </Box>
      
      <Box sx={{ width: '100%', mb: 4 }}>
        <Tabs 
          value={tabValue} 
          onChange={handleTabChange}
          aria-label="content tabs"
          sx={{ mb: 3 }}
        >
          <Tab label="Rascunhos" disabled={loading} />
          <Tab label="Agendados" disabled={loading} />
          <Tab label="Publicados" disabled={loading} />
        </Tabs>
        
        {renderPosts()}
      </Box>
      
      <ContentFormDialog
        open={formOpen}
        onClose={() => setFormOpen(false)}
        onSuccess={handleFormSuccess}
        mode={formMode}
        post={selectedPost}
      />
    </Container>
  );
};

export default ContentPage; 