import React from 'react';
import { Outlet } from 'react-router-dom';
import {
  Container,
  Box,
  Paper,
  Typography,
  Link,
  CssBaseline
} from '@mui/material';

function Copyright() {
  return (
    <Typography variant="body2" color="text.secondary" align="center">
      {'Copyright © '}
      <Link color="inherit" href="/">
        MicroSaaS
      </Link>{' '}
      {new Date().getFullYear()}
      {'.'}
    </Typography>
  );
}

const AuthLayout: React.FC = () => {
  return (
    <Box 
      sx={{ 
        minHeight: '100vh', 
        display: 'flex',
        flexDirection: 'column',
        backgroundColor: (t) => t.palette.mode === 'light' ? t.palette.grey[50] : t.palette.grey[900],
        backgroundImage: 'url(https://source.unsplash.com/random?marketing)',
        backgroundRepeat: 'no-repeat',
        backgroundSize: 'cover',
        backgroundPosition: 'center',
      }}
    >
      <CssBaseline />
      <Container 
        component={Paper} 
        maxWidth="sm" 
        sx={{ 
          my: 8, 
          py: 4, 
          px: 3, 
          display: 'flex', 
          flexDirection: 'column', 
          alignItems: 'center',
          boxShadow: 3,
          borderRadius: 2
        }}
      >
        <Typography component="h1" variant="h4" sx={{ mb: 4 }}>
          MicroSaaS
        </Typography>
        <Container maxWidth="xs">
          <Outlet />
        </Container>
        <Box mt={5}>
          <Copyright />
        </Box>
      </Container>
    </Box>
  );
};

export default AuthLayout; 