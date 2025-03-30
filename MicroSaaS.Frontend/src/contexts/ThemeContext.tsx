import React, { createContext, useState, useEffect, ReactNode } from 'react';

type ThemeMode = 'light' | 'dark' | 'auto';

interface ThemeContextType {
  themeMode: ThemeMode;
  toggleTheme: () => void;
  setThemeMode: (mode: ThemeMode) => void;
  isDarkMode: boolean;
}

export const ThemeContext = createContext<ThemeContextType>({
  themeMode: 'light',
  toggleTheme: () => {},
  setThemeMode: () => {},
  isDarkMode: false,
});

interface ThemeProviderProps {
  children: ReactNode;
}

export const ThemeProvider: React.FC<ThemeProviderProps> = ({ children }) => {
  // Tenta obter preferência salva, ou usa 'light' como padrão
  const [themeMode, setThemeMode] = useState<ThemeMode>(
    (localStorage.getItem('themeMode') as ThemeMode) || 'light'
  );
  
  // Estado separado para o modo escuro efetivo (considerando configuração 'auto')
  const [isDarkMode, setIsDarkMode] = useState<boolean>(themeMode === 'dark');
  
  // Detecta preferência do sistema para tema escuro
  const prefersDarkMode = window.matchMedia && 
    window.matchMedia('(prefers-color-scheme: dark)').matches;

  // Salvar preferência no localStorage quando mudar
  useEffect(() => {
    localStorage.setItem('themeMode', themeMode);
    
    // Se estiver no modo automático, use a preferência do sistema
    if (themeMode === 'auto') {
      setIsDarkMode(prefersDarkMode);
      
      // Adiciona listener para mudar tema quando a preferência do sistema mudar
      const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
      const handleChange = (e: MediaQueryListEvent) => {
        setIsDarkMode(e.matches);
      };
      
      mediaQuery.addEventListener('change', handleChange);
      return () => mediaQuery.removeEventListener('change', handleChange);
    } else {
      // Se não é automático, usar diretamente o themeMode
      setIsDarkMode(themeMode === 'dark');
    }
  }, [themeMode, prefersDarkMode]);

  // Alternar entre claro e escuro (ignora 'auto')
  const toggleTheme = () => {
    setThemeMode(prevMode => {
      if (prevMode === 'light') return 'dark';
      if (prevMode === 'dark') return 'light';
      // Se estiver no modo 'auto', muda para o oposto da preferência atual
      return prefersDarkMode ? 'light' : 'dark';
    });
  };

  return (
    <ThemeContext.Provider value={{ themeMode, toggleTheme, setThemeMode, isDarkMode }}>
      {children}
    </ThemeContext.Provider>
  );
}; 