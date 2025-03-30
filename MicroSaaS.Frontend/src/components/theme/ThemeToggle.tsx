import React from 'react';
import { IconButton, Tooltip } from '@mui/material';
import { useTheme } from '../../hooks/useTheme';
import Brightness4Icon from '@mui/icons-material/Brightness4'; // Ícone de lua
import Brightness7Icon from '@mui/icons-material/Brightness7'; // Ícone de sol
import AutoAwesomeIcon from '@mui/icons-material/AutoAwesome'; // Ícone para modo automático

const ThemeToggle: React.FC = () => {
  const { themeMode, toggleTheme, isDarkMode } = useTheme();

  const getThemeIcon = () => {
    if (themeMode === 'auto') {
      return <AutoAwesomeIcon />;
    }
    return isDarkMode ? <Brightness7Icon /> : <Brightness4Icon />;
  };

  const getTooltipText = () => {
    if (themeMode === 'auto') {
      return 'Modo automático (baseado no sistema)';
    }
    return isDarkMode ? 'Mudar para tema claro' : 'Mudar para tema escuro';
  };

  return (
    <Tooltip title={getTooltipText()}>
      <IconButton 
        onClick={toggleTheme} 
        color="inherit" 
        aria-label="alternar tema"
        sx={{ ml: 1 }}
      >
        {getThemeIcon()}
      </IconButton>
    </Tooltip>
  );
};

export default ThemeToggle; 