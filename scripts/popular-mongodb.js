/**
 * Script para popular o MongoDB com dados de teste para o MicroSaaS
 * 
 * Este script insere dados de exemplo no MongoDB para permitir testes das funcionalidades
 * do frontend sem precisar criar dados manualmente.
 * 
 * Para executar:
 * 1. Certifique-se de que o MongoDB está rodando
 * 2. Execute: mongo < scripts/popular-mongodb.js
 * 
 * (Ou usando MongoDB Shell: mongosh < scripts/popular-mongodb.js)
 */

// Conectar ao banco
db = db.getSiblingDB('microsaas');

print('Limpando coleções existentes...');
db.Users.drop();
db.ContentCreators.drop();
db.ContentPosts.drop();
db.SocialMediaAccounts.drop();
db.ContentPerformances.drop();
db.ContentRecommendations.drop();
db.Analytics.drop();
db.DashboardInsights.drop();
db.ContentChecklists.drop();
db.Schedules.drop();

// Criar usuários
print('Criando usuários...');

// IMPORTANTE: Substitua esta string pelo hash GERADO pelo código C#
const correctPasswordHash = "$2a$11$HTugFcEg3UcG8cXkAtt5s.bX7lKWWKDHORfVzuox9mlTMIRSNClxy"; 

// Remover a senha em texto plano
// const plainPassword = "senha123"; 

const users = [
  {
    _id: ObjectId(),
    name: 'João Silva',
    email: 'joao@example.com',
    username: 'joaosilva',
    passwordHash: correctPasswordHash, // Usar o hash correto
    role: 'user',
    active: true,
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    name: 'Maria Souza',
    email: 'maria@example.com',
    username: 'mariasouza',
    passwordHash: correctPasswordHash, // Usar o hash correto
    role: 'admin',
    active: true,
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    name: 'Pedro Oliveira',
    email: 'pedro@example.com',
    username: 'pedrooliveira',
    passwordHash: correctPasswordHash, // Usar o hash correto
    role: 'user',
    active: true,
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: "f0b4f3a1-0b0a-4e6a-8b0a-000000000001", // <<< USAR Guid Fixo como String
    name: 'felipe',
    email: 'felipe@example.com',
    username: 'felipe',
    passwordHash: correctPasswordHash, // Usar o hash correto
    role: 'user',
    active: true,
    createdAt: new Date(),
    updatedAt: new Date()
  }
];

db.Users.insertMany(users);
print(`Inseridos ${users.length} usuários`);

// Encontrar o usuário Felipe pelo ID fixo para referência futura
const felipeUser = users.find(u => u._id === "f0b4f3a1-0b0a-4e6a-8b0a-000000000001"); // Comparar como string
if (!felipeUser) {
  print("ERRO: Usuário Felipe não encontrado após inserção. Verifique o ID fixo.");
  // Pode-se adicionar um quit() aqui se for crítico
} else {
  print("Usuário Felipe encontrado para referência.");
}

// Criar criadores de conteúdo
print('Criando criadores de conteúdo...');
const contentCreators = [
  {
    _id: ObjectId(),
    userId: users[0]._id,
    name: 'João Creator',
    bio: 'Criador de conteúdo sobre tecnologia e programação',
    categories: ['Tecnologia', 'Programação', 'Web Development'],
    followers: 15000,
    niche: 'Tecnologia',
    platforms: ['Instagram', 'YouTube', 'TikTok'],
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    userId: users[1]._id,
    name: 'Maria Fitness',
    bio: 'Especialista em fitness e alimentação saudável',
    categories: ['Fitness', 'Saúde', 'Bem-estar'],
    followers: 50000,
    niche: 'Saúde e Bem-estar',
    platforms: ['Instagram', 'YouTube'],
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    userId: users[2]._id,
    name: 'Pedro Games',
    bio: 'Conteúdo sobre jogos e tecnologia',
    categories: ['Games', 'Tecnologia', 'Reviews'],
    followers: 25000,
    niche: 'Games',
    platforms: ['YouTube', 'Twitch', 'TikTok'],
    createdAt: new Date(),
    updatedAt: new Date()
  }
];

// Garantir que o ContentCreator para Felipe seja criado ou atualizado
if (felipeUser) {
  print("Garantindo que ContentCreator para Felipe existe com o ID correto...");
  const felipeCreatorDocument = {
    _id: felipeUser._id, // CRUCIAL: Usar o mesmo ID string do usuário
    UserId: felipeUser._id, // Link para o usuário (string)
    Name: felipeUser.name,
    Email: felipeUser.email,
    Username: felipeUser.username,
    Bio: "Perfil de teste para Felipe (criado/atualizado via script)",
    ProfileImageUrl: null,
    WebsiteUrl: null,
    CreatedAt: new Date(), // Definir na criação/substituição
    UpdatedAt: new Date(),
    TotalFollowers: 0,
    TotalPosts: 0,
    SocialMediaAccounts: [] // Assumindo que a entidade C# espera isso
  };

  const resultFelipeCreator = db.ContentCreators.replaceOne(
     { "_id": felipeUser._id }, // Filtra pelo ID string do usuário
     felipeCreatorDocument,      // O documento completo para inserir ou substituir
     { upsert: true }         // Cria se não existir, substitui se existir
  );

  if (resultFelipeCreator.upsertedId) {
    print(`ContentCreator para Felipe inserido/substituído com _id: ${resultFelipeCreator.upsertedId || felipeUser._id}`);
  } else {
    print(`ContentCreator para Felipe atualizado (matched: ${resultFelipeCreator.matchedCount}, modified: ${resultFelipeCreator.modifiedCount})`);
  }
  // Adiciona o documento criado/atualizado à lista para referência, se necessário para posts
  // contentCreators.push(felipeCreatorDoc); // Cuidado: Isso pode não ter todos os campos se for apenas $set
} else {
   print("AVISO: Não foi possível criar/atualizar ContentCreator para Felipe pois o usuário não foi encontrado.");
}

db.ContentCreators.insertMany(contentCreators); // Insere os outros criadores definidos na lista
print(`Inseridos ${contentCreators.length} outros criadores de conteúdo`);

// Criar contas sociais
print('Criando contas sociais...');
const socialAccounts = [
  {
    _id: ObjectId(),
    creatorId: contentCreators[0]._id,
    platform: 'Instagram',
    username: 'joao.tech',
    followers: 10000,
    following: 500,
    engagement: 3.5,
    connected: true,
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    creatorId: contentCreators[0]._id,
    platform: 'YouTube',
    username: 'JoãoTechChannel',
    followers: 5000,
    following: 100,
    engagement: 4.2,
    connected: true,
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    creatorId: contentCreators[1]._id,
    platform: 'Instagram',
    username: 'maria.fitness',
    followers: 45000,
    following: 800,
    engagement: 5.1,
    connected: true,
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    creatorId: contentCreators[2]._id,
    platform: 'YouTube',
    username: 'PedroGamesChannel',
    followers: 20000,
    following: 150,
    engagement: 4.8,
    connected: true,
    createdAt: new Date(),
    updatedAt: new Date()
  }
];

db.SocialMediaAccounts.insertMany(socialAccounts);
print(`Inseridas ${socialAccounts.length} contas sociais`);

// Criar posts de conteúdo
print('Criando posts de conteúdo...');
const contentPosts = [
  {
    _id: ObjectId(),
    creatorId: contentCreators[0]._id,
    title: 'Iniciando com React em 2024',
    description: 'Guia completo para iniciantes em React',
    contentType: 'video',
    status: 'published',
    mediaUrls: ['https://example.com/videos/react2024.mp4'],
    platforms: ['YouTube'],
    tags: ['React', 'JavaScript', 'Frontend'],
    categories: ['Programação', 'Web Development'],
    engagementMetrics: {
      views: 1500,
      likes: 120,
      comments: 45,
      shares: 30
    },
    publishedAt: new Date(new Date().setDate(new Date().getDate() - 10)),
    createdAt: new Date(new Date().setDate(new Date().getDate() - 15)),
    updatedAt: new Date(new Date().setDate(new Date().getDate() - 10))
  },
  {
    _id: ObjectId(),
    creatorId: contentCreators[0]._id,
    title: 'Dicas de TypeScript para Devs React',
    description: 'Melhore seu código React com TypeScript',
    contentType: 'post',
    status: 'draft',
    mediaUrls: ['https://example.com/images/typescript.jpg'],
    platforms: ['Instagram'],
    tags: ['TypeScript', 'React', 'Development'],
    categories: ['Programação'],
    createdAt: new Date(new Date().setDate(new Date().getDate() - 5)),
    updatedAt: new Date(new Date().setDate(new Date().getDate() - 5))
  },
  {
    _id: ObjectId(),
    creatorId: contentCreators[1]._id,
    title: 'Treino de 15 minutos para fazer em casa',
    description: 'Treino rápido e eficiente para seu dia a dia',
    contentType: 'video',
    status: 'published',
    mediaUrls: ['https://example.com/videos/workout15min.mp4'],
    platforms: ['Instagram', 'YouTube'],
    tags: ['Fitness', 'Treino', 'EmCasa'],
    categories: ['Fitness', 'Saúde'],
    engagementMetrics: {
      views: 8500,
      likes: 750,
      comments: 120,
      shares: 80
    },
    publishedAt: new Date(new Date().setDate(new Date().getDate() - 3)),
    createdAt: new Date(new Date().setDate(new Date().getDate() - 7)),
    updatedAt: new Date(new Date().setDate(new Date().getDate() - 3))
  },
  {
    _id: ObjectId(),
    creatorId: contentCreators[2]._id,
    title: 'Review do novo jogo XYZ',
    description: 'Análise completa do aguardado game XYZ',
    contentType: 'video',
    status: 'published',
    mediaUrls: ['https://example.com/videos/gamereview.mp4'],
    platforms: ['YouTube'],
    tags: ['Games', 'Review', 'XYZ'],
    categories: ['Games', 'Reviews'],
    engagementMetrics: {
      views: 12000,
      likes: 980,
      comments: 250,
      shares: 120
    },
    publishedAt: new Date(new Date().setDate(new Date().getDate() - 1)),
    createdAt: new Date(new Date().setDate(new Date().getDate() - 8)),
    updatedAt: new Date(new Date().setDate(new Date().getDate() - 1))
  }
];

db.ContentPosts.insertMany(contentPosts);
print(`Inseridos ${contentPosts.length} posts de conteúdo`);

// Criar recomendações
print('Criando recomendações...');
const contentRecommendations = [
  {
    _id: ObjectId(),
    creatorId: contentCreators[0]._id,
    title: 'Aumente Engajamento com Vídeos Curtos',
    description: 'Vídeos de 15-30 segundos têm 3x mais engajamento que conteúdos longos',
    score: 95,
    category: 'Formato',
    recommendationType: 'FORMAT',
    implementationDifficulty: 'Fácil',
    potentialImpact: 'Alto',
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    creatorId: contentCreators[0]._id,
    title: 'Histórias de Bastidores',
    description: 'Compartilhe o processo criativo para aumentar conexão com seguidores',
    score: 85,
    category: 'Conteúdo',
    recommendationType: 'TOPIC',
    implementationDifficulty: 'Médio',
    potentialImpact: 'Médio',
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    creatorId: contentCreators[1]._id,
    title: 'Receitas Rápidas de Fit Food',
    description: 'Conteúdo sobre receitas fitness tem alto engajamento no seu nicho',
    score: 92,
    category: 'Conteúdo',
    recommendationType: 'TOPIC',
    implementationDifficulty: 'Fácil',
    potentialImpact: 'Alto',
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    creatorId: contentCreators[2]._id,
    title: 'Streaming ao Vivo Semanal',
    description: 'Lives semanais aumentam engajamento em 65% no seu nicho',
    score: 90,
    category: 'Formato',
    recommendationType: 'FORMAT',
    implementationDifficulty: 'Médio',
    potentialImpact: 'Alto',
    createdAt: new Date(),
    updatedAt: new Date()
  }
];

db.ContentRecommendations.insertMany(contentRecommendations);
print(`Inseridas ${contentRecommendations.length} recomendações`);

// Criar métricas de analytics
print('Criando dados de analytics...');

// Função para gerar dados de analytics por dia para os últimos 30 dias
function generateDailyMetrics(baseViews, baseLikes, baseComments, baseShares, baseRevenue) {
  const metrics = [];
  const now = new Date();
  
  for (let i = 0; i < 30; i++) {
    const date = new Date(now.getTime() - (i * 24 * 60 * 60 * 1000));
    const dateFactor = 1 + (Math.sin(i / 5) * 0.3); // Variação sinusoidal para simular picos e quedas
    
    // Randomização adicional (±20%)
    const randomFactor = 0.8 + (Math.random() * 0.4);
    const combinedFactor = dateFactor * randomFactor;
    
    metrics.push({
      date: date,
      views: Math.round(baseViews * combinedFactor),
      likes: Math.round(baseLikes * combinedFactor),
      comments: Math.round(baseComments * combinedFactor),
      shares: Math.round(baseShares * combinedFactor),
      revenue: Number((baseRevenue * combinedFactor).toFixed(2)),
      ctr: Number((0.02 + (Math.random() * 0.03)).toFixed(4)), // CTR entre 2-5%
      conversionRate: Number((0.01 + (Math.random() * 0.03)).toFixed(4)) // Conversão entre 1-4%
    });
  }
  
  return metrics;
}

const analytics = [
  {
    _id: ObjectId(),
    creatorId: contentCreators[0]._id,
    platform: 'YouTube',
    metricsDaily: generateDailyMetrics(500, 50, 10, 5, 15),
    metricsTotals: {
      views: 35000,
      likes: 4200,
      comments: 850,
      shares: 320,
      revenue: 450.75,
      followers: 5000,
      followersGrowth: 320
    },
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    creatorId: contentCreators[0]._id,
    platform: 'Instagram',
    metricsDaily: generateDailyMetrics(800, 100, 25, 15, 5),
    metricsTotals: {
      views: 42000,
      likes: 6800,
      comments: 1200,
      shares: 750,
      revenue: 180.50,
      followers: 10000,
      followersGrowth: 650
    },
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    creatorId: contentCreators[1]._id,
    platform: 'Instagram',
    metricsDaily: generateDailyMetrics(1500, 200, 45, 30, 25),
    metricsTotals: {
      views: 120000,
      likes: 18500,
      comments: 3200,
      shares: 2100,
      revenue: 850.25,
      followers: 45000,
      followersGrowth: 1200
    },
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    creatorId: contentCreators[2]._id,
    platform: 'YouTube',
    metricsDaily: generateDailyMetrics(1200, 150, 40, 25, 30),
    metricsTotals: {
      views: 95000,
      likes: 12500,
      comments: 2800,
      shares: 1500,
      revenue: 1250.80,
      followers: 20000,
      followersGrowth: 850
    },
    createdAt: new Date(),
    updatedAt: new Date()
  }
];

db.Analytics.insertMany(analytics);
print(`Inseridos ${analytics.length} registros de analytics`);

print('Banco de dados populado com sucesso!');
print('Usuários criados (todos com a senha definida no script):'); // Mensagem ajustada
print('- joao@example.com');
print('- maria@example.com');
print('- pedro@example.com');
print('- felipe@example.com'); // Remover a senha da mensagem final 