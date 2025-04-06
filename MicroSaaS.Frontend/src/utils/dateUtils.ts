/**
 * Formata uma data para o formato dd/MM/yyyy
 */
export const formatSimpleDate = (date: Date | string): string => {
  const d = new Date(date);
  return d.toLocaleDateString('pt-BR');
};

/**
 * Formata uma data para o formato dd/MM/yyyy HH:mm
 */
export const formatDateTime = (date: Date | string): string => {
  const d = new Date(date);
  return `${d.toLocaleDateString('pt-BR')} ${d.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}`;
};

/**
 * Retorna um objeto Date para o início do dia corrente
 */
export const startOfDay = (date: Date = new Date()): Date => {
  const d = new Date(date);
  d.setHours(0, 0, 0, 0);
  return d;
};

/**
 * Retorna um objeto Date para o final do dia corrente
 */
export const endOfDay = (date: Date = new Date()): Date => {
  const d = new Date(date);
  d.setHours(23, 59, 59, 999);
  return d;
};

/**
 * Retorna um objeto Date para o início de 7 dias atrás
 */
export const last7Days = (): Date => {
  const date = new Date();
  date.setDate(date.getDate() - 7);
  return startOfDay(date);
};

/**
 * Retorna um objeto Date para o início de 30 dias atrás
 */
export const last30Days = (): Date => {
  const date = new Date();
  date.setDate(date.getDate() - 30);
  return startOfDay(date);
};

/**
 * Retorna um objeto Date para o início de 90 dias atrás
 */
export const last90Days = (): Date => {
  const date = new Date();
  date.setDate(date.getDate() - 90);
  return startOfDay(date);
};

/**
 * Formata uma data no formato ISO para o backend
 */
export const formatDateForApi = (date: Date): string => {
  return date.toISOString();
};

/**
 * Verifica se uma data é hoje
 */
export const isToday = (date: Date | string): boolean => {
  const today = new Date();
  const d = new Date(date);
  
  return (
    d.getDate() === today.getDate() &&
    d.getMonth() === today.getMonth() &&
    d.getFullYear() === today.getFullYear()
  );
};

/**
 * Calcula a diferença em dias entre duas datas
 */
export const daysBetween = (date1: Date | string, date2: Date | string): number => {
  const d1 = new Date(date1);
  const d2 = new Date(date2);
  
  // Time difference in milliseconds
  const diffTime = Math.abs(d2.getTime() - d1.getTime());
  
  // Convert to days
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
};

/**
 * Formata uma data para exibição com data e hora
 * Formato: dd/MM/yyyy HH:mm
 */
export const formatDate = (dateString: string): string => {
  const date = new Date(dateString);
  
  // Verifica se a data é válida
  if (isNaN(date.getTime())) {
    return 'Data inválida';
  }
  
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date);
};

/**
 * Formata apenas a data sem a hora
 * Formato: dd/MM/yyyy
 */
export const formatDateOnly = (dateString: string): string => {
  const date = new Date(dateString);
  
  // Verifica se a data é válida
  if (isNaN(date.getTime())) {
    return 'Data inválida';
  }
  
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  }).format(date);
};

/**
 * Formata apenas a hora
 * Formato: HH:mm
 */
export const formatTimeOnly = (dateString: string): string => {
  const date = new Date(dateString);
  
  // Verifica se a data é válida
  if (isNaN(date.getTime())) {
    return 'Hora inválida';
  }
  
  return new Intl.DateTimeFormat('pt-BR', {
    hour: '2-digit',
    minute: '2-digit'
  }).format(date);
};

/**
 * Converte uma data para o formato ISO 8601 para envio ao backend
 */
export const toISOString = (date: Date): string => {
  return date.toISOString();
}; 