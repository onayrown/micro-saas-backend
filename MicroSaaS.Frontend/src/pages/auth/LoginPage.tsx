import React, { useState } from 'react';
import { 
  TextField, 
  Button, 
  Box, 
  Typography, 
  Link, 
  InputAdornment, 
  IconButton,
  Alert,
  CircularProgress,
  Divider,
  Paper
} from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { useAuth } from '../../hooks/useAuth';

const LoginPage = () => {
  const { login } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login(email, password);
    } catch (err: any) {
      console.error('Erro no login:', err);
      if (err.response?.data?.message) {
        setError(err.response.data.message);
      } else if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('Falha ao fazer login. Verifique suas credenciais.');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleTogglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  const isDevelopment = process.env.NODE_ENV === 'development';

  return (
    <Box component="form" onSubmit={handleSubmit} noValidate sx={{ width: '100%' }}>
      <Typography component="h1" variant="h5" align="center" gutterBottom>
        Login
      </Typography>
      <Typography variant="body2" color="text.secondary" align="center" sx={{ mb: 3 }}>
        Entre com suas credenciais para acessar sua conta
      </Typography>
      
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      {isDevelopment && (
        <Alert severity="info" sx={{ mb: 2 }}>
          Em ambiente de desenvolvimento: qualquer email e senha serão aceitos.
        </Alert>
      )}
      
      <TextField
        margin="normal"
        required
        fullWidth
        id="email"
        label="E-mail"
        name="email"
        autoComplete="email"
        autoFocus
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        disabled={loading}
      />
      
      <TextField
        margin="normal"
        required
        fullWidth
        name="password"
        label="Senha"
        type={showPassword ? 'text' : 'password'}
        id="password"
        autoComplete="current-password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        disabled={loading}
        InputProps={{
          endAdornment: (
            <InputAdornment position="end">
              <IconButton
                aria-label="toggle password visibility"
                onClick={handleTogglePasswordVisibility}
                edge="end"
              >
                {showPassword ? <VisibilityOff /> : <Visibility />}
              </IconButton>
            </InputAdornment>
          ),
        }}
      />
      
      <Button
        type="submit"
        fullWidth
        variant="contained"
        sx={{ mt: 3, mb: 2, py: 1.2 }}
        disabled={loading}
      >
        {loading ? <CircularProgress size={24} /> : 'Entrar'}
      </Button>
      
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 2, flexWrap: 'wrap' }}>
        <Link component={RouterLink} to="/forgot-password" variant="body2" sx={{ mb: { xs: 1, sm: 0 } }}>
          Esqueceu a senha?
        </Link>
        <Link component={RouterLink} to="/register" variant="body2">
          Não tem uma conta? Registre-se
        </Link>
      </Box>
    </Box>
  );
};

export default LoginPage; 