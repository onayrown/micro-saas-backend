/**
 * Script para popular o MongoDB com dados de teste para o MicroSaaS
 * 
 * Inclui usuários, criadores, posts, contas sociais, métricas, recomendações, etc.
 * Última revisão: 14/04/2025 - Atualizado para usar formato GUID compatível com .NET
 */

// --- Conexão e Definições Iniciais ---
const dbName = 'microsaas';
db = db.getSiblingDB(dbName);
print(`Conectado ao banco de dados: ${dbName}`);

// --- Função UUID (RFC4122 v4) compatível com .NET Guid ---
function UUID() {
  // Implementação RFC4122 v4 mais rigorosa que garante compatibilidade com .NET Guid
  const bytes = new Array(16);
  for (let i = 0; i < 16; i++) {
    bytes[i] = Math.floor(Math.random() * 256);
  }
  
  // Ajustar bits conforme especificação v4 (RFC4122)
  bytes[6] = (bytes[6] & 0x0f) | 0x40;  // version 4
  bytes[8] = (bytes[8] & 0x3f) | 0x80;  // variant RFC4122
  
  // Converter para representação hexadecimal com hífens no formato GUID padrão
  const hex = bytes.map(b => b.toString(16).padStart(2, '0')).join('');
  return `${hex.substring(0, 8)}-${hex.substring(8, 12)}-${hex.substring(12, 16)}-${hex.substring(16, 20)}-${hex.substring(20)}`;
}

// --- LIMPEZA INICIAL DAS COLEÇÕES ---
print("Limpando coleções existentes...");
try {
    // Essenciais
    db.Users.deleteMany({});
    db.ContentCreators.deleteMany({});
    db.PerformanceMetrics.deleteMany({});
    
    // Adicionais do script de backup
    db.ContentPosts.deleteMany({});
    db.SocialMediaAccounts.deleteMany({}); // Nome da coleção no script backup
    // db.ContentPerformances.deleteMany({}); // Esta parece redundante com PerformanceMetrics?
    db.ContentRecommendations.deleteMany({});
    db.Analytics.deleteMany({});
    // db.DashboardInsights.deleteMany({}); // Não estava sendo populada no backup
    // db.ContentChecklists.deleteMany({}); // Não estava sendo populada no backup
    // db.Schedules.deleteMany({}); // Não estava sendo populada no backup

    print("Coleções limpas com sucesso.");
} catch (e) {
    print(`Erro ao limpar coleções: ${e}`);
    // quit(1); 
}


// --- Criação de Usuários ---
print("Criando usuários...");
// Usar hash gerado por Microsoft.AspNetCore.Identity.PasswordHasher para "senha123"
const correctPasswordHash = "AQAAAAIAAYagAAAAEF1Uc7TzD1/o90UqYQF1jLf6WCWMjKzbossdbH9eXtelmNpcpCjKn1gFlfdRmvYuXw=="; 
let usersToInsert = [
    // Adicionar role, active, createdAt, updatedAt como no backup
    { _id: UUID(), name: "João Silva", email: "joao@example.com", username: "joaosilva", passwordHash: correctPasswordHash, role: 'user', active: true, createdAt: new Date(), updatedAt: new Date() }, 
    { _id: UUID(), name: "Maria Souza", email: "maria@example.com", username: "mariasouza", passwordHash: correctPasswordHash, role: 'admin', active: true, createdAt: new Date(), updatedAt: new Date() },
    { _id: UUID(), name: "Pedro Oliveira", email: "pedro@example.com", username: "pedrooliveira", passwordHash: correctPasswordHash, role: 'user', active: true, createdAt: new Date(), updatedAt: new Date() },
    { _id: "f0b4f3a1-0b0a-4e6a-8b0a-000000000001", name: "Felipe Teste", email: "felipe@example.com", username: "felipe", passwordHash: correctPasswordHash, role: 'user', active: true, createdAt: new Date(), updatedAt: new Date() } 
];
let insertedUserIds = [];
try {
    const userResult = db.Users.insertMany(usersToInsert);
    insertedUserIds = userResult.insertedIds || []; // Armazena os IDs inseridos
    print(`-> Inseridos ${insertedUserIds.length} usuários.`);
} catch(e) { 
    print(`ERRO FATAL ao inserir usuários: ${e}`); 
    // quit(1); 
}

// --- Criação/Atualização de Criadores de Conteúdo ---
print("Criando/Atualizando Criadores de Conteúdo...");
// Mapear usuários inseridos para facilitar a busca pelos IDs corretos
const usersById = {};
db.Users.find().forEach(user => { usersById[user._id.toString()] = user; });

const creatorsToEnsure = [
    // Dados do script de backup, usando IDs dos usuários recém-criados
    { userId: usersToInsert[0]._id, name: 'João Creator', bio: 'Criador de conteúdo sobre tecnologia e programação', categories: ['Tecnologia', 'Programação'], followers: 15000, niche: 'Tecnologia', platforms: ['YouTube', 'Instagram'], email: usersToInsert[0].email, username: usersToInsert[0].username },
    { userId: usersToInsert[1]._id, name: 'Maria Fitness', bio: 'Especialista em fitness e alimentação saudável', categories: ['Fitness', 'Saúde'], followers: 50000, niche: 'Saúde', platforms: ['Instagram'], email: usersToInsert[1].email, username: usersToInsert[1].username },
    { userId: usersToInsert[2]._id, name: 'Pedro Games', bio: 'Conteúdo sobre jogos', categories: ['Games', 'Tecnologia'], followers: 25000, niche: 'Games', platforms: ['Twitch'], email: usersToInsert[2].email, username: usersToInsert[2].username },
    // Felipe (tratamento especial com ID fixo)
    { userId: "f0b4f3a1-0b0a-4e6a-8b0a-000000000001", name: "Felipe Teste", email: "felipe@example.com", username: "felipe", bio: "Perfil de teste para Felipe", categories: ['Testes'], followers: 0, niche: 'Testes', platforms: ['App'] }
];

let creatorOps = creatorsToEnsure.map(creatorData => {
    const creatorDoc = {
        // Usar o userId como _id para ContentCreator
        // _id: creatorData.userId, 
        userId: creatorData.userId,
        name: creatorData.name,
        email: creatorData.email, // Adicionado
        username: creatorData.username, // Adicionado
        bio: creatorData.bio,
        profileImageUrl: null,
        websiteUrl: null,
        isActive: true,
        niche: creatorData.niche,
        // contentType: "Vídeos", // Remover se não for padrão
        categories: creatorData.categories || [], // Usar do array ou default
        platforms: creatorData.platforms || [], // Usar do array ou default
        subscriptionPlan: null,
        socialMediaAccounts: [], // Populado depois
        posts: [], // Populado depois
        createdAt: new Date(),
        updatedAt: new Date(),
        totalFollowers: creatorData.followers || 0, // Usar do array ou default
        totalPosts: 0,
        averageEngagementRate: 0,
        totalRevenue: 0
    };
    return {
        replaceOne: {
            filter: { _id: creatorData.userId }, // Filtra pelo ID do usuário
            replacement: creatorDoc,
            upsert: true
        }
    };
});

let creatorsResult = null;
try {
    if (creatorOps.length > 0) {
        creatorsResult = db.ContentCreators.bulkWrite(creatorOps, { ordered: false });
        print(`-> Operações BulkWrite para Criadores concluídas: ${creatorsResult.upsertedCount} inseridos, ${creatorsResult.modifiedCount} atualizados, ${creatorsResult.matchedCount} encontrados.`);
    } else {
        print("Nenhuma operação de criador para executar.");
    }
} catch (e) {
    print(`ERRO ao processar Criadores de Conteúdo: ${e}`);
    if (e.writeErrors) {
        e.writeErrors.forEach(err => print(`  - Erro no índice ${err.index}: ${err.errmsg}`));
    }
}

// Obter referência aos IDs dos criadores (que são os mesmos dos usuários)
const creatorIds = usersToInsert.map(u => u._id);
const joaoCreatorId = creatorIds[0];
const mariaCreatorId = creatorIds[1];
const pedroCreatorId = creatorIds[2];
const felipeCreatorId = creatorIds[3]; // ID fixo

// --- Criação de Contas Sociais (SocialMediaAccounts) ---
print("Criando Contas Sociais...");
const socialAccountsToInsert = [
  // Dados do script de backup, usando os IDs corretos
  { _id: UUID(), creatorId: joaoCreatorId, platform: 'Instagram', username: 'joao.tech', followers: 10000, following: 500, engagementRate: 3.5, connected: true, createdAt: new Date(), updatedAt: new Date() },
  { _id: UUID(), creatorId: joaoCreatorId, platform: 'YouTube', username: 'JoãoTechChannel', followers: 5000, following: 100, engagementRate: 4.2, connected: true, createdAt: new Date(), updatedAt: new Date() },
  { _id: UUID(), creatorId: mariaCreatorId, platform: 'Instagram', username: 'maria.fitness', followers: 45000, following: 800, engagementRate: 5.1, connected: true, createdAt: new Date(), updatedAt: new Date() },
  { _id: UUID(), creatorId: pedroCreatorId, platform: 'YouTube', username: 'PedroGamesChannel', followers: 20000, following: 150, engagementRate: 4.8, connected: true, createdAt: new Date(), updatedAt: new Date() }
];
try {
    if (socialAccountsToInsert.length > 0) {
        const socialResult = db.SocialMediaAccounts.insertMany(socialAccountsToInsert);
        print(`-> Inseridas ${socialResult.insertedIds.length} contas sociais.`);
    } else { print("Nenhuma conta social para inserir."); }
} catch (e) { print(`ERRO ao inserir Contas Sociais: ${e}`); }


// --- Criação de Posts (ContentPosts) ---
print("Criando Posts de Conteúdo...");
const contentPostsToInsert = [
  // Dados do script de backup, usando os IDs corretos
  { _id: UUID(), creatorId: joaoCreatorId, title: 'Iniciando com React em 2024', description: 'Guia completo para iniciantes em React', contentType: 'video', status: 'published', mediaUrls: ['https://example.com/videos/react2024.mp4'], platforms: ['YouTube'], tags: ['React', 'JavaScript'], categories: ['Programação'], engagementMetrics: { views: 1500, likes: 120, comments: 45, shares: 30 }, publishedAt: new Date(new Date().setDate(new Date().getDate() - 10)), createdAt: new Date(new Date().setDate(new Date().getDate() - 15)), updatedAt: new Date(new Date().setDate(new Date().getDate() - 10)) },
  { _id: UUID(), creatorId: joaoCreatorId, title: 'Dicas de TypeScript', description: 'Melhore seu código', contentType: 'post', status: 'draft', mediaUrls: [], platforms: ['Instagram'], tags: ['TypeScript'], categories: ['Programação'], createdAt: new Date(new Date().setDate(new Date().getDate() - 5)), updatedAt: new Date() },
  { _id: UUID(), creatorId: mariaCreatorId, title: 'Treino de 15 minutos', description: 'Treino rápido e eficiente', contentType: 'video', status: 'published', mediaUrls: [], platforms: ['Instagram'], tags: ['Fitness'], categories: ['Fitness'], engagementMetrics: { views: 8500, likes: 750, comments: 120, shares: 80 }, publishedAt: new Date(new Date().setDate(new Date().getDate() - 3)), createdAt: new Date(new Date().setDate(new Date().getDate() - 7)), updatedAt: new Date() },
  { _id: UUID(), creatorId: pedroCreatorId, title: 'Review do novo jogo XYZ', description: 'Análise completa', contentType: 'video', status: 'published', mediaUrls: [], platforms: ['YouTube'], tags: ['Games'], categories: ['Games'], engagementMetrics: { views: 12000, likes: 980, comments: 250, shares: 120 }, publishedAt: new Date(new Date().setDate(new Date().getDate() - 1)), createdAt: new Date(new Date().setDate(new Date().getDate() - 8)), updatedAt: new Date() }
];
try {
    if (contentPostsToInsert.length > 0) {
        const postsResult = db.ContentPosts.insertMany(contentPostsToInsert);
        print(`-> Inseridos ${postsResult.insertedIds.length} posts de conteúdo.`);
    } else { print("Nenhum post para inserir."); }
} catch (e) { print(`ERRO ao inserir Posts: ${e}`); }


// --- Criação de Recomendações (ContentRecommendations) ---
print("Criando Recomendações...");
const recommendationsToInsert = [
  // Dados do script de backup, usando os IDs corretos
  { _id: UUID(), creatorId: joaoCreatorId, title: 'Aumente Engajamento com Vídeos Curtos', description: 'Vídeos curtos têm mais engajamento', score: 95, category: 'Formato', recommendationType: 'FORMAT', implementationDifficulty: 'Fácil', potentialImpact: 'Alto', createdAt: new Date(), updatedAt: new Date() },
  { _id: UUID(), creatorId: mariaCreatorId, title: 'Receitas Rápidas de Fit Food', description: 'Conteúdo sobre receitas fitness tem alto engajamento', score: 92, category: 'Conteúdo', recommendationType: 'TOPIC', implementationDifficulty: 'Fácil', potentialImpact: 'Alto', createdAt: new Date(), updatedAt: new Date() },
  { _id: UUID(), creatorId: pedroCreatorId, title: 'Streaming ao Vivo Semanal', description: 'Lives semanais aumentam engajamento', score: 90, category: 'Formato', recommendationType: 'FORMAT', implementationDifficulty: 'Médio', potentialImpact: 'Alto', createdAt: new Date(), updatedAt: new Date() },
  // Recomendação para Felipe
  { _id: UUID(), creatorId: felipeCreatorId, title: 'Explore Novos Formatos', description: 'Experimente posts interativos ou quizzes', score: 80, category: 'Formato', recommendationType: 'FORMAT', implementationDifficulty: 'Médio', potentialImpact: 'Médio', createdAt: new Date(), updatedAt: new Date() }
];
try {
    if (recommendationsToInsert.length > 0) {
        const recResult = db.ContentRecommendations.insertMany(recommendationsToInsert);
        print(`-> Inseridas ${recResult.insertedIds.length} recomendações.`);
    } else { print("Nenhuma recomendação para inserir."); }
} catch (e) { print(`ERRO ao inserir Recomendações: ${e}`); }


// --- Criação de Dados de Analytics --- 
// NOTE: O script de backup usava uma coleção 'Analytics' com dados diários.
// A implementação atual parece usar 'PerformanceMetrics' para dados agregados por período.
// Vamos manter a inserção em 'PerformanceMetrics' como está, pois é o que o dashboard usa.
// Se a coleção 'Analytics' for realmente necessária com dados diários, precisaria ser recriada.
print("Verificando/Inserindo PerformanceMetrics...");

// Função para gerar métricas (adaptada do backup, mas focada em PerformanceMetrics)
function generatePerformanceMetrics(creatorId, baseDate, platform, followers, growth, views, likes, comments, shares, revenue) {
    return {
        _id: UUID(),
        creatorId: creatorId,
        platform: platform,
        date: baseDate, // A data representa o período (e.g., fim do mês)
        followers: followers,
        followersGrowth: growth,
        totalViews: views,
        totalLikes: likes,
        totalComments: comments,
        totalShares: shares,
        engagementRate: parseFloat(((likes + comments + shares) / followers * 100).toFixed(2)) || 0,
        estimatedRevenue: parseFloat(revenue.toFixed(2)),
        topPerformingContentIds: [],
        createdAt: new Date(),
        updatedAt: new Date()
    };
}

const today = new Date();
const oneMonthAgo = new Date(new Date().setMonth(today.getMonth() - 1));
const twoMonthsAgo = new Date(new Date().setMonth(today.getMonth() - 2));

const performanceMetricsToInsert = [
    // João
    generatePerformanceMetrics(joaoCreatorId, twoMonthsAgo, "Instagram", 8000, 500, 80000, 4000, 500, 200, 250.00),
    generatePerformanceMetrics(joaoCreatorId, oneMonthAgo, "Instagram", 10000, 2000, 120000, 6500, 800, 350, 380.50),
    generatePerformanceMetrics(joaoCreatorId, today, "Instagram", 15000, 5000, 150000, 9000, 1100, 450, 520.75),
    generatePerformanceMetrics(joaoCreatorId, oneMonthAgo, "YouTube", 4000, 1000, 200000, 9000, 1200, 500, 600.00),
    generatePerformanceMetrics(joaoCreatorId, today, "YouTube", 5000, 1000, 250000, 11000, 1500, 600, 750.25),
    // Maria
    generatePerformanceMetrics(mariaCreatorId, oneMonthAgo, "Instagram", 40000, 5000, 500000, 45000, 8000, 3000, 1200.00),
    generatePerformanceMetrics(mariaCreatorId, today, "Instagram", 50000, 10000, 650000, 58000, 10000, 4000, 1550.80),
    // Pedro
    generatePerformanceMetrics(pedroCreatorId, oneMonthAgo, "YouTube", 15000, 2000, 300000, 15000, 3000, 1000, 900.00),
    generatePerformanceMetrics(pedroCreatorId, today, "YouTube", 25000, 10000, 450000, 28000, 5500, 1800, 1350.50),
    // Felipe (já inserido antes, mas podemos adicionar/atualizar se necessário)
    // Se a limpeza inicial removeu, precisamos reinserir.
    generatePerformanceMetrics(felipeCreatorId, twoMonthsAgo, "Instagram", 1200, 100, 40000, 2000, 250, 80, 120.50),
    generatePerformanceMetrics(felipeCreatorId, twoMonthsAgo, "YouTube",   600,  40,  80000, 3500, 400, 150, 300.00),
    generatePerformanceMetrics(felipeCreatorId, oneMonthAgo,  "Instagram", 1500, 300, 55000, 2800, 350, 110, 175.75),
    generatePerformanceMetrics(felipeCreatorId, oneMonthAgo,  "YouTube",   800,  200, 110000, 4800, 600, 220, 380.20),
    generatePerformanceMetrics(felipeCreatorId, today,        "Instagram", 1850, 350, 70000, 4000, 500, 180, 240.00),
    generatePerformanceMetrics(felipeCreatorId, today,        "YouTube",   1050, 250, 150000, 6500, 800, 300, 450.90)
];

try {
    if (performanceMetricsToInsert.length > 0) {
        // Usar bulkWrite com upsert pode ser mais seguro se quisermos atualizar em vez de duplicar
        const metricsOps = performanceMetricsToInsert.map(metric => ({
            updateOne: {
                filter: { creatorId: metric.creatorId, platform: metric.platform, date: metric.date },
                update: { $set: metric },
                upsert: true
            }
        }));
        const metricsResult = db.PerformanceMetrics.bulkWrite(metricsOps, { ordered: false });
        print(`-> Operações BulkWrite para Métricas concluídas: ${metricsResult.upsertedCount} inseridas, ${metricsResult.modifiedCount} atualizadas, ${metricsResult.matchedCount} encontradas.`);
    } else { print("Nenhuma métrica de performance para inserir/atualizar."); }
} catch (e) {
    print(`ERRO ao inserir/atualizar PerformanceMetrics: ${e}`);
    if (e.writeErrors) {
        e.writeErrors.forEach(err => print(`  - Erro no índice ${err.index}: ${err.errmsg}`));
    }
}


// --- Finalização ---
print("-----------------------------------------------------");
print("Banco de dados populado (ou atualizado) com sucesso!");
print("Usuários configurados (senha para todos: senha123):");
usersToInsert.forEach(u => print(`- ${u.email} (_id: ${u._id})`));
print("-----------------------------------------------------"); 