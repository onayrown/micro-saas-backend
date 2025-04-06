import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Box,
  Grid,
  Paper,
  Tabs,
  Tab,
  CircularProgress,
  Alert,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  Button,
  Card,
  CardContent,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Divider,
  Chip,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow
} from '@mui/material';
import {
  BarChart,
  Bar,
  LineChart,
  Line,
  PieChart,
  Pie,
  Cell,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer
} from 'recharts';
import { formatCurrency, formatCompactNumber, formatPercentage } from '../../utils/formatUtils';
import { SelectChangeEvent } from '@mui/material/Select';
import {
  TrendingUp as TrendingUpIcon,
  TrendingDown as TrendingDownIcon,
  Timeline as TimelineIcon,
  Assessment as AssessmentIcon,
  Language as LanguageIcon,
  Schedule as ScheduleIcon,
  Refresh as RefreshIcon,
  Info as InfoIcon,
  Facebook as FacebookIcon,
  Instagram as InstagramIcon,
  YouTube as YouTubeIcon,
  Twitter as TwitterIcon,
  LinkedIn as LinkedInIcon
} from '@mui/icons-material';
import AnalyticsService, { 
  PerformanceMetric, 
  ContentPerformance,
  EngagementData,
  PlatformPerformance,
  PerformanceByContent,
  AudienceDemographic
} from '../../services/AnalyticsService';
import { SocialMediaPlatform } from '../../types/common';
import { useUser } from '../../contexts/UserContext';
import { Link } from 'react-router-dom';

// Tipos de análise
enum AnalyticsType {
  REVENUE = 'revenue',
  FOLLOWERS = 'followers',
  ENGAGEMENT = 'engagement',
}

// Dados simulados para receita
const revenueData = [
  { 
    date: 'Jan', 
    total: 4200, 
    adsense: 1500, 
    sponsorships: 2000, 
    affiliate: 700 
  },
  { 
    date: 'Fev', 
    total: 4800, 
    adsense: 1700, 
    sponsorships: 2200, 
    affiliate: 900 
  },
  { 
    date: 'Mar', 
    total: 5100, 
    adsense: 1800, 
    sponsorships: 2400, 
    affiliate: 900 
  },
  { 
    date: 'Abr', 
    total: 5800, 
    adsense: 2100, 
    sponsorships: 2700, 
    affiliate: 1000 
  },
  { 
    date: 'Mai', 
    total: 6200, 
    adsense: 2300, 
    sponsorships: 2900, 
    affiliate: 1000 
  },
  { 
    date: 'Jun', 
    total: 6800, 
    adsense: 2500, 
    sponsorships: 3200, 
    affiliate: 1100 
  },
];

// Dados simulados para distribuição de receita
const revenueDistributionData = [
  { name: 'AdSense', value: 2500 },
  { name: 'Patrocínios', value: 3200 },
  { name: 'Programa Afiliados', value: 1100 },
];

// Dados simulados para receita por plataforma
const revenueByPlatformData = [
  { platform: 'Instagram', value: 3200 },
  { platform: 'YouTube', value: 2100 },
  { platform: 'Twitter', value: 800 },
  { platform: 'LinkedIn', value: 500 },
  { platform: 'Facebook', value: 300 },
];

// Dados simulados para seguidores
const followersData = [
  { date: 'Jan', instagram: 4500, youtube: 2800, twitter: 1800, linkedin: 1200, facebook: 900 },
  { date: 'Fev', instagram: 4800, youtube: 3100, twitter: 1900, linkedin: 1300, facebook: 950 },
  { date: 'Mar', instagram: 5200, youtube: 3400, twitter: 2100, linkedin: 1400, facebook: 1000 },
  { date: 'Abr', instagram: 5600, youtube: 3800, twitter: 2300, linkedin: 1500, facebook: 1050 },
  { date: 'Mai', instagram: 6100, youtube: 4200, twitter: 2500, linkedin: 1600, facebook: 1100 },
  { date: 'Jun', instagram: 6700, youtube: 4500, twitter: 2700, linkedin: 1700, facebook: 1150 },
];

// Dados simulados para taxas de engajamento
const engagementData = [
  { date: 'Jan', instagram: 4.2, youtube: 5.1, twitter: 3.8, linkedin: 2.9, facebook: 2.1 },
  { date: 'Fev', instagram: 4.3, youtube: 5.3, twitter: 3.9, linkedin: 3.0, facebook: 2.2 },
  { date: 'Mar', instagram: 4.5, youtube: 5.5, twitter: 4.0, linkedin: 3.1, facebook: 2.3 },
  { date: 'Abr', instagram: 4.7, youtube: 5.8, twitter: 4.2, linkedin: 3.2, facebook: 2.4 },
  { date: 'Mai', instagram: 4.9, youtube: 6.1, twitter: 4.3, linkedin: 3.3, facebook: 2.5 },
  { date: 'Jun', instagram: 5.1, youtube: 6.3, twitter: 4.5, linkedin: 3.4, facebook: 2.6 },
];

// Cores para os gráficos
const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884d8'];
const PLATFORM_COLORS = {
  [SocialMediaPlatform.Instagram]: '#E1306C',
  [SocialMediaPlatform.YouTube]: '#FF0000',
  [SocialMediaPlatform.Twitter]: '#1DA1F2',
  [SocialMediaPlatform.LinkedIn]: '#0A66C2',
  [SocialMediaPlatform.Facebook]: '#1877F2',
  [SocialMediaPlatform.TikTok]: '#000000'
};

// Função para obter ícone da plataforma
const getPlatformIcon = (platform: SocialMediaPlatform) => {
  switch (platform) {
    case SocialMediaPlatform.Facebook:
      return <FacebookIcon sx={{ color: PLATFORM_COLORS[SocialMediaPlatform.Facebook] }} />;
    case SocialMediaPlatform.Twitter:
      return <TwitterIcon sx={{ color: PLATFORM_COLORS[SocialMediaPlatform.Twitter] }} />;
    case SocialMediaPlatform.Instagram:
      return <InstagramIcon sx={{ color: PLATFORM_COLORS[SocialMediaPlatform.Instagram] }} />;
    case SocialMediaPlatform.YouTube:
      return <YouTubeIcon sx={{ color: PLATFORM_COLORS[SocialMediaPlatform.YouTube] }} />;
    case SocialMediaPlatform.LinkedIn:
      return <LinkedInIcon sx={{ color: PLATFORM_COLORS[SocialMediaPlatform.LinkedIn] }} />;
    default:
      return <LanguageIcon />;
  }
};

const AnalyticsPage: React.FC = () => {
  const { creator, loading: userLoading } = useUser();
  
  // Estados para armazenar dados da API
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [refreshing, setRefreshing] = useState<boolean>(false);
  const [usingSampleData, setUsingSampleData] = useState<boolean>(false);
  const [timePeriod, setTimePeriod] = useState<string>('month');
  
  // Estados para armazenar os dados analíticos
  const [dashboardMetrics, setDashboardMetrics] = useState<PerformanceMetric[]>([]);
  const [contentPerformance, setContentPerformance] = useState<ContentPerformance[]>([]);
  const [engagementData, setEngagementData] = useState<EngagementData[]>([]);
  const [platformPerformance, setPlatformPerformance] = useState<PlatformPerformance[]>([]);
  const [performanceByContent, setPerformanceByContent] = useState<PerformanceByContent[]>([]);
  const [demographicData, setDemographicData] = useState<AudienceDemographic[]>([]);
  
  // Carregar dados iniciais quando o criador estiver disponível
  useEffect(() => {
    if (creator && !userLoading) {
      loadAnalyticsData();
    }
  }, [creator, userLoading]);
  
  // Função para carregar todos os dados de análise
  const loadAnalyticsData = async () => {
    if (!creator) return;
    
    setLoading(true);
    setError(null);
    setUsingSampleData(false);
    
    try {
      console.log('📊 Iniciando carregamento de dados analíticos para o criador:', creator.id);
      
      // Carregar todos os dados necessários
      const results = await Promise.allSettled([
        loadDashboardMetrics(),
        loadContentPerformance(),
        loadEngagementData(),
        loadPlatformPerformance(),
        loadPerformanceByContentType(),
        loadDemographicData()
      ]);
      
      // Verificar se alguma promise falhou
      const failedPromises = results.filter(result => result.status === 'rejected');
      if (failedPromises.length > 0) {
        console.warn(`⚠️ ${failedPromises.length} de ${results.length} requisições falharam`);
        setUsingSampleData(true);
      }
      
      console.log('✅ Dados analíticos carregados com sucesso, usando dados simulados:', usingSampleData);
    } catch (err) {
      console.error('❌ Erro ao carregar dados analíticos:', err);
      // Se estivermos usando dados simulados, não mostramos erro ao usuário
      if (!usingSampleData) {
        setError('Erro ao carregar dados analíticos. Usando dados simulados para demonstração.');
        setUsingSampleData(true);
      }
    } finally {
      setLoading(false);
    }
  };
  
  // Função para recarregar todos os dados
  const handleRefresh = async () => {
    if (!creator) return;
    
    setRefreshing(true);
    setUsingSampleData(false);
    try {
      await loadAnalyticsData();
    } catch (err) {
      if (!usingSampleData) {
        setError('Erro ao atualizar dados analíticos. Tente novamente mais tarde.');
      }
      console.error('Erro ao atualizar dados analíticos:', err);
    } finally {
      setRefreshing(false);
    }
  };
  
  // Função para lidar com a mudança de período
  const handlePeriodChange = (event: SelectChangeEvent<string>) => {
    setTimePeriod(event.target.value);
  };
  
  // Funções para carregar dados específicos
  const loadDashboardMetrics = async () => {
    if (!creator) return;
    try {
      const data = await AnalyticsService.getDashboardMetrics(creator.id);
      setDashboardMetrics(data);
      // Verificar se estamos usando dados simulados
      if (data && data.length > 0 && data[0].id === '1') {
        setUsingSampleData(true);
      }
    } catch (err) {
      console.error('Erro ao carregar métricas do dashboard:', err);
      throw err;
    }
  };
  
  const loadContentPerformance = async () => {
    if (!creator) return;
    try {
      const data = await AnalyticsService.getContentPerformance(creator.id);
      setContentPerformance(data);
      // Verificar se estamos usando dados simulados
      if (data && data.length > 0 && data[0].id === '1') {
        setUsingSampleData(true);
      }
    } catch (err) {
      console.error('Erro ao carregar performance de conteúdo:', err);
      throw err;
    }
  };
  
  const loadEngagementData = async () => {
    if (!creator) return;
    try {
      const startDate = getStartDateForPeriod(timePeriod);
      const endDate = new Date().toISOString().split('T')[0];
      const data = await AnalyticsService.getEngagementData(creator.id, startDate, endDate);
      setEngagementData(data);
      // Verificar se estamos usando dados simulados
      if (data && data.length > 0 && data[0].date === '2024-01-01') {
        setUsingSampleData(true);
      }
    } catch (err) {
      console.error('Erro ao carregar dados de engajamento:', err);
      throw err;
    }
  };
  
  const loadPlatformPerformance = async () => {
    if (!creator) return;
    try {
      const data = await AnalyticsService.getPlatformPerformance(creator.id);
      setPlatformPerformance(data);
      // Verificar se estamos usando dados simulados
      if (data && data.length > 0 && data[0].platform === SocialMediaPlatform.Instagram && data[0].followers === 58200) {
        setUsingSampleData(true);
      }
    } catch (err) {
      console.error('Erro ao carregar performance por plataforma:', err);
      throw err;
    }
  };
  
  const loadPerformanceByContentType = async () => {
    if (!creator) return;
    try {
      const data = await AnalyticsService.getPerformanceByContentType(creator.id);
      setPerformanceByContent(data);
      // Verificar se estamos usando dados simulados
      if (data && data.length > 0 && data[0].contentType === 'Vídeo') {
        setUsingSampleData(true);
      }
    } catch (err) {
      console.error('Erro ao carregar performance por tipo de conteúdo:', err);
      throw err;
    }
  };
  
  const loadDemographicData = async () => {
    if (!creator) return;
    try {
      const data = await AnalyticsService.getAudienceDemographics(creator.id);
      setDemographicData(data);
      // Verificar se estamos usando dados simulados
      if (data && data.length > 0 && data[0].ageGroup === '18-24') {
        setUsingSampleData(true);
      }
    } catch (err) {
      console.error('Erro ao carregar dados demográficos:', err);
      throw err;
    }
  };
  
  // Função para obter a data de início baseada no período selecionado
  const getStartDateForPeriod = (period: string): string => {
    const date = new Date();
    switch (period) {
      case 'week':
        date.setDate(date.getDate() - 7);
        break;
      case 'month':
        date.setMonth(date.getMonth() - 1);
        break;
      case 'quarter':
        date.setMonth(date.getMonth() - 3);
        break;
      case 'year':
        date.setFullYear(date.getFullYear() - 1);
        break;
      default:
        date.setMonth(date.getMonth() - 1);
    }
    return date.toISOString().split('T')[0];
  };
  
  if (userLoading || loading) {
    return (
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4, display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center', height: '70vh' }}>
        <CircularProgress size={60} thickness={4} />
        <Typography variant="h6" sx={{ mt: 2 }}>
          Carregando dados analíticos...
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          Isso pode levar alguns instantes
        </Typography>
      </Container>
    );
  }
  
  if (!creator) {
    return (
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
        <Alert severity="warning" sx={{ mb: 2 }}>
          Você precisa estar registrado como criador de conteúdo para acessar as análises.
        </Alert>
        <Button component={Link} to="/profile" variant="contained" color="primary">
          Completar Perfil
        </Button>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Typography variant="h4" gutterBottom sx={{ mb: 0 }}>
            Análise de Desempenho
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
            <InputLabel id="period-select-label">Período</InputLabel>
            <Select
              labelId="period-select-label"
              value={timePeriod}
              label="Período"
              onChange={handlePeriodChange}
            >
              <MenuItem value="week">Última Semana</MenuItem>
              <MenuItem value="month">Último Mês</MenuItem>
              <MenuItem value="quarter">Último Trimestre</MenuItem>
              <MenuItem value="year">Último Ano</MenuItem>
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
        {/* Métricas do Dashboard */}
        <Grid item xs={12}>
          <Grid container spacing={2}>
            {dashboardMetrics.map((metric, index) => (
              <Grid item xs={12} sm={6} md={3} key={metric.id || index}>
                <Card>
                  <CardContent>
                    <Typography color="text.secondary" gutterBottom>
                      {metric.label}
                    </Typography>
                    <Box sx={{ display: 'flex', alignItems: 'flex-end', gap: 1 }}>
                      <Typography variant="h4">
                        {metric.value.toLocaleString('pt-BR', { 
                          minimumFractionDigits: metric.value % 1 === 0 ? 0 : 1,
                          maximumFractionDigits: metric.value % 1 === 0 ? 0 : 1
                        })}
                      </Typography>
                      <Box 
                        sx={{ 
                          display: 'flex', 
                          alignItems: 'center', 
                          color: metric.trend === 'up' ? 'success.main' : 
                                 metric.trend === 'down' ? 'error.main' : 'text.secondary',
                          mb: 0.5
                        }}
                      >
                        {metric.trend === 'up' ? (
                          <TrendingUpIcon fontSize="small" sx={{ mr: 0.5 }} />
                        ) : metric.trend === 'down' ? (
                          <TrendingDownIcon fontSize="small" sx={{ mr: 0.5 }} />
                        ) : null}
                        <Typography variant="body2">
                          {metric.percentageChange.toFixed(1)}%
                        </Typography>
                      </Box>
                    </Box>
                    <Typography variant="caption" color="text.secondary">
                      vs {metric.period}
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        </Grid>

        {/* Gráfico de Engajamento */}
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column', height: 400 }}>
            <Typography variant="h6" gutterBottom>
              Tendência de Engajamento
            </Typography>
            <ResponsiveContainer>
              <LineChart
                data={engagementData}
                margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis yAxisId="left" orientation="left" />
                <YAxis yAxisId="right" orientation="right" />
                <Tooltip />
                <Legend />
                <Line 
                  yAxisId="left"
                  type="monotone" 
                  dataKey="engagement" 
                  stroke="#8884d8" 
                  name="Engajamento"
                  activeDot={{ r: 8 }}
                />
                <Line 
                  yAxisId="right"
                  type="monotone" 
                  dataKey="impressions" 
                  stroke="#82ca9d" 
                  name="Impressões"
                />
                <Line 
                  yAxisId="left"
                  type="monotone" 
                  dataKey="shares" 
                  stroke="#ff7300" 
                  name="Compartilhamentos"
                />
              </LineChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        {/* Performance por Tipo de Conteúdo */}
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column', height: 400 }}>
            <Typography variant="h6" gutterBottom>
              Performance por Tipo de Conteúdo
            </Typography>
            <ResponsiveContainer>
              <BarChart
                data={performanceByContent}
                layout="vertical"
                margin={{ top: 5, right: 30, left: 50, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis type="number" />
                <YAxis dataKey="contentType" type="category" />
                <Tooltip />
                <Legend />
                <Bar dataKey="engagementRate" name="Taxa de Engajamento (%)" fill="#8884d8" />
              </BarChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        {/* Tabela de Performance de Conteúdo */}
        <Grid item xs={12}>
          <Paper sx={{ p: 2, width: '100%', overflow: 'hidden' }}>
            <Typography variant="h6" gutterBottom>
              Top Conteúdos por Performance
            </Typography>
            <TableContainer sx={{ maxHeight: 440 }}>
              <Table stickyHeader aria-label="performance table">
                <TableHead>
                  <TableRow>
                    <TableCell>Título</TableCell>
                    <TableCell>Plataforma</TableCell>
                    <TableCell>Tipo</TableCell>
                    <TableCell align="right">Impressões</TableCell>
                    <TableCell align="right">Engajamento</TableCell>
                    <TableCell align="right">Taxa de Engajamento</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {contentPerformance.map((post) => (
                    <TableRow key={post.id} hover>
                      <TableCell component="th" scope="row">
                        {post.title}
                      </TableCell>
                      <TableCell>
                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                          {getPlatformIcon(post.platform)}
                          <Typography variant="body2" sx={{ ml: 1 }}>
                            {post.platform}
                          </Typography>
                        </Box>
                      </TableCell>
                      <TableCell>{post.type}</TableCell>
                      <TableCell align="right">{post.impressions.toLocaleString('pt-BR')}</TableCell>
                      <TableCell align="right">{post.engagement.toLocaleString('pt-BR')}</TableCell>
                      <TableCell align="right">{post.engagementRate.toFixed(1)}%</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        </Grid>

        {/* Performance por Plataforma */}
        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column', height: 400 }}>
            <Typography variant="h6" gutterBottom>
              Performance por Plataforma
            </Typography>
            <ResponsiveContainer>
              <BarChart
                data={platformPerformance}
                margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="platform" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="engagement" name="Engajamento (%)" fill="#8884d8" />
                <Bar dataKey="growth" name="Crescimento (%)" fill="#82ca9d" />
              </BarChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        {/* Demografia da Audiência */}
        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column', height: 400 }}>
            <Typography variant="h6" gutterBottom>
              Demografia da Audiência
            </Typography>
            <ResponsiveContainer>
              <PieChart>
                <Pie
                  data={demographicData}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={({ name, percent }) => `${name} (${(percent * 100).toFixed(1)}%)`}
                  outerRadius={120}
                  fill="#8884d8"
                  dataKey="percentage"
                  nameKey="ageGroup"
                >
                  {demographicData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip formatter={(value) => `${value}%`} />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
};

export default AnalyticsPage; 