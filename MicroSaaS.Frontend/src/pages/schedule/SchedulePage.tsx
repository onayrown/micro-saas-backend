import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Box,
  Paper,
  Grid,
  Card,
  CardContent,
  CardHeader,
  Divider,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  Avatar,
  Chip,
  CircularProgress,
  Button,
  IconButton
} from '@mui/material';
import {
  Instagram as InstagramIcon,
  YouTube as YouTubeIcon,
  Facebook as FacebookIcon,
  Twitter as TwitterIcon,
  LinkedIn as LinkedInIcon,
  EventNote as EventNoteIcon,
  Delete as DeleteIcon,
  Edit as EditIcon,
  Add as AddIcon
} from '@mui/icons-material';
import { ContentPost, PostStatus, SocialMediaPlatform } from '../../types';
import { formatDate, formatDateTime } from '../../utils/dateUtils';

interface ScheduleDay {
  date: string;
  formattedDate: string;
  isToday: boolean;
  posts: ContentPost[];
}

const SchedulePage = () => {
  const [loading, setLoading] = useState(true);
  const [scheduleDays, setScheduleDays] = useState<ScheduleDay[]>([]);
  const [error, setError] = useState<string | null>(null);

  // Simulando carregamento de dados
  useEffect(() => {
    const timer = setTimeout(() => {
      // Dados de exemplo
      const today = new Date();
      const nextWeek = new Array(7).fill(null).map((_, index) => {
        const date = new Date();
        date.setDate(today.getDate() + index);
        return date;
      });

      const samplePosts: ContentPost[] = [
        {
          id: '1',
          title: '5 dicas para melhorar seu SEO',
          content: 'Conteúdo sobre dicas de SEO...',
          platform: SocialMediaPlatform.Instagram,
          scheduledDate: new Date(today.setHours(14, 30, 0, 0)).toISOString(),
          status: PostStatus.Scheduled,
          creatorId: 'user1',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        },
        {
          id: '2',
          title: 'Como otimizar seu workflow',
          content: 'Dicas para melhorar a produtividade...',
          platform: SocialMediaPlatform.LinkedIn,
          scheduledDate: new Date(today.setHours(16, 0, 0, 0)).toISOString(),
          status: PostStatus.Scheduled,
          creatorId: 'user1',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        },
        {
          id: '3',
          title: 'Review: Novos frameworks JS',
          content: 'Análise dos frameworks mais recentes...',
          platform: SocialMediaPlatform.YouTube,
          scheduledDate: new Date(nextWeek[2].setHours(10, 0, 0, 0)).toISOString(),
          status: PostStatus.Scheduled,
          creatorId: 'user1',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        },
        {
          id: '4',
          title: 'Thread: Estratégias de marketing',
          content: 'Dicas de marketing para crescer seu negócio...',
          platform: SocialMediaPlatform.Twitter,
          scheduledDate: new Date(nextWeek[4].setHours(18, 30, 0, 0)).toISOString(),
          status: PostStatus.Scheduled,
          creatorId: 'user1',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        }
      ];

      // Organizar posts por dia
      const days: ScheduleDay[] = nextWeek.map((date) => {
        const dateString = date.toISOString().split('T')[0];
        const isToday = date.getDate() === today.getDate() && 
                        date.getMonth() === today.getMonth() && 
                        date.getFullYear() === today.getFullYear();
        
        return {
          date: dateString,
          formattedDate: formatDate(date),
          isToday,
          posts: samplePosts.filter(post => 
            post.scheduledDate && new Date(post.scheduledDate).toISOString().split('T')[0] === dateString
          )
        };
      });

      setScheduleDays(days);
      setLoading(false);
    }, 1500);

    return () => clearTimeout(timer);
  }, []);

  const getPlatformIcon = (platform: SocialMediaPlatform) => {
    switch(platform) {
      case SocialMediaPlatform.Instagram:
        return <InstagramIcon style={{ color: '#E1306C' }} />;
      case SocialMediaPlatform.YouTube:
        return <YouTubeIcon style={{ color: '#FF0000' }} />;
      case SocialMediaPlatform.Facebook:
        return <FacebookIcon style={{ color: '#4267B2' }} />;
      case SocialMediaPlatform.Twitter:
        return <TwitterIcon style={{ color: '#1DA1F2' }} />;
      case SocialMediaPlatform.LinkedIn:
        return <LinkedInIcon style={{ color: '#0077B5' }} />;
      default:
        return <Avatar>{platform.charAt(0)}</Avatar>;
    }
  };

  const handleEditPost = (postId: string) => {
    // Implementar lógica de edição
    console.log('Editar post:', postId);
  };

  const handleDeletePost = (postId: string) => {
    // Implementar lógica de exclusão
    console.log('Excluir post:', postId);
  };

  const handleCreatePost = () => {
    // Implementar lógica de criação
    console.log('Criar novo post');
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '80vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Container>
        <Typography variant="h6" color="error" sx={{ mt: 4 }}>
          Erro ao carregar dados: {error}
        </Typography>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" gutterBottom>
          Agenda de Publicações
        </Typography>
        <Button 
          variant="contained" 
          startIcon={<AddIcon />}
          onClick={handleCreatePost}
        >
          Agendar Publicação
        </Button>
      </Box>

      <Grid container spacing={3}>
        {scheduleDays.map((day) => (
          <Grid item xs={12} key={day.date}>
            <Paper variant="outlined" sx={{ p: 0 }}>
              <Box sx={{ 
                p: 2, 
                backgroundColor: day.isToday ? 'primary.light' : 'background.paper',
                color: day.isToday ? 'primary.contrastText' : 'text.primary',
                borderTopLeftRadius: 4,
                borderTopRightRadius: 4
              }}>
                <Typography variant="h6" component="div">
                  {day.isToday ? 'Hoje' : day.formattedDate}
                  {day.isToday && <Typography variant="subtitle2" component="span"> - {day.formattedDate}</Typography>}
                </Typography>
              </Box>
              <Divider />
              <List sx={{ p: 0 }}>
                {day.posts.length === 0 ? (
                  <ListItem sx={{ pl: 3 }}>
                    <ListItemText 
                      primary="Nenhuma publicação agendada" 
                      primaryTypographyProps={{ color: 'text.secondary' }}
                    />
                  </ListItem>
                ) : (
                  day.posts.map((post) => (
                    <ListItem key={post.id} alignItems="flex-start" sx={{ pl: 3, pr: 2 }}>
                      <ListItemAvatar>
                        <Avatar>
                          {getPlatformIcon(post.platform)}
                        </Avatar>
                      </ListItemAvatar>
                      <ListItemText
                        primary={
                          <Box sx={{ display: 'flex', alignItems: 'center' }}>
                            <Typography variant="subtitle1" component="span">
                              {post.title}
                            </Typography>
                            {post.scheduledDate && (
                              <Chip 
                                label={formatDateTime(post.scheduledDate).split(' ')[1]} 
                                size="small"
                                color="primary"
                                variant="outlined"
                                icon={<EventNoteIcon />}
                                sx={{ ml: 1 }}
                              />
                            )}
                          </Box>
                        }
                        secondary={
                          <React.Fragment>
                            <Typography component="span" variant="body2">
                              {post.content.length > 100 ? `${post.content.substring(0, 100)}...` : post.content}
                            </Typography>
                            <Chip 
                              label={post.platform} 
                              size="small"
                              sx={{ ml: 1 }}
                            />
                          </React.Fragment>
                        }
                      />
                      <Box>
                        <IconButton size="small" onClick={() => handleEditPost(post.id)}>
                          <EditIcon />
                        </IconButton>
                        <IconButton size="small" onClick={() => handleDeletePost(post.id)}>
                          <DeleteIcon />
                        </IconButton>
                      </Box>
                    </ListItem>
                  ))
                )}
              </List>
            </Paper>
          </Grid>
        ))}
      </Grid>
    </Container>
  );
};

export default SchedulePage; 