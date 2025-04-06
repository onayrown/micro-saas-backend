/**
 * Script para limpar todas as coleções do MongoDB no MicroSaaS
 * 
 * Este script remove todos os dados de todas as coleções no MongoDB,
 * deixando o banco de dados vazio para novos testes.
 * 
 * Para executar:
 * 1. Certifique-se de que o MongoDB está rodando
 * 2. Execute este script usando o shell do MongoDB
 */

// Conectar ao banco
db = db.getSiblingDB('microsaas');

print('Iniciando limpeza das coleções...');

// Lista de todas as coleções padronizadas
const colecoes = [
  'Users',
  'ContentCreators',
  'ContentPosts',
  'SocialMediaAccounts',
  'ContentPerformances',
  'ContentRecommendations',
  'ContentChecklists',
  'Analytics',
  'DashboardInsights',
  'Schedules'
];

// Limpar cada coleção
colecoes.forEach(colecao => {
  try {
    db[colecao].drop();
    print(`Coleção '${colecao}' removida com sucesso.`);
  } catch (error) {
    print(`Aviso: Não foi possível remover a coleção '${colecao}': ${error.message}`);
  }
});

print('Limpeza concluída!');
print('Todas as coleções foram removidas do banco de dados.'); 