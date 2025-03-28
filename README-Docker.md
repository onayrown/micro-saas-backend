# MicroSaaS - Configuração Docker

Este documento descreve como configurar e executar o MicroSaaS em um ambiente Docker, incluindo Redis e MongoDB.

## Pré-requisitos

- [Docker](https://www.docker.com/products/docker-desktop/) instalado e funcionando no seu sistema
- [Docker Compose](https://docs.docker.com/compose/install/) instalado (já vem com o Docker Desktop para Windows/Mac)
- Portas disponíveis:
  - 7169 e 7170 (API)
  - 27017 (MongoDB)
  - 6379 (Redis)
  - 8081 (MongoDB Express - interface administrativa)
  - 8082 (Redis Commander - interface administrativa)

## Estrutura do Ambiente

O ambiente Docker inclui os seguintes serviços:

1. **API** - Aplicação ASP.NET Core
2. **MongoDB** - Banco de dados NoSQL
3. **Redis** - Cache e gerenciamento de sessão
4. **Mongo Express** - Interface web para administração do MongoDB
5. **Redis Commander** - Interface web para administração do Redis

## Configuração

Todos os serviços são configurados automaticamente via Docker Compose. As configurações específicas da aplicação para o ambiente Docker estão definidas no arquivo `appsettings.Docker.json`.

## Inicialização

### Windows

Execute o script `start-docker.bat` no Windows para iniciar todos os serviços:

```
start-docker.bat
```

Este script irá:
1. Gerar um certificado SSL auto-assinado para HTTPS
2. Parar containers existentes (se houver)
3. Construir e iniciar todos os serviços
4. Mostrar o status dos containers

### Linux/Mac

Execute o script `start-docker.sh` no Linux/Mac para iniciar todos os serviços:

```bash
chmod +x start-docker.sh
./start-docker.sh
```

O script realizará as mesmas operações que a versão para Windows.

### Inicialização Manual

Alternativamente, você pode iniciar os serviços manualmente com os seguintes comandos:

```bash
# Gerar certificado SSL (Windows)
powershell -ExecutionPolicy Bypass -File .\setup-cert.ps1

# OU Gerar certificado SSL (Linux/Mac)
chmod +x ./setup-cert.sh
./setup-cert.sh

# Iniciar todos os serviços em segundo plano
docker-compose up -d

# Reconstruir e iniciar
docker-compose up --build -d

# Parar todos os serviços
docker-compose down
```

## Acessando os Serviços

Após a inicialização, você pode acessar os serviços nos seguintes endereços:

- **API e Swagger:** [https://localhost:7170/swagger/index.html](https://localhost:7170/swagger/index.html)
- **MongoDB Express:** [http://localhost:8081](http://localhost:8081)
  - Usuário: admin
  - Senha: senha123
- **Redis Commander:** [http://localhost:8082](http://localhost:8082)

## Persistência de Dados

Os dados são persistidos em volumes Docker, o que significa que eles são preservados mesmo quando os containers são reiniciados ou recriados:

- `mongodb_data`: Dados do MongoDB
- `redis_data`: Dados do Redis

Para limpar completamente os dados e começar do zero, descomente as linhas relevantes nos scripts de inicialização ou execute:

```bash
docker-compose down
docker volume rm microsaas_mongodb_data microsaas_redis_data
```

## Solução de Problemas

### Verificar Status dos Containers

```bash
docker-compose ps
```

### Ver Logs de um Serviço Específico

```bash
docker-compose logs -f api        # Logs da API
docker-compose logs -f mongodb    # Logs do MongoDB
docker-compose logs -f redis      # Logs do Redis
```

### Reiniciar um Serviço Específico

```bash
docker-compose restart api
```

### Certificados SSL

Se você encontrar problemas com certificados SSL ao acessar a API via HTTPS, você precisará configurar os certificados de desenvolvimento para o Docker. Consulte a documentação da Microsoft sobre [HTTPS em ASP.NET Core com Docker](https://docs.microsoft.com/pt-br/aspnet/core/security/docker-https) para mais detalhes.

## Para Ambientes de Produção

Esta configuração é otimizada para desenvolvimento. Para produção, são recomendadas as seguintes alterações:

1. Use secrets seguros e gerenciados para senhas e chaves
2. Configure replicas para MongoDB e Redis
3. Implemente backups automatizados
4. Configure limites de recursos e escalabilidade
5. Utilize serviços gerenciados de nuvem quando possível
6. Remova as ferramentas administrativas (Mongo Express e Redis Commander) 