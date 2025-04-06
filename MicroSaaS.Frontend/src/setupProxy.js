const { createProxyMiddleware } = require('http-proxy-middleware');
require('dotenv').config();

// Configuração de proxy para a API
module.exports = function(app) {
  console.log('⚙️ Configurando proxy para a API...');

  // URL alvo fixa para garantir conexão com o backend
  const targetUrl = 'https://localhost:7171';
  console.log(`🎯 URL alvo do proxy: ${targetUrl}`);

  // Opções de proxy comuns
  const proxyOptions = {
    target: targetUrl,
    changeOrigin: true,
    secure: false, // Ignora erros de SSL em desenvolvimento
    onError: (err, req, res) => {
      console.error('Erro no proxy:', err);
      res.writeHead(500, {
        'Content-Type': 'text/plain'
      });
      res.end('Erro de proxy: Não foi possível conectar ao servidor backend.');
    }
  };

  // Proxy para a API
  app.use(
    '/api',
    createProxyMiddleware({
      ...proxyOptions,
      pathRewrite: {
        '^/api': '/api'
      },
      onProxyReq: (proxyReq, req, res) => {
        console.log(`[Proxy] ${req.method} ${req.url} -> ${proxyReq.protocol}//${proxyReq.host}${proxyReq.path}`);
      },
      logLevel: 'debug'
    })
  );

  // Redirecionar /swagger para a documentação da API
  app.use(
    '/swagger',
    createProxyMiddleware({
      ...proxyOptions,
      onProxyReq: (proxyReq, req, res) => {
        console.log(`[Proxy Swagger] ${req.method} ${req.url} -> ${proxyReq.protocol}//${proxyReq.host}${proxyReq.path}`);
      }
    })
  );

  console.log('✅ Proxy configurado com sucesso para https://localhost:7171!');
};