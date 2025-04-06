# Guia de Debug do MicroSaaS Backend

Este documento explica como configurar e executar o debug do backend MicroSaaS usando o Visual Studio 2022 e Docker.

## Pré-requisitos

- Visual Studio 2022 com suporte a .NET 8 instalado
- Docker Desktop funcionando
- Configuração de debug já adicionada ao projeto (feito por nós)

## Passo 1: Iniciar Containers de Suporte

Para realizar o debug, precisamos que os containers de MongoDB e Redis estejam em execução, enquanto o Visual Studio irá gerenciar o container do backend.

Execute o script `start-debug.bat` para iniciar apenas os containers de suporte:

```
start-debug.bat
```

Este script irá:
1. Verificar se os containers MongoDB e Redis estão rodando
2. Iniciar os containers necessários se não estiverem rodando
3. Exibir o status dos containers

## Passo 2: Configurar o Debug no Visual Studio 2022

1. Abra a solução `MicroSaaS.sln` no Visual Studio 2022
2. No Solution Explorer, clique com o botão direito na solução (não no projeto)
3. Selecione "Properties" (Propriedades)
4. Na aba "Startup Project", escolha:
   - Selecione "Single startup project" (Projeto único de inicialização)
   - Escolha "MicroSaaS.Backend" na lista suspensa

## Passo 3: Iniciar o Debug

1. No Visual Studio, certifique-se que o perfil "Docker" está selecionado na barra de ferramentas
2. Coloque breakpoints no código onde deseja pausar a execução
3. Pressione F5 ou clique no botão "Start Debugging" (ícone de play verde)

O Visual Studio irá:
- Construir o projeto
- Criar e iniciar o container do backend
- Anexar o debugger ao processo
- Parar nos breakpoints quando o código for executado

## Passo 4: Testar a API

1. Abra o Swagger em: https://localhost:7171/swagger/index.html
2. Execute alguma operação (ex: login, registro)
3. O código irá parar no breakpoint correspondente

## Solução de Problemas

### Se o MongoDB não estiver acessível

Erro comum: `MongoDB.Driver.MongoConnectionException: A timeout occurred after 30000ms selecting a server...`

Verifique se:
1. O container do MongoDB está rodando: `docker ps | findstr mongodb`
2. O nome do host está correto: em ambiente Docker, use `mongodb` e não `localhost`
3. A porta está acessível: `docker logs mongodb`

### Se o Redis não estiver acessível

Verifique se:
1. O container do Redis está rodando: `docker ps | findstr redis`
2. A conexão está correta em appsettings.json: `redis:6379` para Docker

### Se ainda houver problemas

Tente reiniciar todos os containers:

```
docker-compose down
./start-debug.bat
```

## Dicas para Debug Eficiente

1. Coloque breakpoints estratégicos nos controladores e serviços
2. Use a janela "Immediate Window" para avaliar expressões durante o debug
3. Observe as variáveis ​​locais na janela "Locals"
4. Use "Step Into" (F11) para entrar em métodos
5. Use "Step Over" (F10) para executar linha a linha

## Finalizar o Debug

1. Clique em "Stop Debugging" no Visual Studio ou pressione Shift+F5
2. Os containers de suporte (MongoDB, Redis) continuarão rodando para uso posterior
3. Para parar todos os containers: `docker-compose down` 