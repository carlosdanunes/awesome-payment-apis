# TypeScript — Silapay Integration

> Mês 1 do roadmap. Exemplos com NestJS e Node.js puro.

## Conteúdo planejado

- [ ] PIX CashIn (gerar QR Code)
- [ ] PIX CashOut (transferir para chave PIX)
- [ ] Cartão de crédito
- [ ] Boleto bancário
- [ ] Webhook handler
- [ ] Autenticação JWT + Redis

## Stack

- Node.js 20+
- TypeScript 5+
- NestJS 10+
- Prisma (MySQL)
- Redis (ioredis)

## Como rodar

```bash
npm install
cp .env.example .env
# preenche as chaves no .env
npx prisma migrate dev
npm run start:dev
```

## Arquivos

| Arquivo | Descrição |
|---|---|
| `nestjs-silapay/` | Projeto NestJS completo com todos os métodos |
| `node-pix-cashin.ts` | Exemplo mínimo — PIX CashIn sem framework |
| `node-pix-cashout.ts` | Exemplo mínimo — PIX CashOut sem framework |
| `node-cartao.ts` | Exemplo mínimo — cartão de crédito |
| `node-boleto.ts` | Exemplo mínimo — boleto bancário |
| `webhook-handler.ts` | Receber e processar postback da Silapay |
