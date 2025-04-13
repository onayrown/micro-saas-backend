import React, { useState, useEffect, useCallback, useMemo } from 'react';
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
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  IconButton,
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
  FilterList as FilterListIcon,
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
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { Dayjs } from 'dayjs';
import { useAuth } from '../../hooks/useAuth';
import DashboardService, { PerformanceMetrics, SocialMediaPlatform } from '../../services/DashboardService';
import dayjs from 'dayjs';
import 'dayjs/locale/pt-br';

dayjs.locale('pt-br');

// Lista de plataformas (mantida para o filtro, pode ser ajustada se necessário)
// const platforms = ['Todos', 'Instagram', 'YouTube', 'TikTok', 'Facebook', 'Twitter', 'LinkedIn'];

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

// Função para obter a cor da plataforma (NOVA)
const getPlatformColor = (platform: SocialMediaPlatform): string => {
  switch (platform) {
    case SocialMediaPlatform.Facebook:
      return '#1877F2';
    case SocialMediaPlatform.Twitter:
      return '#1DA1F2';
    case SocialMediaPlatform.Instagram:
      return '#E4405F';
    case SocialMediaPlatform.YouTube:
      return '#FF0000';
    case SocialMediaPlatform.LinkedIn:
      return '#0A66C2';
    case SocialMediaPlatform.TikTok:
      return '#000000'; // TikTok pode ter várias cores, usar preto como padrão
    case SocialMediaPlatform.Pinterest:
        return '#E60023';
    case SocialMediaPlatform.Snapchat:
        return '#FFFC00'; // Amarelo
    default:
      return '#808080'; // Cinza padrão
  }
};

const DashboardPage: React.FC = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState<boolean>(true);
  const [loadingMetrics, setLoadingMetrics] = useState<boolean>(false);
  const [apiStatus, setApiStatus] = useState<string | null>(null);
  const [isTestingApi, setIsTestingApi] = useState(false);

  // Estados para os filtros
  const [startDate, setStartDate] = useState<Dayjs | null>(null);
  const [endDate, setEndDate] = useState<Dayjs | null>(null);
  const [selectedPlatform, setSelectedPlatform] = useState<string>(SocialMediaPlatform.All);

  // Estado para os dados das métricas
  const [metricsData, setMetricsData] = useState<PerformanceMetrics[]>([]);
  const [fetchError, setFetchError] = useState<string | null>(null);

  // Função para buscar métricas
  const fetchMetrics = useCallback(async () => {
    if (!user?.id) {
      console.error("ID do criador não encontrado no contexto de autenticação.");
      setFetchError("Não foi possível identificar o usuário para buscar os dados.");
      return;
    }

    setLoadingMetrics(true);
    setFetchError(null);
    console.log('Buscando métricas com filtros:', { startDate, endDate, selectedPlatform });

    try {
      // Chama o serviço, que agora retorna o array diretamente ou vazio em caso de erro
      const data = await DashboardService.getDashboardMetrics(
        user.id,
        startDate,
        endDate,
        selectedPlatform
      );
      
      // Atualiza o estado com os dados recebidos (pode ser um array vazio)
      console.log('Métricas recebidas no componente:', data);
      setMetricsData(data); 

      // Limpa erro anterior se a busca for bem-sucedida (mesmo com array vazio)
      setFetchError(null); 

    } catch (error: any) { // O catch aqui só pegaria erros lançados pelo serviço (se o fizéssemos)
      console.error('Erro crítico ao buscar métricas (componente):', error);
      setFetchError('Erro de conexão ao buscar dados do dashboard.');
      setMetricsData([]); // Limpar dados em caso de erro crítico
    } finally {
      setLoadingMetrics(false);
    }
  }, [user, startDate, endDate, selectedPlatform]); // Dependências do useCallback

  // Buscar dados iniciais ao montar o componente
  useEffect(() => {
    setLoading(true);
    fetchMetrics().finally(() => setLoading(false));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // Handler para o botão "Aplicar Filtros"
  const handleApplyFilters = () => {
    fetchMetrics();
  };

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

  // Processar dados para o gráfico de receita mensal
  const monthlyRevenueData = useMemo(() => {
    if (!metricsData || metricsData.length === 0) {
      return [];
    }

    const revenueByMonth: { [key: string]: number } = {};

    metricsData.forEach(metric => {
      // Usar dayjs para formatar a data como 'YYYY-MM' para agrupar
      const monthKey = dayjs(metric.date).format('YYYY-MM');
      if (!revenueByMonth[monthKey]) {
        revenueByMonth[monthKey] = 0;
      }
      revenueByMonth[monthKey] += metric.estimatedRevenue;
    });

    // Converter para o formato esperado pelo gráfico e ordenar por mês
    return Object.entries(revenueByMonth)
      .map(([monthKey, revenue]) => ({
        // Usar dayjs para formatar o nome do mês (ex: 'Jan/24')
        monthName: dayjs(monthKey + '-01').format('MMM/YY'),
        revenue: parseFloat(revenue.toFixed(2)) // Arredondar para 2 casas decimais
      }))
      .sort((a, b) => dayjs(a.monthName, 'MMM/YY').valueOf() - dayjs(b.monthName, 'MMM/YY').valueOf()); // Ordenar

  }, [metricsData]); // Recalcular apenas quando metricsData mudar

  // Processar dados para a lista de seguidores por plataforma (NOVO)
  const followersByPlatformData = useMemo(() => {
    if (!metricsData || metricsData.length === 0) {
      return [];
    }

    // Agrupar métricas por plataforma
    const metricsByPlatform: { [key in SocialMediaPlatform]?: PerformanceMetrics[] } = {};
    metricsData.forEach(metric => {
      if (!metricsByPlatform[metric.platform]) {
        metricsByPlatform[metric.platform] = [];
      }
      metricsByPlatform[metric.platform]?.push(metric);
    });

    // Para cada plataforma, encontrar a métrica mais recente e extrair os seguidores
    const latestFollowers = Object.entries(metricsByPlatform).map(([platform, platformMetrics]) => {
      // Ordenar por data descendente e pegar a primeira (mais recente)
      const latestMetric = platformMetrics?.sort((a, b) => dayjs(b.date).valueOf() - dayjs(a.date).valueOf())[0];
      return {
        platform: platform as SocialMediaPlatform,
        followers: latestMetric?.followers ?? 0, // Usar 0 se não houver métrica
        color: getPlatformColor(platform as SocialMediaPlatform)
      };
    });

    // Ordenar por número de seguidores descendente
    return latestFollowers.sort((a, b) => b.followers - a.followers);

  }, [metricsData]);

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
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
        {/* Seção de Filtros e Teste API */}
        <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column', mb: 3 }}>
          <Typography variant="h6" gutterBottom>
            Filtros e Status
          </Typography>
          <Grid container spacing={2} alignItems="center">
            <Grid item xs={12} sm={6} md={3}>
              <DatePicker
                label="Data Início"
                value={startDate}
                onChange={(newValue) => setStartDate(newValue)}
                // renderInput={(params) => <TextField {...params} fullWidth margin="dense" />} // Verificar se isso é necessário com a versão atual do MUI
              />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <DatePicker
                label="Data Fim"
                value={endDate}
                onChange={(newValue) => setEndDate(newValue)}
                // renderInput={(params) => <TextField {...params} fullWidth margin="dense" />}
              />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <FormControl fullWidth margin="dense">
                <InputLabel>Plataforma</InputLabel>
                <Select
                  value={selectedPlatform}
                  label="Plataforma"
                  onChange={(e) => setSelectedPlatform(e.target.value)}
                >
                  {/* Usar o enum SocialMediaPlatform para gerar as opções */}
                  {Object.values(SocialMediaPlatform).map((platformValue) => (
                    <MenuItem key={platformValue} value={platformValue}>
                      {platformValue}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} sm={6} md={3} sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
              <Button
                variant="contained"
                startIcon={<FilterListIcon />}
                onClick={handleApplyFilters}
                disabled={loadingMetrics}
                sx={{ flexGrow: 1 }}
              >
                {loadingMetrics ? <CircularProgress size={24} /> : 'Aplicar Filtros'}
              </Button>
              <Button onClick={openSwagger} size="small">Swagger</Button>
              <Button onClick={testApiConnection} disabled={isTestingApi} size="small">
                {isTestingApi ? <CircularProgress size={20} /> : 'API Test'}
              </Button>
            </Grid>
          </Grid>
          {/* Exibir status da API */}
          {apiStatus && (
             <Alert severity={apiStatus.includes('❌') ? 'error' : 'success'} sx={{ mt: 2, whiteSpace: 'pre-wrap' }}>
               {apiStatus}
             </Alert>
           )}
          {/* Exibir erro de fetch */}
          {fetchError && (
            <Alert severity="error" sx={{ mt: 2 }}>
              {fetchError}
            </Alert>
          )}
        </Paper>

        {/* Grid Principal para os Cards/Gráficos */}
        <Grid container spacing={3}>

          {/* Card: Receita Mensal */}
          <Grid item xs={12} md={6}> {/* Ocupa 12 colunas em extra-small, 6 em medium+ */}
            <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column', height: 300 }}>
              <Typography variant="h6" gutterBottom>
                Receita Mensal Estimada
              </Typography>
              {loadingMetrics ? (
                  <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
                    <CircularProgress />
                  </Box>
              ) : monthlyRevenueData.length > 0 ? (
                  <ResponsiveContainer>
                    <LineChart data={monthlyRevenueData}>
                      <CartesianGrid strokeDasharray="3 3" />
                      {/* Usar monthName para o eixo X */}
                      <XAxis dataKey="monthName" />
                      <YAxis />
                      <Tooltip formatter={(value: number) => [`R$ ${value.toFixed(2)}`, "Receita"]} />
                      <Legend />
                      {/* Usar revenue para a linha */}
                      <Line type="monotone" dataKey="revenue" stroke="#8884d8" name="Receita Estimada" />
                    </LineChart>
                  </ResponsiveContainer>
              ) : (
                  <Typography sx={{ textAlign: 'center', mt: 4 }}>
                    Sem dados de receita para o período selecionado.
                  </Typography>
              )}
            </Paper>
          </Grid>

          {/* Card: Seguidores por Plataforma (ATUALIZADO) */}
          <Grid item xs={12} md={6}>
            <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column', height: 300 }}>
              <Typography variant="h6" gutterBottom>
                Seguidores por Plataforma (Último dado)
              </Typography>
              {loadingMetrics ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
                  <CircularProgress />
                </Box>
              ) : followersByPlatformData.length > 0 ? (
                <List dense sx={{ overflow: 'auto', flexGrow: 1 }}> {/* Adicionado overflow e flexGrow */}
                  {followersByPlatformData.map((data) => (
                    <ListItem key={data.platform} divider>
                      <ListItemAvatar>
                        <Avatar sx={{ bgcolor: data.color, width: 32, height: 32 }}> {/* Usar cor e ajustar tamanho */} 
                          {getPlatformIcon(data.platform)} 
                        </Avatar>
                      </ListItemAvatar>
                      <ListItemText
                        primary={data.platform}
                        secondary={`${data.followers.toLocaleString()} seguidores`}
                      />
                      {/* Opcional: Adicionar uma barra ou indicador visual */}
                    </ListItem>
                  ))}
                </List>
              ) : (
                <Typography sx={{ textAlign: 'center', mt: 4 }}>
                  Sem dados de seguidores para o período selecionado.
                </Typography>
              )}
            </Paper>
          </Grid>

          {/* Card: Taxa de Engajamento (REMOVIDO/TODO) */}
          {/*
          <Grid item xs={12} md={6}>
             <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column', height: 300 }}>
              <Typography variant="h6" gutterBottom>
                Taxa de Engajamento Média // TODO: Implementar com dados reais
              </Typography>
             // TODO: Adicionar gráfico ou visualização com dados de engagementRate de metricsData
             <Typography sx={{ textAlign: 'center', mt: 4 }}>
                Visualização de engajamento pendente.
             </Typography>
            </Paper>
          </Grid>
          */}

          {/* Card: Notificações (REMOVIDO/TODO) */}
          <Grid item xs={12} md={6}>
            <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column' }}>
              <Typography variant="h6" gutterBottom>
                Notificações // TODO: Implementar com API real
              </Typography>
              {/* TODO: Substituir por componente que busca e exibe notificações da API */}
              <Typography sx={{ textAlign: 'center', mt: 2, color: 'text.secondary' }}>
                Funcionalidade de notificações pendente.
              </Typography>
              {/* <List dense>
                {notifications.map((notif) => ( ... ))}
              </List> */}
            </Paper>
          </Grid>

          {/* Card: Publicações Agendadas (REMOVIDO/TODO) */}
          <Grid item xs={12} md={6}>
            <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column' }}>
              <Typography variant="h6" gutterBottom>
                Publicações Agendadas // TODO: Implementar com API real
              </Typography>
              {/* TODO: Substituir por componente que busca e exibe posts agendados da API */}
              <Typography sx={{ textAlign: 'center', mt: 2, color: 'text.secondary' }}>
                Funcionalidade de posts agendados pendente.
              </Typography>
              {/* <List dense>
                {scheduledPosts.map((post) => ( ... ))}
              </List> */}
            </Paper>
          </Grid>

        </Grid> {/* Fim do Grid Principal */}

      </Container>
    </LocalizationProvider>
  );
};

export default DashboardPage; 