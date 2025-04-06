import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import CssBaseline from '@mui/material/CssBaseline';
import { useAuth } from './hooks/useAuth';
import { UserProvider } from './contexts/UserContext';

// Layouts
import MainLayout from './layouts/MainLayout';
import AuthLayout from './layouts/AuthLayout';

// Pages
import LoginPage from './pages/auth/LoginPage';
import RegisterPage from './pages/auth/RegisterPage';
import DashboardPage from './pages/dashboard/DashboardPage';
import ProfilePage from './pages/profile/ProfilePage';
import ContentPage from './pages/content/ContentPage';
import SchedulePage from './pages/schedule/SchedulePage';
import AnalyticsPage from './pages/analytics/AnalyticsPage';
import SettingsPage from './pages/settings/SettingsPage';
import NotFoundPage from './pages/NotFoundPage';
import SocialAccountsPage from './pages/social/SocialAccountsPage';
import RecommendationsPage from './pages/recommendations/RecommendationsPage';

// Rotas públicas (acessíveis sem autenticação)
const PublicRoutes = () => {
  return (
    <Routes>
      <Route path="/" element={<AuthLayout />}>
        <Route index element={<Navigate to="/login" replace />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="register" element={<RegisterPage />} />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Route>
    </Routes>
  );
};

// Rotas privadas (requerem autenticação)
const PrivateRoutes = () => {
  return (
    <Routes>
      <Route path="/" element={<MainLayout />}>
        <Route index element={<DashboardPage />} />
        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="profile" element={<ProfilePage />} />
        <Route path="content" element={<ContentPage />} />
        <Route path="schedule" element={<SchedulePage />} />
        <Route path="analytics" element={<AnalyticsPage />} />
        <Route path="social" element={<SocialAccountsPage />} />
        <Route path="recommendations" element={<RecommendationsPage />} />
        <Route path="settings" element={<SettingsPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
};

const App: React.FC = () => {
  const { isAuthenticated, isInitialized } = useAuth();
  
  if (!isInitialized) {
    return <div>Carregando Aplicação...</div>;
  }

  return (
    <>
      <CssBaseline />
      <UserProvider>
        {isAuthenticated ? <PrivateRoutes /> : <PublicRoutes />}
      </UserProvider>
    </>
  );
};

export default App; 