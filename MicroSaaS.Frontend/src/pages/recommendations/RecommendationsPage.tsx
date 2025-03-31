import React from 'react';
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
} from '@mui/material';
import {
  TrendingUp as TrendingUpIcon,
  Lightbulb as LightbulbIcon,
  Timeline as TimelineIcon,
  Assessment as AssessmentIcon,
  Group as GroupIcon,
  Schedule as ScheduleIcon,
} from '@mui/icons-material';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';

// Dados simulados para demonstração
const engagementData = [
  { name: 'Jan', engagement: 65, followers: 1000 },
  { name: 'Fev', engagement: 68, followers: 1200 },
  { name: 'Mar', engagement: 72, followers: 1500 },
  { name: 'Abr', engagement: 75, followers: 1800 },
  { name: 'Mai', engagement: 78, followers: 2200 },
  { name: 'Jun', engagement: 82, followers: 2600 },
];

const recommendations = [
  {
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
  },
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
  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h4" gutterBottom>
        Recomendações e Métricas
      </Typography>

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
              {recommendations.map((rec, index) => (
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
                  {index < recommendations.length - 1 && <Divider />}
                </React.Fragment>
              ))}
            </List>
          </Paper>
        </Grid>

        {/* Dicas de Crescimento */}
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Dicas para Crescimento
              </Typography>
              <List>
                {growthTips.map((tip, index) => (
                  <React.Fragment key={index}>
                    <ListItem>
                      <ListItemIcon>
                        <LightbulbIcon color="primary" />
                      </ListItemIcon>
                      <ListItemText primary={tip} />
                    </ListItem>
                    {index < growthTips.length - 1 && <Divider />}
                  </React.Fragment>
                ))}
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