import React, { useState, useEffect } from 'react';
import { Outlet, Link, useLocation } from 'react-router-dom';
import {
  AppBar,
  Box,
  CssBaseline,
  Divider,
  Drawer,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Typography,
  Menu,
  MenuItem,
  Avatar,
  Button,
  useTheme,
  useMediaQuery,
  Container
} from '@mui/material';
import {
  Menu as MenuIcon,
  Dashboard,
  Article,
  Schedule,
  Settings,
  Person,
  Logout,
  Notifications,
  TrendingUp,
  Share,
  Lightbulb
} from '@mui/icons-material';
import { useAuth } from '../hooks/useAuth';

const drawerWidth = 240;

const MainLayout: React.FC = () => {
  const [mobileOpen, setMobileOpen] = useState(false);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [notificationsAnchorEl, setNotificationsAnchorEl] = useState<null | HTMLElement>(null);
  const { user, logout } = useAuth();
  const location = useLocation();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

  // Log de navegação para depuração
  useEffect(() => {
    console.log('🧭 Navegação para:', location.pathname);
  }, [location.pathname]);

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const handleProfileMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleProfileMenuClose = () => {
    setAnchorEl(null);
  };

  const handleNotificationsMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setNotificationsAnchorEl(event.currentTarget);
  };

  const handleNotificationsMenuClose = () => {
    setNotificationsAnchorEl(null);
  };

  const handleLogout = () => {
    handleProfileMenuClose();
    logout();
  };

  const menuItems = [
    { text: 'Dashboard', icon: <Dashboard />, path: '/dashboard' },
    { text: 'Conteúdo', icon: <Article />, path: '/content' },
    { text: 'Agendamento', icon: <Schedule />, path: '/schedule' },
    { text: 'Analytics', icon: <TrendingUp />, path: '/analytics' },
    { text: 'Redes Sociais', icon: <Share />, path: '/social' },
    { text: 'Recomendações', icon: <Lightbulb />, path: '/recommendations' },
    { text: 'Configurações', icon: <Settings />, path: '/settings' },
    { text: 'Perfil', icon: <Person />, path: '/profile' },
  ];

  const drawer = (
    <div>
      <Toolbar>
        <Typography variant="h6" noWrap component="div">
          MicroSaaS
        </Typography>
      </Toolbar>
      <Divider />
      <List>
        {menuItems.map((item) => (
          <ListItem key={item.text} disablePadding>
            <ListItemButton
              component={Link}
              to={item.path}
              selected={location.pathname === item.path}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </div>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      <AppBar
        position="fixed"
        sx={{
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          ml: { sm: `${drawerWidth}px` },
        }}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            onClick={handleDrawerToggle}
            sx={{ mr: 2, display: { sm: 'none' } }}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
            {menuItems.find((item) => item.path === location.pathname)?.text || 'MicroSaaS'}
          </Typography>
          <IconButton 
            color="inherit" 
            sx={{ mx: 1 }}
            onClick={handleNotificationsMenuOpen}
          >
            <Notifications />
          </IconButton>
          <Menu
            anchorEl={notificationsAnchorEl}
            open={Boolean(notificationsAnchorEl)}
            onClose={handleNotificationsMenuClose}
            transformOrigin={{ horizontal: 'right', vertical: 'top' }}
            anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
          >
            <MenuItem>
              <ListItemIcon>
                <Notifications fontSize="small" color="primary" />
              </ListItemIcon>
              <ListItemText 
                primary="Nova conexão de rede social" 
                secondary="Sua conta Instagram foi conectada com sucesso" 
              />
            </MenuItem>
            <MenuItem>
              <ListItemIcon>
                <Notifications fontSize="small" color="primary" />
              </ListItemIcon>
              <ListItemText 
                primary="Publicação agendada" 
                secondary="Seu post será publicado em 30 minutos" 
              />
            </MenuItem>
            <MenuItem>
              <ListItemIcon>
                <Notifications fontSize="small" color="success" />
              </ListItemIcon>
              <ListItemText 
                primary="Post publicado com sucesso" 
                secondary="Seu conteúdo foi publicado no Instagram" 
              />
            </MenuItem>
            <Divider />
            <MenuItem>
              <Typography align="center" sx={{ width: '100%' }}>
                Ver todas as notificações
              </Typography>
            </MenuItem>
          </Menu>
          <Button
            onClick={handleProfileMenuOpen}
            color="inherit"
            startIcon={
              <Avatar
                sx={{ width: 32, height: 32 }}
                alt={user?.name || 'Usuário'}
                src="/avatars/default.jpg"
              />
            }
          >
            {!isMobile && (user?.name || 'Usuário')}
          </Button>
          <Menu
            anchorEl={anchorEl}
            open={Boolean(anchorEl)}
            onClose={handleProfileMenuClose}
            transformOrigin={{ horizontal: 'right', vertical: 'top' }}
            anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
          >
            <MenuItem onClick={handleProfileMenuClose} component={Link} to="/profile">
              <ListItemIcon>
                <Person fontSize="small" />
              </ListItemIcon>
              Perfil
            </MenuItem>
            <MenuItem onClick={handleProfileMenuClose} component={Link} to="/settings">
              <ListItemIcon>
                <Settings fontSize="small" />
              </ListItemIcon>
              Configurações
            </MenuItem>
            <MenuItem onClick={handleProfileMenuClose} component={Link} to="/analytics">
              <ListItemIcon>
                <TrendingUp fontSize="small" />
              </ListItemIcon>
              Analytics
            </MenuItem>
            <MenuItem onClick={handleProfileMenuClose} component={Link} to="/social">
              <ListItemIcon>
                <Share fontSize="small" />
              </ListItemIcon>
              Redes Sociais
            </MenuItem>
            <MenuItem onClick={handleProfileMenuClose} component={Link} to="/recommendations">
              <ListItemIcon>
                <Lightbulb fontSize="small" />
              </ListItemIcon>
              Recomendações
            </MenuItem>
            <Divider />
            <MenuItem onClick={handleLogout}>
              <ListItemIcon>
                <Logout fontSize="small" />
              </ListItemIcon>
              Sair
            </MenuItem>
          </Menu>
        </Toolbar>
      </AppBar>
      <Box
        component="nav"
        sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
      >
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{
            keepMounted: true, // Melhor performance em dispositivos móveis
          }}
          sx={{
            display: { xs: 'block', sm: 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          open
        >
          {drawer}
        </Drawer>
      </Box>
      <Box
        component="main"
        sx={{ flexGrow: 1, p: 3, width: { sm: `calc(100% - ${drawerWidth}px)` } }}
      >
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  );
};

export default MainLayout; 