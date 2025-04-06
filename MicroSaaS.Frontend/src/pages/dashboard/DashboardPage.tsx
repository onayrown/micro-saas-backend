import React, { useState, useEffect } from 'react';
import {
  Container,
  Grid,
  Paper,
  Typography,
  Box,
  Chip,
  CircularProgress,
  Divider,
  Card,
  CardContent,
  Avatar,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  LinearProgress,
  Button,
  Alert,
} from '@mui/material';
import {
  TrendingUp as TrendingUpIcon,
  TrendingDown as TrendingDownIcon,
  Facebook as FacebookIcon,
  Twitter as TwitterIcon,
  Instagram as InstagramIcon,
  YouTube as YouTubeIcon,
  LinkedIn as LinkedInIcon,
  CheckCircle as CheckCircleIcon,
  Notifications as NotificationsIcon,
  Schedule as ScheduleIcon,
  Launch as LaunchIcon,
} from '@mui/icons-material';
import {
  BarChart,
  Bar,
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Legend,
} from 'recharts';
import api from '../../services/api';

// Dados simulados
const revenueData = [
  { name: 'Jan', receita: 4000 },
  { name: 'Fev', receita: 3000 },
  { name: 'Mar', receita: 5000 },
  { name: 'Abr', receita: 7000 },
  { name: 'Mai', receita: 6000 },
  { name: 'Jun', receita: 8000 },
  { name: 'Jul', receita: 10000 },
];

const followersData = [
  { platform: 'Facebook', seguidores: 5200, crescimento: 120, cor: '#1877F2' },
  { platform: 'Twitter', seguidores: 3100, crescimento: -50, cor: '#1DA1F2' },
  { platform: 'Instagram', seguidores: 12500, crescimento: 340, cor: '#E4405F' },
  { platform: 'YouTube', seguidores: 8700, crescimento: 220, cor: '#FF0000' },
  { platform: 'LinkedIn', seguidores: 2800, crescimento: 90, cor: '#0A66C2' },
];

const engagementData = [
  { name: 'Jan', facebook: 65, twitter: 45, instagram: 85, youtube: 55, linkedin: 35 },
  { name: 'Fev', facebook: 70, twitter: 50, instagram: 80, youtube: 60, linkedin: 40 },
  { name: 'Mar', facebook: 75, twitter: 55, instagram: 90, youtube: 65, linkedin: 45 },
  { name: 'Abr', facebook: 80, twitter: 60, instagram: 95, youtube: 70, linkedin: 50 },
  { name: 'Mai', facebook: 85, twitter: 65, instagram: 100, youtube: 75, linkedin: 55 },
  { name: 'Jun', facebook: 90, twitter: 70, instagram: 105, youtube: 80, linkedin: 60 },
  { name: 'Jul', facebook: 95, twitter: 75, instagram: 110, youtube: 85, linkedin: 65 },
];

// Notificações simuladas
const notifications = [
  { id: 1, message: 'Nova assinatura Premium adquirida', time: '10 minutos atrás' },
  { id: 2, message: 'Publicação agendada foi publicada', time: '1 hora atrás' },
  { id: 3, message: 'Aviso de cobrança: sua fatura vence em 3 dias', time: '5 horas atrás' },
  { id: 4, message: 'Post no Instagram alcançou 5.000 curtidas', time: '1 dia atrás' },
];

// Publicações agendadas simuladas
const scheduledPosts = [
  { id: 1, title: 'Lançamento de produto', platform: 'Instagram', time: 'Hoje, 18:00' },
  { id: 2, title: 'Webinar de marketing', platform: 'LinkedIn', time: 'Amanhã, 15:30' },
  { id: 3, title: 'Dicas de crescimento', platform: 'Twitter', time: '23/05, 09:00' },
];

// Função para obter o ícone da plataforma
const getPlatformIcon = (platform: string) => {
  switch (platform.toLowerCase()) {
    case 'facebook':
      return <FacebookIcon sx={{ color: '#1877F2' }} />;
    case 'twitter':
      return <TwitterIcon sx={{ color: '#1DA1F2' }} />;
    case 'instagram':
      return <InstagramIcon sx={{ color: '#E4405F' }} />;
    case 'youtube':
      return <YouTubeIcon sx={{ color: '#FF0000' }} />;
    case 'linkedin':
      return <LinkedInIcon sx={{ color: '#0A66C2' }} />;
    default:
      return <CheckCircleIcon />;
  }
};

const DashboardPage: React.FC = () => {
  const [loading, setLoading] = useState<boolean>(true);
  const [apiStatus, setApiStatus] = useState<string | null>(null);
  const [isTestingApi, setIsTestingApi] = useState(false);

  // Simula o carregamento de dados
  useEffect(() => {
    const timer = setTimeout(() => {
      setLoading(false);
    }, 1500);

    return () => clearTimeout(timer);
  }, []);

  const testApiConnection = async () => {
    setIsTestingApi(true);
    setApiStatus(null);
    try {
      console.log('🔍 Testando conexão com a API...');
      
      // Verificar e mostrar a URL base usada pelo cliente API
      console.log('⚠️ URL base do cliente API:', api.defaults.baseURL);
      
      // Usar apenas o caminho relativo - sem URL absoluta
      const response = await api.get('/SwaggerDebug/test');
      
      console.log('✅ Resposta da API:', response.data);
      
      // Criar uma mensagem formatada com os dados recebidos
      let successMessage = '✅ Conexão com a API estabelecida com sucesso!';
      if (response.data) {
        const dataString = typeof response.data === 'object' 
          ? JSON.stringify(response.data, null, 2)
          : response.data.toString();
        successMessage += `\n\nDados recebidos: ${dataString}`;
      }
      
      setApiStatus(successMessage);
    } catch (error: any) {
      console.error('❌ Erro ao testar API:', error);
      let errorMessage = error.message;
      
      // Log para debug da URL completa
      if (error.config) {
        console.error('🔍 URL completa que falhou:', `${error.config.baseURL || ''}${error.config.url || ''}`);
      }
      
      // Tentar obter mais detalhes do erro
      if (error.response) {
        // O servidor respondeu com um status de erro
        errorMessage = `Erro ${error.response.status}: ${error.response.statusText}`;
        if (error.response.data) {
          errorMessage += `\nDetalhes: ${JSON.stringify(error.response.data)}`;
        }
      } else if (error.request) {
        // A requisição foi feita mas não houve resposta
        errorMessage = 'Sem resposta do servidor. Verifique se o backend está rodando na porta 7171.';
      } 
      
      setApiStatus(`❌ Erro na conexão: ${errorMessage}`);
    } finally {
      setIsTestingApi(false);
    }
  };

  // Função para abrir o Swagger
  const openSwagger = () => {
    // Usar a mesma porta que a API
    window.open('https://localhost:7171/swagger/index.html', '_blank');
  };

  if (loading) {
    return (
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          height: '100vh',
        }}
      >
        <CircularProgress />
        <Typography variant="h6" sx={{ mt: 2 }}>
          Carregando dados do dashboard...
        </Typography>
      </Box>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ mb: 4, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4" gutterBottom>
          Dashboard
        </Typography>
        
        <Box sx={{ display: 'flex', gap: 2 }}>
          <Button 
            variant="outlined" 
            color="primary"
            onClick={openSwagger}
            startIcon={<LaunchIcon />}
          >
            Documentação API
          </Button>
          
          <Button 
            variant="contained" 
            color="primary"
            onClick={testApiConnection}
            disabled={isTestingApi}
          >
            {isTestingApi ? 'Testando...' : 'Testar Conexão com API'}
          </Button>
        </Box>
      </Box>
      
      {apiStatus && (
        <Alert 
          severity={apiStatus.includes('sucesso') ? 'success' : 'error'}
          sx={{ mb: 3 }}
        >
          {apiStatus}
        </Alert>
      )}

      {/* Resumo financeiro */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} md={4}>
          <Paper
            elevation={2}
            sx={{
              p: 3,
              display: 'flex',
              flexDirection: 'column',
              height: 140,
            }}
          >
            <Typography variant="h6" color="text.secondary" gutterBottom>
              Receita Total (Mensal)
            </Typography>
            <Typography component="p" variant="h4">
              R$ 38.000,00
            </Typography>
            <Box sx={{ display: 'flex', alignItems: 'center', mt: 1 }}>
              <TrendingUpIcon sx={{ color: 'success.main', mr: 1 }} />
              <Typography variant="body2" color="success.main">
                +12% desde o mês passado
              </Typography>
            </Box>
          </Paper>
        </Grid>
        <Grid item xs={12} md={4}>
          <Paper
            elevation={2}
            sx={{
              p: 3,
              display: 'flex',
              flexDirection: 'column',
              height: 140,
            }}
          >
            <Typography variant="h6" color="text.secondary" gutterBottom>
              Seguidores Totais
            </Typography>
            <Typography component="p" variant="h4">
              32.300
            </Typography>
            <Box sx={{ display: 'flex', alignItems: 'center', mt: 1 }}>
              <TrendingUpIcon sx={{ color: 'success.main', mr: 1 }} />
              <Typography variant="body2" color="success.main">
                +720 nos últimos 30 dias
              </Typography>
            </Box>
          </Paper>
        </Grid>
        <Grid item xs={12} md={4}>
          <Paper
            elevation={2}
            sx={{
              p: 3,
              display: 'flex',
              flexDirection: 'column',
              height: 140,
            }}
          >
            <Typography variant="h6" color="text.secondary" gutterBottom>
              Taxa de Engajamento
            </Typography>
            <Typography component="p" variant="h4">
              3.8%
            </Typography>
            <Box sx={{ display: 'flex', alignItems: 'center', mt: 1 }}>
              <TrendingDownIcon sx={{ color: 'error.main', mr: 1 }} />
              <Typography variant="body2" color="error.main">
                -0.2% desde o mês passado
              </Typography>
            </Box>
          </Paper>
        </Grid>
      </Grid>

      {/* Gráficos */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} md={8}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Receita Mensal
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={revenueData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis
                  tickFormatter={(value) => `R$ ${value.toLocaleString()}`}
                />
                <Tooltip
                  formatter={(value: any) => [`R$ ${value.toLocaleString()}`, 'Receita']}
                />
                <Legend />
                <Bar dataKey="receita" fill="#3f51b5" name="Receita (R$)" />
              </BarChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>
        <Grid item xs={12} md={4}>
          <Paper elevation={2} sx={{ p: 3, height: '100%' }}>
            <Typography variant="h6" gutterBottom>
              Seguidores por Plataforma
            </Typography>
            <Box sx={{ height: 300, overflow: 'auto' }}>
              {followersData.map((platform) => (
                <Box key={platform.platform} sx={{ mb: 2 }}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      {getPlatformIcon(platform.platform)}
                      <Typography variant="body1" sx={{ ml: 1 }}>
                        {platform.platform}
                      </Typography>
                    </Box>
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <Typography variant="body2">
                        {platform.seguidores.toLocaleString()}
                      </Typography>
                      <Chip
                        size="small"
                        icon={
                          platform.crescimento > 0 ? (
                            <TrendingUpIcon fontSize="small" />
                          ) : (
                            <TrendingDownIcon fontSize="small" />
                          )
                        }
                        label={`${platform.crescimento > 0 ? '+' : ''}${platform.crescimento}`}
                        color={platform.crescimento > 0 ? 'success' : 'error'}
                        sx={{ ml: 1 }}
                      />
                    </Box>
                  </Box>
                  <LinearProgress
                    variant="determinate"
                    value={(platform.seguidores / 15000) * 100}
                    sx={{
                      height: 8,
                      borderRadius: 5,
                      bgcolor: 'rgba(0,0,0,0.1)',
                      '& .MuiLinearProgress-bar': { bgcolor: platform.cor },
                    }}
                  />
                </Box>
              ))}
            </Box>
          </Paper>
        </Grid>
      </Grid>

      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Engajamento por Plataforma
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={engagementData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Line type="monotone" dataKey="facebook" stroke="#1877F2" name="Facebook" />
                <Line type="monotone" dataKey="twitter" stroke="#1DA1F2" name="Twitter" />
                <Line type="monotone" dataKey="instagram" stroke="#E4405F" name="Instagram" />
                <Line type="monotone" dataKey="youtube" stroke="#FF0000" name="YouTube" />
                <Line type="monotone" dataKey="linkedin" stroke="#0A66C2" name="LinkedIn" />
              </LineChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>
        <Grid item xs={12} md={4}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <Card elevation={2}>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <NotificationsIcon color="primary" />
                    <Typography variant="h6" sx={{ ml: 1 }}>
                      Notificações Recentes
                    </Typography>
                  </Box>
                  <Divider />
                  <List sx={{ maxHeight: 200, overflow: 'auto' }}>
                    {notifications.map((notification) => (
                      <React.Fragment key={notification.id}>
                        <ListItem alignItems="flex-start">
                          <ListItemAvatar>
                            <Avatar sx={{ bgcolor: 'primary.main' }}>
                              <NotificationsIcon />
                            </Avatar>
                          </ListItemAvatar>
                          <ListItemText
                            primary={notification.message}
                            secondary={notification.time}
                          />
                        </ListItem>
                        <Divider variant="inset" component="li" />
                      </React.Fragment>
                    ))}
                  </List>
                </CardContent>
              </Card>
            </Grid>
            <Grid item xs={12}>
              <Card elevation={2}>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <ScheduleIcon color="primary" />
                    <Typography variant="h6" sx={{ ml: 1 }}>
                      Publicações Agendadas
                    </Typography>
                  </Box>
                  <Divider />
                  <List sx={{ maxHeight: 200, overflow: 'auto' }}>
                    {scheduledPosts.map((post) => (
                      <React.Fragment key={post.id}>
                        <ListItem alignItems="flex-start">
                          <ListItemAvatar>
                            <Avatar sx={{ bgcolor: 'info.main' }}>
                              {getPlatformIcon(post.platform)}
                            </Avatar>
                          </ListItemAvatar>
                          <ListItemText
                            primary={post.title}
                            secondary={
                              <React.Fragment>
                                <Typography
                                  sx={{ display: 'inline' }}
                                  component="span"
                                  variant="body2"
                                  color="text.primary"
                                >
                                  {post.platform}
                                </Typography>
                                {` — ${post.time}`}
                              </React.Fragment>
                            }
                          />
                        </ListItem>
                        <Divider variant="inset" component="li" />
                      </React.Fragment>
                    ))}
                  </List>
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </Container>
  );
};

export default DashboardPage; 