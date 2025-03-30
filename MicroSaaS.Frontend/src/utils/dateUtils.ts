/**
 * Formata uma data para o formato dd/MM/yyyy
 */
export const formatDate = (date: Date | string): string => {
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