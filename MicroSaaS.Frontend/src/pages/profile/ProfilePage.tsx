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
  Alert,
  FormControl,
  FormControlLabel,
  FormLabel,
  Radio,
  RadioGroup,
  Card,
  CardContent
} from '@mui/material';
import {
  Person as PersonIcon,
  Edit as EditIcon,
  Save as SaveIcon,
  ColorLens as ColorLensIcon,
  Language as LanguageIcon,
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
        <Box sx={{ p: 3 }}>
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
  const [loading, setLoading] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  const [formData, setFormData] = useState({
    name: user?.name || '',
    email: user?.email || '',
    language: 'pt-BR',
    timezone: 'America/Sao_Paulo',
  });

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | { name?: string; value: unknown }>) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name as string]: value,
    });
  };

  const handleToggleEditMode = () => {
    if (isEditing) {
      handleUpdateProfile();
    } else {
      setIsEditing(true);
    }
  };

  const handleUpdateProfile = async () => {
    setLoading(true);
    setSuccessMessage('');
    setErrorMessage('');

    try {
      // Aqui deve ser implementada a chamada real à API para atualizar o perfil
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      setSuccessMessage('Perfil atualizado com sucesso!');
      setIsEditing(false);
    } catch (error) {
      setErrorMessage('Erro ao atualizar perfil. Tente novamente.');
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
            <Typography variant="h5">{user?.name || ''}</Typography>
            <Typography variant="body2" color="text.secondary">
              {user?.email || ''}
            </Typography>
            {!user?.name && !user?.email && (
              <Typography variant="body2" color="error">
                Não foi possível carregar os dados do usuário
              </Typography>
            )}
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
            <Tab label="Preferências" icon={<ColorLensIcon />} iconPosition="start" {...a11yProps(1)} />
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

          <Box component="form" onSubmit={(e) => { e.preventDefault(); handleUpdateProfile(); }}>
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

                  <FormControl fullWidth sx={{ mt: 2 }}>
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
      </Paper>
    </Container>
  );
};

export default ProfilePage;
