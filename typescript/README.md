

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

## Conector unificado

Existe uma implementação inicial em `silapay-connector/` com o mesmo padrão das versões JavaScript e Python:

- `PaymentConnector` com provider padrão `silapay`
- métodos unificados: `pix(dados)`, `boleto(dados)`, `cartao(dados)` e `saldo()`
- provider real da Silapay
- stubs estruturados para MercadoPago e Adyen

### Como testar o conector

```bash
cd typescript/silapay-connector
npm install
cp .env.example .env
npm run build
npm run exemplo
```
