import React, { useState } from 'react';
import {
  Container,
  Typography,
  Box,
  Paper,
  Grid,
  TextField,
  Button,
  Avatar,
  Divider,
  Tab,
  Tabs,
  CircularProgress,
  InputAdornment,
  IconButton,
  Alert,
  FormControl,
  FormControlLabel,
  FormLabel,
  Radio,
  RadioGroup,
  Card,
  CardContent,
  Switch
} from '@mui/material';
import {
  Person as PersonIcon,
  Edit as EditIcon,
  Save as SaveIcon,
  Visibility as VisibilityIcon,
  VisibilityOff as VisibilityOffIcon,
  Key as KeyIcon,
  Delete as DeleteIcon,
  Language as LanguageIcon,
  Notifications as NotificationsIcon,
  ColorLens as ColorLensIcon,
} from '@mui/icons-material';
import { useAuth } from '../../hooks/useAuth';
import { useTheme } from '../../hooks/useTheme';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`profile-tabpanel-${index}`}
      aria-labelledby={`profile-tab-${index}`}
      {...other}
    >
      {value === index && (
        <Box sx={{ pt: 3 }}>
          {children}
        </Box>
      )}
    </div>
  );
}

function a11yProps(index: number) {
  return {
    id: `profile-tab-${index}`,
    'aria-controls': `profile-tabpanel-${index}`,
  };
}

const ProfilePage = () => {
  const { user } = useAuth();
  const { mode, setThemeMode } = useTheme();
  const [tabValue, setTabValue] = useState(0);
  const [isEditing, setIsEditing] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  const [formData, setFormData] = useState({
    name: user?.name || '',
    email: user?.email || '',
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
    language: 'pt-BR',
    timezone: 'America/Sao_Paulo',
    emailNotifications: true,
    browserNotifications: true,
  });

  const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const handleToggleEditMode = () => {
    setIsEditing(!isEditing);
  };

  const handleTogglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, checked, type } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : value,
    });
  };

  const handleUpdateProfile = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setSuccessMessage('');
    setErrorMessage('');

    try {
      // Simulando uma requisição ao backend
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      setSuccessMessage('Perfil atualizado com sucesso!');
      setIsEditing(false);
    } catch (error) {
      setErrorMessage('Erro ao atualizar perfil. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setSuccessMessage('');
    setErrorMessage('');

    try {
      // Validar as senhas
      if (formData.newPassword !== formData.confirmPassword) {
        throw new Error('As senhas não coincidem.');
      }

      if (formData.newPassword.length < 8) {
        throw new Error('A nova senha deve ter pelo menos 8 caracteres.');
      }

      // Simulando uma requisição ao backend
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      setSuccessMessage('Senha alterada com sucesso!');
      setFormData({
        ...formData,
        currentPassword: '',
        newPassword: '',
        confirmPassword: '',
      });
    } catch (error) {
      if (error instanceof Error) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage('Erro ao alterar senha. Tente novamente.');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleThemeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setThemeMode(event.target.value as 'light' | 'dark');
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ p: 3 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
          <Avatar
            sx={{ width: 64, height: 64, mr: 2 }}
          >
            {user?.name?.charAt(0) || <PersonIcon />}
          </Avatar>
          <Box>
            <Typography variant="h5">{user?.name || 'Usuário'}</Typography>
            <Typography variant="body2" color="text.secondary">
              {user?.email || 'email@exemplo.com'}
            </Typography>
          </Box>
        </Box>

        <Divider sx={{ mb: 2 }} />

        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tabs
            value={tabValue}
            onChange={handleTabChange}
            aria-label="perfil tabs"
            variant="scrollable"
            scrollButtons="auto"
          >
            <Tab label="Informações Pessoais" icon={<PersonIcon />} iconPosition="start" {...a11yProps(0)} />
            <Tab label="Segurança" icon={<KeyIcon />} iconPosition="start" {...a11yProps(1)} />
            <Tab label="Preferências" icon={<ColorLensIcon />} iconPosition="start" {...a11yProps(2)} />
            <Tab label="Notificações" icon={<NotificationsIcon />} iconPosition="start" {...a11yProps(3)} />
          </Tabs>
        </Box>

        <TabPanel value={tabValue} index={0}>
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
            <Button
              variant="outlined"
              startIcon={isEditing ? <SaveIcon /> : <EditIcon />}
              onClick={handleToggleEditMode}
              disabled={loading}
            >
              {isEditing ? 'Salvar' : 'Editar'}
            </Button>
          </Box>

          {successMessage && (
            <Alert severity="success" sx={{ mb: 2 }}>
              {successMessage}
            </Alert>
          )}

          {errorMessage && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {errorMessage}
            </Alert>
          )}

          <Box component="form" onSubmit={handleUpdateProfile}>
            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Nome completo"
                  name="name"
                  value={formData.name}
                  onChange={handleChange}
                  disabled={!isEditing || loading}
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Email"
                  name="email"
                  type="email"
                  value={formData.email}
                  onChange={handleChange}
                  disabled={!isEditing || loading}
                  required
                />
              </Grid>

              {isEditing && (
                <Grid item xs={12}>
                  <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 2 }}>
                    <Button
                      type="submit"
                      variant="contained"
                      startIcon={loading ? <CircularProgress size={20} /> : <SaveIcon />}
                      disabled={loading}
                    >
                      Salvar Alterações
                    </Button>
                  </Box>
                </Grid>
              )}
            </Grid>
          </Box>
        </TabPanel>

        <TabPanel value={tabValue} index={1}>
          <Typography variant="h6" gutterBottom>
            Alterar Senha
          </Typography>

          {successMessage && (
            <Alert severity="success" sx={{ mb: 2 }}>
              {successMessage}
            </Alert>
          )}

          {errorMessage && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {errorMessage}
            </Alert>
          )}

          <Box component="form" onSubmit={handleChangePassword}>
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Senha atual"
                  name="currentPassword"
                  type={showPassword ? 'text' : 'password'}
                  value={formData.currentPassword}
                  onChange={handleChange}
                  required
                  InputProps={{
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          aria-label="toggle password visibility"
                          onClick={handleTogglePasswordVisibility}
                          edge="end"
                        >
                          {showPassword ? <VisibilityOffIcon /> : <VisibilityIcon />}
                        </IconButton>
                      </InputAdornment>
                    ),
                  }}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Nova senha"
                  name="newPassword"
                  type={showPassword ? 'text' : 'password'}
                  value={formData.newPassword}
                  onChange={handleChange}
                  required
                  helperText="Mínimo de 8 caracteres"
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Confirmar nova senha"
                  name="confirmPassword"
                  type={showPassword ? 'text' : 'password'}
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  required
                  error={formData.newPassword !== formData.confirmPassword && formData.confirmPassword !== ''}
                  helperText={
                    formData.newPassword !== formData.confirmPassword && formData.confirmPassword !== ''
                      ? 'As senhas não coincidem'
                      : ''
                  }
                />
              </Grid>
              <Grid item xs={12}>
                <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 2 }}>
                  <Button
                    type="submit"
                    variant="contained"
                    startIcon={loading ? <CircularProgress size={20} /> : <SaveIcon />}
                    disabled={loading}
                  >
                    Alterar Senha
                  </Button>
                </Box>
              </Grid>
            </Grid>
          </Box>
        </TabPanel>

        <TabPanel value={tabValue} index={2}>
          <Typography variant="h6" gutterBottom>
            Preferências do Sistema
          </Typography>

          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <Card variant="outlined">
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    <ColorLensIcon sx={{ mr: 1, verticalAlign: 'middle' }} />
                    Tema
                  </Typography>
                  <FormControl component="fieldset" sx={{ mt: 2 }}>
                    <FormLabel component="legend">Tema da Interface</FormLabel>
                    <RadioGroup
                      aria-label="theme"
                      name="theme-mode"
                      value={mode}
                      onChange={handleThemeChange}
                      row
                    >
                      <FormControlLabel value="light" control={<Radio />} label="Claro" />
                      <FormControlLabel value="dark" control={<Radio />} label="Escuro" />
                    </RadioGroup>
                  </FormControl>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} md={6}>
              <Card variant="outlined">
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    <LanguageIcon sx={{ mr: 1, verticalAlign: 'middle' }} />
                    Região e Idioma
                  </Typography>
                  <FormControl fullWidth sx={{ mb: 2 }}>
                    <FormLabel id="language-label">Idioma</FormLabel>
                    <RadioGroup
                      aria-labelledby="language-label"
                      name="language"
                      value={formData.language}
                      onChange={handleChange}
                    >
                      <FormControlLabel value="pt-BR" control={<Radio />} label="Português (Brasil)" />
                      <FormControlLabel value="en-US" control={<Radio />} label="English (United States)" />
                      <FormControlLabel value="es" control={<Radio />} label="Español" />
                    </RadioGroup>
                  </FormControl>

                  <FormControl fullWidth>
                    <FormLabel id="timezone-label">Fuso Horário</FormLabel>
                    <TextField
                      select
                      name="timezone"
                      value={formData.timezone}
                      onChange={handleChange}
                      SelectProps={{
                        native: true,
                      }}
                      size="small"
                      sx={{ mt: 1 }}
                    >
                      <option value="America/Sao_Paulo">Brasília (GMT-3)</option>
                      <option value="America/New_York">Nova York (GMT-5)</option>
                      <option value="Europe/London">Londres (GMT+0)</option>
                      <option value="Europe/Paris">Paris (GMT+1)</option>
                      <option value="Asia/Tokyo">Tóquio (GMT+9)</option>
                    </TextField>
                  </FormControl>
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </TabPanel>

        <TabPanel value={tabValue} index={3}>
          <Typography variant="h6" gutterBottom>
            Configurações de Notificações
          </Typography>

          <Card variant="outlined">
            <CardContent>
              <Typography variant="subtitle1" gutterBottom>
                Notificações por Email
              </Typography>
              <Box sx={{ mb: 2 }}>
                <FormControlLabel
                  control={
                    <Switch
                      checked={formData.emailNotifications}
                      onChange={handleChange}
                      name="emailNotifications"
                    />
                  }
                  label="Receber notificações por email"
                />
              </Box>

              <Divider sx={{ my: 2 }} />

              <Typography variant="subtitle1" gutterBottom>
                Notificações no Navegador
              </Typography>
              <Box>
                <FormControlLabel
                  control={
                    <Switch
                      checked={formData.browserNotifications}
                      onChange={handleChange}
                      name="browserNotifications"
                    />
                  }
                  label="Receber notificações no navegador"
                />
              </Box>
            </CardContent>
          </Card>
        </TabPanel>
      </Paper>
    </Container>
  );
};

export default ProfilePage; 