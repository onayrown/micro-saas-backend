import React, { useState } from 'react';
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
  IconButton
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Instagram as InstagramIcon,
  YouTube as YouTubeIcon,
  Facebook as FacebookIcon,
  Twitter as TwitterIcon,
  LinkedIn as LinkedInIcon
} from '@mui/icons-material';
import { PostStatus, SocialMediaPlatform } from '../../types';
import { formatDate } from '../../utils/dateUtils';

interface Post {
  id: string;
  title: string;
  content: string;
  platform: SocialMediaPlatform;
  scheduledDate: string;
  status: PostStatus;
  createdAt: string;
}

const ContentPage = () => {
  const [tabValue, setTabValue] = useState(0);
  
  // Dados de exemplo para posts
  const draftPosts: Post[] = [
    {
      id: '1',
      title: 'Dicas para iniciantes em desenvolvimento web',
      content: 'Conteúdo sobre dicas para desenvolvimento web...',
      platform: SocialMediaPlatform.Instagram,
      scheduledDate: '',
      status: PostStatus.Draft,
      createdAt: new Date().toISOString()
    },
    {
      id: '2',
      title: 'Como monetizar seu canal no YouTube',
      content: 'Guia completo sobre monetização no YouTube...',
      platform: SocialMediaPlatform.YouTube,
      scheduledDate: '',
      status: PostStatus.Draft,
      createdAt: new Date().toISOString()
    }
  ];
  
  const scheduledPosts: Post[] = [
    {
      id: '3',
      title: 'Tendências de marketing digital para 2023',
      content: 'Análise das tendências de marketing para o próximo ano...',
      platform: SocialMediaPlatform.LinkedIn,
      scheduledDate: new Date(new Date().setDate(new Date().getDate() + 2)).toISOString(),
      status: PostStatus.Scheduled,
      createdAt: new Date().toISOString()
    }
  ];
  
  const publishedPosts: Post[] = [
    {
      id: '4',
      title: '10 ferramentas essenciais para desenvolvedores',
      content: 'Lista de ferramentas úteis para desenvolvedores...',
      platform: SocialMediaPlatform.Twitter,
      scheduledDate: new Date(new Date().setDate(new Date().getDate() - 3)).toISOString(),
      status: PostStatus.Published,
      createdAt: new Date().toISOString()
    },
    {
      id: '5',
      title: 'Como aumentar sua produtividade',
      content: 'Dicas para melhorar a produtividade no trabalho...',
      platform: SocialMediaPlatform.Instagram,
      scheduledDate: new Date(new Date().setDate(new Date().getDate() - 7)).toISOString(),
      status: PostStatus.Published,
      createdAt: new Date().toISOString()
    }
  ];

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const handleCreatePost = () => {
    // Implementar lógica para criação de post
    console.log('Criar novo post');
  };

  const handleEditPost = (id: string) => {
    // Implementar lógica para edição de post
    console.log('Editar post', id);
  };

  const handleDeletePost = (id: string) => {
    // Implementar lógica para exclusão de post
    console.log('Excluir post', id);
  };

  const getPlatformIcon = (platform: SocialMediaPlatform) => {
    switch(platform) {
      case SocialMediaPlatform.Instagram:
        return <InstagramIcon sx={{ color: '#E1306C' }} />;
      case SocialMediaPlatform.YouTube:
        return <YouTubeIcon sx={{ color: '#FF0000' }} />;
      case SocialMediaPlatform.Facebook:
        return <FacebookIcon sx={{ color: '#4267B2' }} />;
      case SocialMediaPlatform.Twitter:
        return <TwitterIcon sx={{ color: '#1DA1F2' }} />;
      case SocialMediaPlatform.LinkedIn:
        return <LinkedInIcon sx={{ color: '#0077B5' }} />;
      default:
        return null;
    }
  };

  const getStatusColor = (status: PostStatus) => {
    switch(status) {
      case PostStatus.Draft:
        return 'default';
      case PostStatus.Scheduled:
        return 'primary';
      case PostStatus.Published:
        return 'success';
      case PostStatus.Failed:
        return 'error';
      case PostStatus.Processing:
        return 'warning';
      default:
        return 'default';
    }
  };

  const renderPosts = (posts: Post[]) => {
    return (
      <Grid container spacing={3}>
        {posts.length === 0 ? (
          <Grid item xs={12}>
            <Typography variant="subtitle1" color="text.secondary" align="center" sx={{ py: 5 }}>
              Nenhum conteúdo encontrado nesta categoria
            </Typography>
          </Grid>
        ) : (
          posts.map((post) => (
            <Grid item xs={12} sm={6} md={4} key={post.id}>
              <Card variant="outlined">
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                    {getPlatformIcon(post.platform)}
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
                    {post.content.substring(0, 100)}...
                  </Typography>
                  {post.scheduledDate && (
                    <Typography variant="caption" display="block">
                      {post.status === PostStatus.Published ? 'Publicado em: ' : 'Agendado para: '}
                      {formatDate(post.scheduledDate)}
                    </Typography>
                  )}
                </CardContent>
                <Divider />
                <CardActions>
                  <IconButton size="small" onClick={() => handleEditPost(post.id)}>
                    <EditIcon />
                  </IconButton>
                  <IconButton size="small" onClick={() => handleDeletePost(post.id)}>
                    <DeleteIcon />
                  </IconButton>
                </CardActions>
              </Card>
            </Grid>
          ))
        )}
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
          <Tab label="Rascunhos" />
          <Tab label="Agendados" />
          <Tab label="Publicados" />
        </Tabs>
        
        {tabValue === 0 && renderPosts(draftPosts)}
        {tabValue === 1 && renderPosts(scheduledPosts)}
        {tabValue === 2 && renderPosts(publishedPosts)}
      </Box>
    </Container>
  );
};

export default ContentPage; 