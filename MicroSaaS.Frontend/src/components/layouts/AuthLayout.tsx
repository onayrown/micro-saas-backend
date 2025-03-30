import React from 'react';
import { Outlet } from 'react-router-dom';
import { Container, Box, Paper, Typography } from '@mui/material';

const AuthLayout = () => {
  return (
    <Container component="main" maxWidth="sm">
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          width: '100%'
        }}
      >
        <Typography
          component="h1"
          variant="h4"
          sx={{ 
            mb: 4,
            fontWeight: 600,
            color: 'primary.main',
            textAlign: 'center',
          }}
        >
          MicroSaaS
        </Typography>
        <Paper
          elevation={3}
          sx={{
            p: { xs: 2, sm: 4 },
            width: '100%',
            borderRadius: 2,
          }}
        >
          <Outlet />
        </Paper>
        <Typography
          variant="body2"
          color="text.secondary"
          align="center"
          sx={{ mt: 5 }}
        >
          &copy; {new Date().getFullYear()} MicroSaaS. Todos os direitos reservados.
        </Typography>
      </Box>
    </Container>
  );
};

export default AuthLayout; 