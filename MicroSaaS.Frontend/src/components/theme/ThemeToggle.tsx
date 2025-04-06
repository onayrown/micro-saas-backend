import React from 'react';
import { IconButton, Tooltip } from '@mui/material';
import { useTheme } from '../../hooks/useTheme';
import Brightness4Icon from '@mui/icons-material/Brightness4'; // Ícone de lua
import Brightness7Icon from '@mui/icons-material/Brightness7'; // Ícone de sol

const ThemeToggle: React.FC = () => {
  const { mode, toggleTheme } = useTheme();

  const getThemeIcon = () => {
    if (mode === 'dark') {
      return <Brightness7Icon />;
    }
    return <Brightness4Icon />;
  };

  const getTooltipText = () => {
    return mode === 'dark' ? 'Mudar para modo claro' : 'Mudar para modo escuro';
  };

  return (
    <Tooltip title={getTooltipText()}>
      <IconButton color="inherit" onClick={toggleTheme} aria-label="toggle theme">
        {getThemeIcon()}
      </IconButton>
    </Tooltip>
  );
};

export default ThemeToggle; 