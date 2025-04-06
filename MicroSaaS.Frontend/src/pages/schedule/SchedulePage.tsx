import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Box,
  Paper,
  Grid,
  Card,
  CardContent,
  Divider,
  CircularProgress,
  Button,
  IconButton,
  Chip
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
  Add as AddIcon,
  ChevronLeft as ChevronLeftIcon,
  ChevronRight as ChevronRightIcon,
  Event as EventIcon
} from '@mui/icons-material';
import ContentService, { ContentPost } from '../../services/ContentService';
import { formatDate, formatDateOnly, isToday } from '../../utils/dateUtils';
import ContentFormDialog from '../../components/content/ContentFormDialog';
import { ContentFormMode } from '../../components/content/ContentForm';
import { useAuth } from '../../hooks/useAuth';

// Lista de cores usada para identificação visual de diferentes plataformas
const PLATFORM_COLORS = ['#1DA1F2', '#E1306C', '#4267B2', '#0077B5', '#FF0000'];

interface CalendarDay {
  date: string;
  formattedDate: string;
  isToday: boolean;
  posts: ContentPost[];
}

const SchedulePage = () => {
  const { user } = useAuth();
  const [currentDate, setCurrentDate] = useState(new Date());
  const [calendarDays, setCalendarDays] = useState<CalendarDay[]>([]);
  const [loading, setLoading] = useState(false);
  const [selectedPost, setSelectedPost] = useState<ContentPost | undefined>(undefined);
  const [formOpen, setFormOpen] = useState(false);
  const [formMode, setFormMode] = useState<ContentFormMode>('create');

  // Posts de amostra para demonstração - serão substituídos por dados da API
  const samplePosts: ContentPost[] = [
    {
      id: '1',
      title: 'Dicas para melhorar seu SEO',
      description: 'Aprenda como melhorar o ranqueamento do seu site.',
      contentType: 'text',
      platforms: ['Instagram'],
      scheduledDate: new Date(new Date().setHours(12, 0, 0, 0)).toISOString(),
      status: 'scheduled',
      userId: user?.id || '1',
    },
    {
      id: '2', 
      title: 'Como aumentar engajamento',
      description: 'Estratégias para aumentar o engajamento nas redes sociais.',
      contentType: 'text',
      platforms: ['Twitter'],
      scheduledDate: new Date(new Date().setHours(15, 30, 0, 0)).toISOString(),
      status: 'scheduled',
      userId: user?.id || '1',
    },
    {
      id: '3',
      title: 'Anúncio de novo produto',
      description: 'Lançamento oficial do nosso novo produto.',
      contentType: 'text',
      platforms: ['Facebook', 'Instagram'],
      scheduledDate: new Date(new Date().setDate(new Date().getDate() + 1)).toISOString(),
      status: 'scheduled',
      userId: user?.id || '1',
    },
    {
      id: '4',
      title: 'Webinar sobre marketing digital',
      description: 'Participe do nosso webinar exclusivo sobre estratégias de marketing.',
      contentType: 'text',
      platforms: ['LinkedIn'],
      scheduledDate: new Date(new Date().setDate(new Date().getDate() + 3)).toISOString(),
      status: 'scheduled',
      userId: user?.id || '1',
    },
  ];

  // Gera os dias do calendário para o período atual
  useEffect(() => {
    generateCalendarDays();
  }, [currentDate]);

  // Função para gerar os dias do calendário
  const generateCalendarDays = () => {
    setLoading(true);
    
    try {
      const startOfWeek = new Date(currentDate);
      const day = currentDate.getDay();
      const diff = currentDate.getDate() - day + (day === 0 ? -6 : 1); // Ajuste para considerar o domingo como último dia
      startOfWeek.setDate(diff);
      
      const days: CalendarDay[] = [];
      
      for (let i = 0; i < 7; i++) {
        const date = new Date(startOfWeek);
        date.setDate(date.getDate() + i);
        
        const dateString = date.toISOString().split('T')[0];
        const isCurrentDay = isToday(date);
        
        days.push({
          date: dateString,
          formattedDate: formatDateOnly(dateString),
          isToday: isCurrentDay,
          posts: samplePosts.filter(post => 
            post.scheduledDate && new Date(post.scheduledDate).toISOString().split('T')[0] === dateString
          )
        });
      }
      
      setCalendarDays(days);
    } catch (error) {
      console.error('Erro ao gerar dias do calendário:', error);
    } finally {
      setLoading(false);
    }
  };

  // Abre o diálogo para editar um post
  const handleEditPost = (post: ContentPost) => {
    setSelectedPost(post);
    setFormMode('edit');
    setFormOpen(true);
  };

  // Abre o diálogo para criar um novo post
  const handleCreatePost = () => {
    setSelectedPost(undefined);
    setFormMode('create');
    setFormOpen(true);
  };

  // Callback quando a edição ou criação é concluída
  const handleFormSuccess = () => {
    // Recarrega os dados do calendário
    generateCalendarDays();
  };

  // Navega para a semana anterior
  const goToPreviousWeek = () => {
    const newDate = new Date(currentDate);
    newDate.setDate(currentDate.getDate() - 7);
    setCurrentDate(newDate);
  };

  // Navega para a próxima semana
  const goToNextWeek = () => {
    const newDate = new Date(currentDate);
    newDate.setDate(currentDate.getDate() + 7);
    setCurrentDate(newDate);
  };

  // Retorna para a semana atual
  const goToCurrentWeek = () => {
    setCurrentDate(new Date());
  };

  // Renderiza os posts para um dia específico
  const renderPostsForDay = (posts: ContentPost[]) => {
    if (posts.length === 0) {
      return (
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1, fontStyle: 'italic' }}>
          Nenhum post agendado
        </Typography>
      );
    }

    return posts.map((post) => (
      <Card 
        key={post.id} 
        variant="outlined" 
        sx={{ 
          mt: 1, 
          cursor: 'pointer',
          '&:hover': { boxShadow: 1 }
        }}
        onClick={() => handleEditPost(post)}
      >
        <CardContent sx={{ p: 1, '&:last-child': { pb: 1 } }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 0.5 }}>
            <Typography variant="subtitle2" sx={{ fontWeight: 'bold' }}>
              {formatDate(post.scheduledDate || '').split(' ')[1]}
            </Typography>
            <Box>
              {post.platforms?.slice(0, 2).map((platform: string, i: number) => (
                <Chip 
                  key={i}
                  label={platform}
                  size="small"
                  sx={{ 
                    height: 20, 
                    mr: i < post.platforms.length - 1 ? 0.5 : 0,
                    bgcolor: PLATFORM_COLORS[i % PLATFORM_COLORS.length],
                    color: 'white',
                    fontSize: '0.7rem'
                  }}
                />
              ))}
              {post.platforms && post.platforms.length > 2 && (
                <Chip 
                  label={`+${post.platforms.length - 2}`}
                  size="small"
                  sx={{ height: 20, fontSize: '0.7rem' }}
                />
              )}
            </Box>
          </Box>
          <Typography variant="body2" noWrap>
            {post.title}
          </Typography>
        </CardContent>
      </Card>
    ));
  };

  // Renderiza o cabeçalho do calendário
  const renderCalendarHeader = () => {
    const startDate = calendarDays.length > 0 ? calendarDays[0].date : '';
    const endDate = calendarDays.length > 0 ? calendarDays[calendarDays.length - 1].date : '';
    
    const formattedStartDate = startDate ? formatDateOnly(startDate) : '';
    const formattedEndDate = endDate ? formatDateOnly(endDate) : '';
    
    return (
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
        <Typography variant="h5" sx={{ flexGrow: 1 }}>
          {formattedStartDate} - {formattedEndDate}
        </Typography>
        <Box>
          <IconButton onClick={goToPreviousWeek} disabled={loading}>
            <ChevronLeftIcon />
          </IconButton>
          <Button 
            variant="outlined" 
            startIcon={<EventIcon />} 
            onClick={goToCurrentWeek}
            disabled={loading}
            sx={{ mx: 1 }}
          >
            Hoje
          </Button>
          <IconButton onClick={goToNextWeek} disabled={loading}>
            <ChevronRightIcon />
          </IconButton>
        </Box>
      </Box>
    );
  };

  // Mostrar loading quando estiver carregando inicialmente
  if (loading && calendarDays.length === 0) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '80vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Agendamento de Conteúdo</Typography>
        <Button 
          variant="contained" 
          startIcon={<AddIcon />} 
          onClick={handleCreatePost}
          disabled={loading}
        >
          Agendar Publicação
        </Button>
      </Box>
      
      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', my: 5 }}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          {renderCalendarHeader()}
          
          <Grid container spacing={2}>
            {calendarDays.map((day) => (
              <Grid item xs={12} sm={6} md={12/7} key={day.date}>
                <Paper 
                  elevation={0} 
                  sx={{ 
                    p: 2, 
                    height: '100%',
                    bgcolor: day.isToday ? 'rgba(25, 118, 210, 0.1)' : 'background.paper',
                    border: day.isToday ? '1px solid #1976d2' : '1px solid #e0e0e0',
                    borderRadius: 1
                  }}
                >
                  <Typography 
                    variant="subtitle1" 
                    sx={{ 
                      fontWeight: day.isToday ? 'bold' : 'medium',
                      color: day.isToday ? 'primary.main' : 'text.primary',
                      mb: 1
                    }}
                  >
                    {day.formattedDate}
                  </Typography>
                  <Divider />
                  <Box sx={{ mt: 1, maxHeight: 350, overflow: 'auto' }}>
                    {renderPostsForDay(day.posts)}
                  </Box>
                </Paper>
              </Grid>
            ))}
          </Grid>
        </>
      )}
      
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

export default SchedulePage; 