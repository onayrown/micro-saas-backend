/**
 * Formata um valor numérico como moeda (BRL por padrão)
 * @param value - O valor a ser formatado
 * @param locale - A localidade a ser usada (padrão: 'pt-BR')
 * @param currency - A moeda a ser usada (padrão: 'BRL')
 * @returns String formatada como moeda
 */
export const formatCurrency = (
  value: number,
  locale = 'pt-BR',
  currency = 'BRL'
): string => {
  return new Intl.NumberFormat(locale, {
    style: 'currency',
    currency,
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(value);
};

/**
 * Formata um valor numérico com separadores de milhar
 * @param value - O valor a ser formatado
 * @param locale - A localidade a ser usada (padrão: 'pt-BR')
 * @returns String formatada com separadores de milhar
 */
export const formatNumber = (value: number, locale = 'pt-BR'): string => {
  return new Intl.NumberFormat(locale).format(value);
};

/**
 * Formata uma porcentagem
 */
export const formatPercent = (value: number): string => {
  return `${value.toLocaleString('pt-BR', { maximumFractionDigits: 2 })}%`;
};

/**
 * Formata um texto truncando-o se for maior que o tamanho especificado
 */
export const truncateText = (text: string, maxLength = 50): string => {
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength - 3) + '...';
};

/**
 * Converte um valor de string para número
 */
export const parseNumberInput = (value: string): number => {
  if (!value) return 0;
  
  // Remove caracteres não numéricos exceto ponto e vírgula
  const sanitized = value.replace(/[^\d,.]/g, '');
  
  // Converte vírgula para ponto para parsing correto
  const numberStr = sanitized.replace(',', '.');
  
  return parseFloat(numberStr) || 0;
};

/**
 * Formata um valor numérico como formato compacto (ex: 1.2K, 1.5M)
 * @param value - O valor a ser formatado
 * @param locale - A localidade a ser usada (padrão: 'pt-BR')
 * @returns String formatada como número compacto
 */
export const formatCompactNumber = (value: number, locale = 'pt-BR'): string => {
  return new Intl.NumberFormat(locale, {
    notation: 'compact',
    compactDisplay: 'short',
  }).format(value);
};

/**
 * Formata uma data como string no formato local
 * @param date - A data a ser formatada (Date ou string ISO)
 * @param locale - A localidade a ser usada (padrão: 'pt-BR')
 * @returns String formatada como data
 */
export const formatDate = (
  date: Date | string,
  locale = 'pt-BR'
): string => {
  const dateObj = typeof date === 'string' ? new Date(date) : date;
  return dateObj.toLocaleDateString(locale);
};

/**
 * Formata um timestamp como string no formato local incluindo hora
 * @param date - A data a ser formatada (Date ou string ISO)
 * @param locale - A localidade a ser usada (padrão: 'pt-BR')
 * @returns String formatada como data e hora
 */
export const formatDateTime = (
  date: Date | string,
  locale = 'pt-BR'
): string => {
  const dateObj = typeof date === 'string' ? new Date(date) : date;
  return dateObj.toLocaleString(locale);
};

/**
 * Formata um intervalo de tempo relativo (ex: "há 5 minutos", "há 2 dias")
 * @param date - A data a ser formatada (Date ou string ISO)
 * @param locale - A localidade a ser usada (padrão: 'pt-BR')
 * @returns String formatada como tempo relativo
 */
export const formatRelativeTime = (
  date: Date | string,
  locale = 'pt-BR'
): string => {
  const dateObj = typeof date === 'string' ? new Date(date) : date;
  const now = new Date();
  const diffInSeconds = Math.floor((now.getTime() - dateObj.getTime()) / 1000);
  
  if (diffInSeconds < 60) {
    return 'agora mesmo';
  }
  
  const diffInMinutes = Math.floor(diffInSeconds / 60);
  if (diffInMinutes < 60) {
    return `há ${diffInMinutes} ${diffInMinutes === 1 ? 'minuto' : 'minutos'}`;
  }
  
  const diffInHours = Math.floor(diffInMinutes / 60);
  if (diffInHours < 24) {
    return `há ${diffInHours} ${diffInHours === 1 ? 'hora' : 'horas'}`;
  }
  
  const diffInDays = Math.floor(diffInHours / 24);
  if (diffInDays < 30) {
    return `há ${diffInDays} ${diffInDays === 1 ? 'dia' : 'dias'}`;
  }
  
  const diffInMonths = Math.floor(diffInDays / 30);
  if (diffInMonths < 12) {
    return `há ${diffInMonths} ${diffInMonths === 1 ? 'mês' : 'meses'}`;
  }
  
  const diffInYears = Math.floor(diffInMonths / 12);
  return `há ${diffInYears} ${diffInYears === 1 ? 'ano' : 'anos'}`;
};

/**
 * Formata um porcentagem com símbolo
 * @param value - O valor a ser formatado (0-1 ou 0-100)
 * @param decimalPlaces - Número de casas decimais (padrão: 1)
 * @returns String formatada como porcentagem
 */
export const formatPercentage = (
  value: number,
  decimalPlaces = 1
): string => {
  // Normaliza para 0-100 se for 0-1
  const normalizedValue = value <= 1 ? value * 100 : value;
  return `${normalizedValue.toFixed(decimalPlaces)}%`;
}; 