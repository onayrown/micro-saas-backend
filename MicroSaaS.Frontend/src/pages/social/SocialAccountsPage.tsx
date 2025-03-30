import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Box,
  Button,
  Grid,
  Paper,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  ListItemSecondaryAction,
  Avatar,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  TextField,
  MenuItem,
  CircularProgress,
  Chip
} from '@mui/material';
import {
  Instagram as InstagramIcon,
  YouTube as YouTubeIcon,
  Facebook as FacebookIcon,
  Twitter as TwitterIcon,
  LinkedIn as LinkedInIcon,
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon
} from '@mui/icons-material';
import { SocialMediaAccount, SocialMediaPlatform } from '../../types';
import { formatNumber } from '../../utils/formatUtils';

const SocialAccountsPage = () => {
  const [accounts, setAccounts] = useState<SocialMediaAccount[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedAccount, setSelectedAccount] = useState<SocialMediaAccount | null>(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [dialogMode, setDialogMode] = useState<'add' | 'edit'>('add');
  const [formValues, setFormValues] = useState({
    platform: SocialMediaPlatform.Instagram,
    handle: '',
    followers: 0
  });
  const [error, setError] = useState<string | null>(null);

  // Simula carregamento de dados
  useEffect(() => {
    const timer = setTimeout(() => {
      // Dados de exemplo
      setAccounts([
        {
          id: '1',
          creatorId: 'user1',
          platform: SocialMediaPlatform.Instagram,
          handle: '@creador_oficial',
          followers: 125000,
          isVerified: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        },
        {
          id: '2',
          creatorId: 'user1',
          platform: SocialMediaPlatform.YouTube,
          handle: 'Canal Criador',
          followers: 450000,
          isVerified: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        },
        {
          id: '3',
          creatorId: 'user1',
          platform: SocialMediaPlatform.TikTok,
          handle: '@creador',
          followers: 350000,
          isVerified: false,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        }
      ]);
      setLoading(false);
    }, 1500);
    
    return () => clearTimeout(timer);
  }, []);

  const handleOpenAddDialog = () => {
    setDialogMode('add');
    setFormValues({
      platform: SocialMediaPlatform.Instagram,
      handle: '',
      followers: 0
    });
    setOpenDialog(true);
  };

  const handleOpenEditDialog = (account: SocialMediaAccount) => {
    setDialogMode('edit');
    setSelectedAccount(account);
    setFormValues({
      platform: account.platform,
      handle: account.handle,
      followers: account.followers
    });
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setSelectedAccount(null);
    setError(null);
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormValues({
      ...formValues,
      [name]: name === 'followers' ? Number(value) : value
    });
  };

  const handleSelectChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormValues({
      ...formValues,
      platform: e.target.value as SocialMediaPlatform
    });
  };

  const handleSubmit = () => {
    // Validação básica
    if (!formValues.handle) {
      setError('Nome de usuário é obrigatório');
      return;
    }

    if (dialogMode === 'add') {
      // Simulando adição de conta
      const newAccount: SocialMediaAccount = {
        id: Date.now().toString(),
        creatorId: 'user1',
        platform: formValues.platform,
        handle: formValues.handle,
        followers: formValues.followers,
        isVerified: false,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      };
      
      setAccounts([...accounts, newAccount]);
    } else if (dialogMode === 'edit' && selectedAccount) {
      // Simulando edição de conta
      const updatedAccounts = accounts.map(account => 
        account.id === selectedAccount.id 
          ? { ...account, ...formValues, updatedAt: new Date().toISOString() }
          : account
      );
      
      setAccounts(updatedAccounts);
    }
    
    handleCloseDialog();
  };

  const handleDelete = (id: string) => {
    // Simulando exclusão de conta
    setAccounts(accounts.filter(account => account.id !== id));
  };

  const getPlatformIcon = (platform: SocialMediaPlatform) => {
    switch(platform) {
      case SocialMediaPlatform.Instagram:
        return <InstagramIcon style={{ color: '#E1306C' }} />;
      case SocialMediaPlatform.YouTube:
        return <YouTubeIcon style={{ color: '#FF0000' }} />;
      case SocialMediaPlatform.Facebook:
        return <FacebookIcon style={{ color: '#4267B2' }} />;
      case SocialMediaPlatform.Twitter:
        return <TwitterIcon style={{ color: '#1DA1F2' }} />;
      case SocialMediaPlatform.LinkedIn:
        return <LinkedInIcon style={{ color: '#0077B5' }} />;
      default:
        return <Avatar>{platform.charAt(0)}</Avatar>;
    }
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '80vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Contas de Mídia Social</Typography>
        <Button 
          variant="contained" 
          startIcon={<AddIcon />}
          onClick={handleOpenAddDialog}
        >
          Adicionar Conta
        </Button>
      </Box>

      {accounts.length === 0 ? (
        <Paper sx={{ p: 3, textAlign: 'center' }}>
          <Typography variant="h6" color="text.secondary">
            Você ainda não tem contas vinculadas
          </Typography>
          <Button
            variant="outlined"
            startIcon={<AddIcon />}
            sx={{ mt: 2 }}
            onClick={handleOpenAddDialog}
          >
            Adicionar sua primeira conta
          </Button>
        </Paper>
      ) : (
        <Paper>
          <List>
            {accounts.map((account) => (
              <ListItem key={account.id} divider>
                <ListItemAvatar>
                  <Avatar>
                    {getPlatformIcon(account.platform)}
                  </Avatar>
                </ListItemAvatar>
                <ListItemText
                  primary={
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      {account.handle}
                      {account.isVerified && (
                        <Chip
                          label="Verificado"
                          size="small"
                          color="primary"
                          sx={{ ml: 1 }}
                        />
                      )}
                    </Box>
                  }
                  secondary={
                    <React.Fragment>
                      <Typography component="span" variant="body2" color="text.primary">
                        {account.platform}
                      </Typography>
                      {` - ${formatNumber(account.followers)} seguidores`}
                    </React.Fragment>
                  }
                />
                <ListItemSecondaryAction>
                  <IconButton edge="end" aria-label="edit" onClick={() => handleOpenEditDialog(account)}>
                    <EditIcon />
                  </IconButton>
                  <IconButton 
                    edge="end" 
                    aria-label="delete" 
                    sx={{ ml: 1 }}
                    onClick={() => handleDelete(account.id)}
                  >
                    <DeleteIcon />
                  </IconButton>
                </ListItemSecondaryAction>
              </ListItem>
            ))}
          </List>
        </Paper>
      )}

      {/* Diálogo de Adicionar/Editar conta */}
      <Dialog open={openDialog} onClose={handleCloseDialog}>
        <DialogTitle>
          {dialogMode === 'add' ? 'Adicionar nova conta' : 'Editar conta'}
        </DialogTitle>
        <DialogContent>
          {error && (
            <DialogContentText color="error">
              {error}
            </DialogContentText>
          )}
          <TextField
            select
            fullWidth
            margin="normal"
            name="platform"
            label="Plataforma"
            value={formValues.platform}
            onChange={handleSelectChange}
          >
            {Object.values(SocialMediaPlatform).map((platform) => (
              <MenuItem key={platform} value={platform}>
                {platform}
              </MenuItem>
            ))}
          </TextField>
          <TextField
            fullWidth
            margin="normal"
            name="handle"
            label="Nome de usuário"
            value={formValues.handle}
            onChange={handleInputChange}
            placeholder="@usuario ou nome do canal"
          />
          <TextField
            fullWidth
            margin="normal"
            name="followers"
            label="Número de seguidores"
            type="number"
            value={formValues.followers}
            onChange={handleInputChange}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancelar</Button>
          <Button onClick={handleSubmit} variant="contained">
            {dialogMode === 'add' ? 'Adicionar' : 'Salvar'}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default SocialAccountsPage; 