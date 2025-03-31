import React, { useState, useEffect } from 'react';
import {
  Container,
  Grid,
  Paper,
  Typography,
  Box,
  Card,
  CardContent,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Divider,
  LinearProgress,
  CircularProgress,
  Alert,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  SelectChangeEvent,
  Chip
} from '@mui/material';
import {
  TrendingUp as TrendingUpIcon,
  Lightbulb as LightbulbIcon,
  Timeline as TimelineIcon,
  Assessment as AssessmentIcon,
  Group as GroupIcon,
  Schedule as ScheduleIcon,
  Refresh as RefreshIcon,
  Info as InfoIcon
} from '@mui/icons-material';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import RecommendationService from '../../services/RecommendationService';
import { SocialMediaPlatform } from '../../types/common';
import { useUser } from '../../contexts/UserContext';

// Dados temporários para demonstração (serão substituídos pelos dados da API)
const engagementData = [
  { name: 'Jan', engagement: 65, followers: 1000 },
  { name: 'Fev', engagement: 68, followers: 1200 },
  { name: 'Mar', engagement: 72, followers: 1500 },
  { name: 'Abr', engagement: 75, followers: 1800 },
  { name: 'Mai', engagement: 78, followers: 2200 },
  { name: 'Jun', engagement: 82, followers: 2600 },
];

const growthTips = [
  'Crie conteúdo educativo e informativo',
  'Use hashtags relevantes e populares',
  'Mantenha consistência nas postagens',
  'Interaja com sua comunidade',
  'Analise métricas regularmente',
  'Experimente diferentes formatos',
];

const RecommendationsPage = () => {
  const { creator, loading: userLoading } = useUser();
  
  // Estados para armazenar dados da API
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [refreshing, setRefreshing] = useState<boolean>(false);
  const [selectedPlatform, setSelectedPlatform] = useState<SocialMediaPlatform>(SocialMediaPlatform.Instagram);
  const [usingSampleData, setUsingSampleData] = useState<boolean>(false);
  
  const [contentRecommendations, setContentRecommendations] = useState<any[]>([]);
  const [bestTimes, setBestTimes] = useState<any[]>([]);
  const [trendingTopics, setTrendingTopics] = useState<any[]>([]);
  const [growthRecommendations, setGrowthRecommendations] = useState<any[]>([]);
  
  // Carregar dados iniciais quando o criador estiver disponível
  useEffect(() => {
    if (creator && !userLoading) {
      loadRecommendationData();
    }
  }, [creator, userLoading]);
  
  // Carregar dados quando a plataforma selecionada mudar
  useEffect(() => {
    if (creator && !userLoading) {
      loadBestTimes();
      loadTrendingTopics();
    }
  }, [selectedPlatform, creator, userLoading]);
  
  // Função para carregar todos os dados de recomendação
  const loadRecommendationData = async () => {
    if (!creator) return;
    
    setLoading(true);
    setError(null);
    setUsingSampleData(false);
    
    try {
      await Promise.all([
        loadContentRecommendations(),
        loadBestTimes(),
        loadTrendingTopics(),
        loadGrowthRecommendations()
      ]);
    } catch (err) {
      // Se estivermos usando dados simulados, não mostramos erro ao usuário
      if (!usingSampleData) {
        setError('Erro ao carregar recomendações. Tente novamente mais tarde.');
      }
      console.error('Erro ao carregar recomendações:', err);
    } finally {
      setLoading(false);
    }
  };
  
  // Função para recarregar todas as recomendações
  const handleRefresh = async () => {
    if (!creator) return;
    
    setRefreshing(true);
    setUsingSampleData(false);
    try {
      await RecommendationService.refreshRecommendations(creator.id);
      await loadRecommendationData();
    } catch (err) {
      if (!usingSampleData) {
        setError('Erro ao atualizar recomendações. Tente novamente mais tarde.');
      }
      console.error('Erro ao atualizar recomendações:', err);
    } finally {
      setRefreshing(false);
    }
  };
  
  // Função para lidar com a mudança de plataforma
  const handlePlatformChange = (event: SelectChangeEvent<string>) => {
    setSelectedPlatform(event.target.value as SocialMediaPlatform);
  };
  
  // Funções para carregar dados específicos
  const loadContentRecommendations = async () => {
    if (!creator) return;
    try {
      const data = await RecommendationService.getContentRecommendations(creator.id);
      setContentRecommendations(data);
      // Se recebemos dados simulados
      if (data && data.length > 0 && data[0].id && data[0].id.startsWith('1')) {
        setUsingSampleData(true);
      }
    } catch (err) {
      console.error('Erro ao carregar recomendações de conteúdo:', err);
      throw err;
    }
  };
  
  const loadBestTimes = async () => {
    if (!creator) return;
    try {
      const data = await RecommendationService.getBestTimeToPost(creator.id, selectedPlatform);
      setBestTimes(data);
      // Se recebemos dados simulados
      if (data && data.length > 0 && data[0].recommendationReason === 'Baseado em alto engajamento histórico') {
        setUsingSampleData(true);
      }
    } catch (err) {
      console.error('Erro ao carregar melhores horários:', err);
      throw err;
    }
  };
  
  const loadTrendingTopics = async () => {
    if (!creator) return;
    try {
      const data = await RecommendationService.getTrendingTopics(selectedPlatform);
      setTrendingTopics(data);
      // Se recebemos dados simulados
      if (data && data.length > 0 && data[0].id === '1') {
        setUsingSampleData(true);
      }
    } catch (err) {
      console.error('Erro ao carregar tópicos em tendência:', err);
      throw err;
    }
  };
  
  const loadGrowthRecommendations = async () => {
    if (!creator) return;
    try {
      const data = await RecommendationService.getAudienceGrowthRecommendations(creator.id);
      setGrowthRecommendations(data);
      // Se recebemos dados simulados
      if (data && data.length > 0 && data[0].id === '6') {
        setUsingSampleData(true);
      }
    } catch (err) {
      console.error('Erro ao carregar recomendações de crescimento:', err);
      throw err;
    }
  };
  
  // Converter dias da semana para nomes
  const getDayName = (day: number) => {
    const days = ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado'];
    return days[day];
  };
  
  if (userLoading || loading) {
    return (
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4, display: 'flex', justifyContent: 'center', alignItems: 'center', height: '70vh' }}>
        <CircularProgress />
      </Container>
    );
  }
  
  if (!creator) {
    return (
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
        <Alert severity="warning">
          Você precisa estar registrado como criador de conteúdo para acessar as recomendações.
        </Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Typography variant="h4" gutterBottom sx={{ mb: 0 }}>
            Recomendações e Métricas
          </Typography>
          
          {usingSampleData && (
            <Chip
              icon={<InfoIcon />}
              label="Dados de demonstração"
              color="info"
              variant="outlined"
              size="small"
            />
          )}
        </Box>
        
        <Box sx={{ display: 'flex', gap: 2 }}>
          <FormControl sx={{ minWidth: 200 }}>
            <InputLabel id="platform-select-label">Plataforma</InputLabel>
            <Select
              labelId="platform-select-label"
              value={selectedPlatform}
              label="Plataforma"
              onChange={handlePlatformChange}
            >
              <MenuItem value={SocialMediaPlatform.Instagram}>Instagram</MenuItem>
              <MenuItem value={SocialMediaPlatform.YouTube}>YouTube</MenuItem>
              <MenuItem value={SocialMediaPlatform.TikTok}>TikTok</MenuItem>
              <MenuItem value={SocialMediaPlatform.Facebook}>Facebook</MenuItem>
              <MenuItem value={SocialMediaPlatform.Twitter}>Twitter</MenuItem>
            </Select>
          </FormControl>
          
          <Button 
            variant="outlined" 
            startIcon={<RefreshIcon />}
            onClick={handleRefresh}
            disabled={refreshing}
          >
            {refreshing ? 'Atualizando...' : 'Atualizar'}
          </Button>
        </Box>
      </Box>
      
      {error && !usingSampleData && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}
      
      {usingSampleData && (
        <Alert severity="info" sx={{ mb: 3 }}>
          Mostrando dados de demonstração. Alguns recursos da API não estão disponíveis no momento.
        </Alert>
      )}

      <Grid container spacing={3}>
        {/* Gráfico de Engajamento */}
        <Grid item xs={12}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column', height: 400 }}>
            <Typography variant="h6" gutterBottom>
              Tendência de Engajamento
            </Typography>
            <ResponsiveContainer>
              <LineChart data={engagementData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Line type="monotone" dataKey="engagement" stroke="#8884d8" name="Engajamento (%)" />
                <Line type="monotone" dataKey="followers" stroke="#82ca9d" name="Seguidores" />
              </LineChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        {/* Recomendações Personalizadas */}
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>
              Recomendações Personalizadas
            </Typography>
            <List>
              {contentRecommendations.length > 0 ? (
                contentRecommendations.slice(0, 3).map((rec, index) => (
                  <React.Fragment key={rec.id || index}>
                    <ListItem>
                      <ListItemIcon>
                        {rec.recommendationType === 'FORMAT' ? <AssessmentIcon /> : 
                         rec.recommendationType === 'TOPIC' ? <LightbulbIcon /> : 
                         <ScheduleIcon />}
                      </ListItemIcon>
                      <ListItemText
                        primary={rec.title}
                        secondary={rec.description}
                      />
                    </ListItem>
                    <Box sx={{ px: 2, pb: 2 }}>
                      <LinearProgress
                        variant="determinate"
                        value={rec.score}
                        sx={{ height: 8, borderRadius: 4 }}
                      />
                    </Box>
                    {index < Math.min(contentRecommendations.length, 3) - 1 && <Divider />}
                  </React.Fragment>
                ))
              ) : (
                // Fallback para dados demonstrativos quando não há recomendações da API
                [{
                  title: 'Melhor Horário para Postagem',
                  description: 'Seus posts têm maior engajamento entre 18h e 20h',
                  icon: <ScheduleIcon />,
                  progress: 85,
                },
                {
                  title: 'Tipo de Conteúdo',
                  description: 'Vídeos curtos geram 2x mais engajamento que imagens',
                  icon: <AssessmentIcon />,
                  progress: 75,
                },
                {
                  title: 'Interação com Seguidores',
                  description: 'Responder comentários aumenta o engajamento em 40%',
                  icon: <GroupIcon />,
                  progress: 60,
                }].map((rec, index) => (
                  <React.Fragment key={index}>
                    <ListItem>
                      <ListItemIcon>{rec.icon}</ListItemIcon>
                      <ListItemText
                        primary={rec.title}
                        secondary={rec.description}
                      />
                    </ListItem>
                    <Box sx={{ px: 2, pb: 2 }}>
                      <LinearProgress
                        variant="determinate"
                        value={rec.progress}
                        sx={{ height: 8, borderRadius: 4 }}
                      />
                    </Box>
                    {index < 2 && <Divider />}
                  </React.Fragment>
                ))
              )}
            </List>
          </Paper>
        </Grid>

        {/* Melhores Horários para Postagem */}
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Melhores Horários ({selectedPlatform})
              </Typography>
              <List>
                {bestTimes.length > 0 ? (
                  bestTimes.slice(0, 5).map((time, index) => (
                    <React.Fragment key={index}>
                      <ListItem>
                        <ListItemIcon>
                          <ScheduleIcon color="primary" />
                        </ListItemIcon>
                        <ListItemText 
                          primary={`${getDayName(time.dayOfWeek)} às ${time.timeOfDay}`} 
                          secondary={time.recommendationReason}
                        />
                      </ListItem>
                      {index < Math.min(bestTimes.length, 5) - 1 && <Divider />}
                    </React.Fragment>
                  ))
                ) : (
                  growthTips.slice(0, 5).map((tip, index) => (
                    <React.Fragment key={index}>
                      <ListItem>
                        <ListItemIcon>
                          <LightbulbIcon color="primary" />
                        </ListItemIcon>
                        <ListItemText primary={tip} />
                      </ListItem>
                      {index < Math.min(growthTips.length, 5) - 1 && <Divider />}
                    </React.Fragment>
                  ))
                )}
              </List>
            </CardContent>
          </Card>
        </Grid>

        {/* Métricas de Crescimento */}
        <Grid item xs={12}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>
              Métricas de Crescimento
            </Typography>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6} md={3}>
                <Card>
                  <CardContent>
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                      <TrendingUpIcon color="primary" sx={{ mr: 1 }} />
                      <Typography variant="h6">+45%</Typography>
                    </Box>
                    <Typography color="text.secondary">
                      Crescimento de Seguidores
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
              <Grid item xs={12} sm={6} md={3}>
                <Card>
                  <CardContent>
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                      <TimelineIcon color="primary" sx={{ mr: 1 }} />
                      <Typography variant="h6">+32%</Typography>
                    </Box>
                    <Typography color="text.secondary">
                      Taxa de Engajamento
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
              <Grid item xs={12} sm={6} md={3}>
                <Card>
                  <CardContent>
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                      <AssessmentIcon color="primary" sx={{ mr: 1 }} />
                      <Typography variant="h6">2.5x</Typography>
                    </Box>
                    <Typography color="text.secondary">
                      Alcance por Post
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
              <Grid item xs={12} sm={6} md={3}>
                <Card>
                  <CardContent>
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                      <GroupIcon color="primary" sx={{ mr: 1 }} />
                      <Typography variant="h6">+28%</Typography>
                    </Box>
                    <Typography color="text.secondary">
                      Interação com Seguidores
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
            </Grid>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
};

export default RecommendationsPage; 