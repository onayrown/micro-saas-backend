import React, { useState, SyntheticEvent } from 'react';
import {
  Container,
  Box,
  Typography,
  Paper,
  TextField,
  Button,
  Grid,
  Tab,
  Tabs,
  FormControl,
  FormControlLabel,
  Switch,
  MenuItem,
  Select,
  InputLabel,
  Divider,
  Alert,
  AlertTitle,
  FormHelperText,
  SelectChangeEvent
} from '@mui/material';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

const TabPanel = (props: TabPanelProps) => {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`settings-tabpanel-${index}`}
      aria-labelledby={`settings-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
};

const SettingsPage: React.FC = () => {
  const [tabValue, setTabValue] = useState(0);
  const [name, setName] = useState('Usuário Teste');
  const [email, setEmail] = useState('usuario@teste.com');
  const [language, setLanguage] = useState('pt-BR');
  const [timezone, setTimezone] = useState('America/Sao_Paulo');
  
  const [appNotifications, setAppNotifications] = useState(true);
  const [emailNotifications, setEmailNotifications] = useState(true);
  const [contentNotifications, setContentNotifications] = useState(true);
  const [commentNotifications, setCommentNotifications] = useState(true);
  const [analyticsNotifications, setAnalyticsNotifications] = useState(false);

  const [saving, setSaving] = useState(false);
  const [saveSuccess, setSaveSuccess] = useState(false);

  const handleTabChange = (event: SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const handleLanguageChange = (event: SelectChangeEvent) => {
    setLanguage(event.target.value);
  };

  const handleTimezoneChange = (event: SelectChangeEvent) => {
    setTimezone(event.target.value);
  };

  const handleSaveGeneral = async () => {
    setSaving(true);
    setSaveSuccess(false);
    
    // Simulação de chamada de API
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    setSaving(false);
    setSaveSuccess(true);
    
    // Limpar mensagem de sucesso após 3 segundos
    setTimeout(() => {
      setSaveSuccess(false);
    }, 3000);
  };

  const handleSaveNotifications = async () => {
    setSaving(true);
    setSaveSuccess(false);
    
    // Simulação de chamada de API
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    setSaving(false);
    setSaveSuccess(true);
    
    // Limpar mensagem de sucesso após 3 segundos
    setTimeout(() => {
      setSaveSuccess(false);
    }, 3000);
  };

  return (
    <Container maxWidth="lg">
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" gutterBottom>
          Configurações
        </Typography>
      </Box>

      <Paper sx={{ width: '100%' }}>
        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tabs
            value={tabValue}
            onChange={handleTabChange}
            aria-label="configurações tabs"
            variant="scrollable"
            scrollButtons="auto"
          >
            <Tab label="Geral" id="settings-tab-0" aria-controls="settings-tabpanel-0" />
            <Tab label="Notificações" id="settings-tab-1" aria-controls="settings-tabpanel-1" />
            <Tab label="Segurança" id="settings-tab-2" aria-controls="settings-tabpanel-2" />
            <Tab label="Integrações" id="settings-tab-3" aria-controls="settings-tabpanel-3" />
          </Tabs>
        </Box>

        {/* Tab: Configurações Gerais */}
        <TabPanel value={tabValue} index={0}>
          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                label="Nome"
                value={name}
                onChange={(e) => setName(e.target.value)}
                margin="normal"
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                label="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                margin="normal"
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <FormControl fullWidth margin="normal">
                <InputLabel id="language-label">Idioma</InputLabel>
                <Select
                  labelId="language-label"
                  value={language}
                  label="Idioma"
                  onChange={handleLanguageChange}
                >
                  <MenuItem value="pt-BR">Português (Brasil)</MenuItem>
                  <MenuItem value="en-US">English (United States)</MenuItem>
                  <MenuItem value="es">Español</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} md={6}>
              <FormControl fullWidth margin="normal">
                <InputLabel id="timezone-label">Fuso Horário</InputLabel>
                <Select
                  labelId="timezone-label"
                  value={timezone}
                  label="Fuso Horário"
                  onChange={handleTimezoneChange}
                >
                  <MenuItem value="America/Sao_Paulo">Brasília (GMT-3)</MenuItem>
                  <MenuItem value="America/New_York">New York (GMT-5)</MenuItem>
                  <MenuItem value="Europe/London">London (GMT+0)</MenuItem>
                  <MenuItem value="Europe/Paris">Paris (GMT+1)</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12}>
              <Box sx={{ mt: 2, display: 'flex', justifyContent: 'flex-end' }}>
                <Button
                  variant="contained"
                  onClick={handleSaveGeneral}
                  disabled={saving}
                >
                  {saving ? 'Salvando...' : 'Salvar Alterações'}
                </Button>
              </Box>
              {saveSuccess && (
                <Alert severity="success" sx={{ mt: 2 }}>
                  Configurações salvas com sucesso!
                </Alert>
              )}
            </Grid>
          </Grid>
        </TabPanel>

        {/* Tab: Notificações */}
        <TabPanel value={tabValue} index={1}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <Typography variant="h6" gutterBottom>
                Canais de Notificação
              </Typography>
              <FormControlLabel
                control={
                  <Switch
                    checked={appNotifications}
                    onChange={(e) => setAppNotifications(e.target.checked)}
                  />
                }
                label="Notificações no aplicativo"
              />
              <FormHelperText>
                Receba notificações dentro da plataforma
              </FormHelperText>
              
              <Box sx={{ mt: 2 }}>
                <FormControlLabel
                  control={
                    <Switch
                      checked={emailNotifications}
                      onChange={(e) => setEmailNotifications(e.target.checked)}
                    />
                  }
                  label="Notificações por e-mail"
                />
                <FormHelperText>
                  Receba notificações via e-mail
                </FormHelperText>
              </Box>
            </Grid>
            
            <Grid item xs={12}>
              <Divider sx={{ my: 2 }} />
              <Typography variant="h6" gutterBottom>
                Tipos de Notificação
              </Typography>
              
              <FormControlLabel
                control={
                  <Switch
                    checked={contentNotifications}
                    onChange={(e) => setContentNotifications(e.target.checked)}
                  />
                }
                label="Conteúdo"
              />
              <FormHelperText>
                Notificações sobre publicações agendadas, conteúdo publicado, etc.
              </FormHelperText>
              
              <Box sx={{ mt: 2 }}>
                <FormControlLabel
                  control={
                    <Switch
                      checked={commentNotifications}
                      onChange={(e) => setCommentNotifications(e.target.checked)}
                    />
                  }
                  label="Comentários"
                />
                <FormHelperText>
                  Notificações sobre comentários nas suas publicações
                </FormHelperText>
              </Box>
              
              <Box sx={{ mt: 2 }}>
                <FormControlLabel
                  control={
                    <Switch
                      checked={analyticsNotifications}
                      onChange={(e) => setAnalyticsNotifications(e.target.checked)}
                    />
                  }
                  label="Análises"
                />
                <FormHelperText>
                  Notificações sobre relatórios de desempenho e métricas
                </FormHelperText>
              </Box>
            </Grid>
            
            <Grid item xs={12}>
              <Box sx={{ mt: 2, display: 'flex', justifyContent: 'flex-end' }}>
                <Button
                  variant="contained"
                  onClick={handleSaveNotifications}
                  disabled={saving}
                >
                  {saving ? 'Salvando...' : 'Salvar Preferências'}
                </Button>
              </Box>
              {saveSuccess && (
                <Alert severity="success" sx={{ mt: 2 }}>
                  Preferências de notificação salvas com sucesso!
                </Alert>
              )}
            </Grid>
          </Grid>
        </TabPanel>

        {/* Tab: Segurança */}
        <TabPanel value={tabValue} index={2}>
          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <Typography variant="h6" gutterBottom>
                Alterar Senha
              </Typography>
              <TextField
                fullWidth
                type="password"
                label="Senha Atual"
                margin="normal"
              />
              <TextField
                fullWidth
                type="password"
                label="Nova Senha"
                margin="normal"
              />
              <TextField
                fullWidth
                type="password"
                label="Confirmar Nova Senha"
                margin="normal"
              />
              <Box sx={{ mt: 2 }}>
                <Button variant="contained">
                  Alterar Senha
                </Button>
              </Box>
            </Grid>
            
            <Grid item xs={12} md={6}>
              <Typography variant="h6" gutterBottom>
                Autenticação de Dois Fatores
              </Typography>
              <FormControlLabel
                control={<Switch />}
                label="Ativar autenticação de dois fatores"
              />
              <FormHelperText>
                Aumenta a segurança da sua conta exigindo um código além da senha
              </FormHelperText>
              
              <Box sx={{ mt: 4 }}>
                <Typography variant="h6" gutterBottom>
                  Sessões Ativas
                </Typography>
                <Typography variant="body2">
                  Você tem 1 sessão ativa no momento.
                </Typography>
                <Button variant="outlined" color="error" sx={{ mt: 1 }}>
                  Encerrar Todas as Sessões
                </Button>
              </Box>
            </Grid>
          </Grid>
        </TabPanel>

        {/* Tab: Integrações */}
        <TabPanel value={tabValue} index={3}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <Typography variant="h6" gutterBottom>
                Redes Sociais
              </Typography>
              
              <Box sx={{ mb: 2 }}>
                <Button variant="outlined" sx={{ mr: 2 }}>
                  Conectar Instagram
                </Button>
                <Button variant="outlined" sx={{ mr: 2 }}>
                  Conectar Facebook
                </Button>
                <Button variant="outlined">
                  Conectar Twitter
                </Button>
              </Box>
              
              <Divider sx={{ my: 3 }} />
              
              <Typography variant="h6" gutterBottom>
                Ferramentas de Análise
              </Typography>
              
              <Box sx={{ mb: 2 }}>
                <Button variant="outlined" sx={{ mr: 2 }}>
                  Conectar Google Analytics
                </Button>
                <Button variant="outlined">
                  Conectar Facebook Insights
                </Button>
              </Box>
            </Grid>
          </Grid>
        </TabPanel>
      </Paper>
    </Container>
  );
};

export default SettingsPage; 