import React from 'react';
import { Box, Button, Container, Typography } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline';

const NotFoundPage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <Container maxWidth="md">
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: '100vh',
          textAlign: 'center',
          py: 5,
        }}
      >
        <ErrorOutlineIcon sx={{ fontSize: 100, color: 'error.main', mb: 4 }} />
        <Typography variant="h1" component="h1" gutterBottom>
          404
        </Typography>
        <Typography variant="h4" component="h2" gutterBottom>
          Página não encontrada
        </Typography>
        <Typography variant="body1" color="textSecondary" paragraph>
          A página que você está procurando não existe ou foi movida.
        </Typography>
        <Box mt={4}>
          <Button
            variant="contained"
            color="primary"
            size="large"
            onClick={() => navigate('/')}
          >
            Voltar para o Dashboard
          </Button>
        </Box>
      </Box>
    </Container>
  );
};

export default NotFoundPage; 