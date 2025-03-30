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
  ResponsiveContainer,
} from 'recharts';
import { formatCurrency, formatCompactNumber, formatPercentage } from '../../utils/formatUtils';
import { SelectChangeEvent } from '@mui/material/Select';

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
  instagram: '#E1306C',
  youtube: '#FF0000',
  twitter: '#1DA1F2',
  linkedin: '#0A66C2',
  facebook: '#1877F2',
};

const AnalyticsPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<AnalyticsType>(AnalyticsType.REVENUE);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [timeRange, setTimeRange] = useState<string>('6m');
  const [platform, setPlatform] = useState<string>('all');

  // Simula o carregamento de dados
  useEffect(() => {
    const loadData = async () => {
      setLoading(true);
      try {
        // Simula chamada à API
        await new Promise((resolve) => setTimeout(resolve, 1000));
        setError(null);
      } catch (err) {
        setError('Erro ao carregar dados de análise. Tente novamente.');
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [activeTab, timeRange, platform]);

  const handleTabChange = (_event: React.SyntheticEvent, newValue: AnalyticsType) => {
    setActiveTab(newValue);
  };

  const handleTimeRangeChange = (event: SelectChangeEvent<string>) => {
    setTimeRange(event.target.value);
  };

  const handlePlatformChange = (event: SelectChangeEvent<string>) => {
    setPlatform(event.target.value);
  };

  const renderRevenueAnalytics = () => {
    return (
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Receita Total ao Longo do Tempo
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart
                data={revenueData}
                margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis
                  tickFormatter={(value) => `R$ ${formatCompactNumber(value)}`}
                />
                <Tooltip 
                  formatter={(value: any) => [`${formatCurrency(value)}`, 'Receita']}
                  labelFormatter={(label) => `Data: ${label}`}
                />
                <Legend />
                <Line
                  type="monotone"
                  dataKey="total"
                  name="Receita Total"
                  stroke="#8884d8"
                  activeDot={{ r: 8 }}
                />
                <Line type="monotone" dataKey="adsense" name="AdSense" stroke="#82ca9d" />
                <Line type="monotone" dataKey="sponsorships" name="Patrocínios" stroke="#ffc658" />
                <Line type="monotone" dataKey="affiliate" name="Afiliados" stroke="#ff8042" />
              </LineChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 3, height: '100%' }}>
            <Typography variant="h6" gutterBottom>
              Distribuição de Receita
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  dataKey="value"
                  data={revenueDistributionData}
                  cx="50%"
                  cy="50%"
                  outerRadius={80}
                  fill="#8884d8"
                  label={({ name, percent }) => `${name}: ${formatPercentage(percent)}`}
                >
                  {revenueDistributionData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip formatter={(value: any) => [formatCurrency(value), 'Receita']} />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 3, height: '100%' }}>
            <Typography variant="h6" gutterBottom>
              Receita por Plataforma
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart
                data={revenueByPlatformData}
                margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="platform" />
                <YAxis 
                  tickFormatter={(value) => `R$ ${formatCompactNumber(value)}`}
                />
                <Tooltip formatter={(value: any) => [formatCurrency(value), 'Receita']} />
                <Legend />
                <Bar dataKey="value" name="Receita" fill="#8884d8">
                  {revenueByPlatformData.map((entry, index) => (
                    <Cell 
                      key={`cell-${index}`} 
                      fill={
                        entry.platform.toLowerCase() === 'instagram' ? PLATFORM_COLORS.instagram :
                        entry.platform.toLowerCase() === 'youtube' ? PLATFORM_COLORS.youtube :
                        entry.platform.toLowerCase() === 'twitter' ? PLATFORM_COLORS.twitter :
                        entry.platform.toLowerCase() === 'linkedin' ? PLATFORM_COLORS.linkedin :
                        entry.platform.toLowerCase() === 'facebook' ? PLATFORM_COLORS.facebook :
                        COLORS[index % COLORS.length]
                      } 
                    />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>
      </Grid>
    );
  };

  const renderFollowersAnalytics = () => {
    return (
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Crescimento de Seguidores
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart
                data={followersData}
                margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis 
                  tickFormatter={(value) => formatCompactNumber(value)}
                />
                <Tooltip formatter={(value: any, name) => [
                  formatCompactNumber(value),
                  name === 'instagram' ? 'Instagram' :
                  name === 'youtube' ? 'YouTube' :
                  name === 'twitter' ? 'Twitter' :
                  name === 'linkedin' ? 'LinkedIn' :
                  name === 'facebook' ? 'Facebook' : name
                ]} />
                <Legend />
                <Line type="monotone" dataKey="instagram" name="Instagram" stroke={PLATFORM_COLORS.instagram} />
                <Line type="monotone" dataKey="youtube" name="YouTube" stroke={PLATFORM_COLORS.youtube} />
                <Line type="monotone" dataKey="twitter" name="Twitter" stroke={PLATFORM_COLORS.twitter} />
                <Line type="monotone" dataKey="linkedin" name="LinkedIn" stroke={PLATFORM_COLORS.linkedin} />
                <Line type="monotone" dataKey="facebook" name="Facebook" stroke={PLATFORM_COLORS.facebook} />
              </LineChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>
      </Grid>
    );
  };

  const renderEngagementAnalytics = () => {
    return (
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Taxa de Engajamento por Plataforma
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart
                data={engagementData}
                margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis 
                  tickFormatter={(value) => `${value}%`}
                />
                <Tooltip formatter={(value: any) => [`${value}%`, 'Engajamento']} />
                <Legend />
                <Line type="monotone" dataKey="instagram" name="Instagram" stroke={PLATFORM_COLORS.instagram} />
                <Line type="monotone" dataKey="youtube" name="YouTube" stroke={PLATFORM_COLORS.youtube} />
                <Line type="monotone" dataKey="twitter" name="Twitter" stroke={PLATFORM_COLORS.twitter} />
                <Line type="monotone" dataKey="linkedin" name="LinkedIn" stroke={PLATFORM_COLORS.linkedin} />
                <Line type="monotone" dataKey="facebook" name="Facebook" stroke={PLATFORM_COLORS.facebook} />
              </LineChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>
      </Grid>
    );
  };

  const renderContent = () => {
    if (loading) {
      return (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
          <CircularProgress />
        </Box>
      );
    }

    if (error) {
      return (
        <Alert severity="error" sx={{ mt: 2 }}>
          {error}
        </Alert>
      );
    }

    switch (activeTab) {
      case AnalyticsType.REVENUE:
        return renderRevenueAnalytics();
      case AnalyticsType.FOLLOWERS:
        return renderFollowersAnalytics();
      case AnalyticsType.ENGAGEMENT:
        return renderEngagementAnalytics();
      default:
        return null;
    }
  };

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h4" gutterBottom>
        Análises
      </Typography>

      <Tabs
        value={activeTab}
        onChange={handleTabChange}
        indicatorColor="primary"
        textColor="primary"
        sx={{ mb: 3 }}
      >
        <Tab label="Receita" value={AnalyticsType.REVENUE} />
        <Tab label="Seguidores" value={AnalyticsType.FOLLOWERS} />
        <Tab label="Engajamento" value={AnalyticsType.ENGAGEMENT} />
      </Tabs>

      <Paper sx={{ p: 2, mb: 3 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} md={3}>
            <FormControl fullWidth>
              <InputLabel id="time-range-label">Período</InputLabel>
              <Select
                labelId="time-range-label"
                id="time-range"
                value={timeRange}
                label="Período"
                onChange={handleTimeRangeChange}
              >
                <MenuItem value="30d">Últimos 30 dias</MenuItem>
                <MenuItem value="3m">Últimos 3 meses</MenuItem>
                <MenuItem value="6m">Últimos 6 meses</MenuItem>
                <MenuItem value="1y">Último ano</MenuItem>
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={12} md={3}>
            <FormControl fullWidth>
              <InputLabel id="platform-label">Plataforma</InputLabel>
              <Select
                labelId="platform-label"
                id="platform"
                value={platform}
                label="Plataforma"
                onChange={handlePlatformChange}
              >
                <MenuItem value="all">Todas as plataformas</MenuItem>
                <MenuItem value="instagram">Instagram</MenuItem>
                <MenuItem value="youtube">YouTube</MenuItem>
                <MenuItem value="twitter">Twitter</MenuItem>
                <MenuItem value="linkedin">LinkedIn</MenuItem>
                <MenuItem value="facebook">Facebook</MenuItem>
              </Select>
            </FormControl>
          </Grid>
        </Grid>
      </Paper>

      {renderContent()}
    </Container>
  );
};

export default AnalyticsPage; 