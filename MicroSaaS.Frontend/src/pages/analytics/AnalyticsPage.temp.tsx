import React from 'react';
import {
  Container,
  Typography,
  Box,
  Grid,
  Paper,
  Button,
  Alert
} from '@mui/material';
import { Link } from 'react-router-dom';

const AnalyticsPage: React.FC = () => {
  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ p: 3 }}>
        <Typography variant="h4" gutterBottom>
          Análise de Desempenho
        </Typography>
        
        <Alert severity="info" sx={{ mb: 3 }}>
          Esta página está em desenvolvimento. Em breve você poderá visualizar análises detalhadas do seu conteúdo.
        </Alert>
        
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Box sx={{ textAlign: 'center', py: 8 }}>
              <Typography variant="h6" gutterBottom>
                Estamos trabalhando para trazer análises detalhadas para você!
              </Typography>
              <Typography variant="body1" paragraph>
                Nesta página, você poderá visualizar métricas de desempenho, engajamento, crescimento de seguidores e muito mais.
              </Typography>
              <Button 
                variant="contained" 
                component={Link} 
                to="/dashboard"
                sx={{ mt: 2 }}
              >
                Voltar para o Dashboard
              </Button>
            </Box>
          </Grid>
        </Grid>
      </Paper>
    </Container>
  );
};

export default AnalyticsPage;
