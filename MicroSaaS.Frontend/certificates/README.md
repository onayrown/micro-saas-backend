# HTTPS para Desenvolvimento

Este diretório foi originalmente criado para armazenar certificados SSL personalizados, mas agora estamos usando a funcionalidade nativa do React Scripts para gerar certificados autoassinados.

## Usando HTTPS em Desenvolvimento

Para iniciar o servidor de desenvolvimento com HTTPS:

```bash
npm run start:secure
```

O React Scripts gerará automaticamente certificados SSL temporários para o desenvolvimento local.

## Observações

- Os certificados gerados são apenas para desenvolvimento local
- Os navegadores mostrarão um aviso de segurança para certificados autoassinados
- Você pode adicionar uma exceção de segurança no navegador para o localhost
- Em produção, use certificados válidos emitidos por uma autoridade certificadora confiável

## Alternativa para Produção

Para um ambiente de produção, recomendamos:

1. Adquirir certificados de uma autoridade certificadora confiável (como Let's Encrypt)
2. Configurar um proxy reverso (como Nginx ou Caddy) para lidar com HTTPS
3. Ou usar um serviço de hospedagem que fornece HTTPS automaticamente (como Vercel, Netlify, etc.)
