# JavaScript — Silapay Integration

> Exemplos com Node.js puro (sem TypeScript) e Express.

## Conteúdo planejado

- [ ] PIX CashIn (gerar QR Code)
- [ ] PIX CashOut (transferir para chave PIX)
- [ ] Cartão de crédito
- [ ] Boleto bancário
- [ ] Webhook handler com Express

## Stack

- Node.js 20+
- JavaScript (CommonJS e ESM)
- Express 4+
- axios

## Como rodar

```bash
npm install
cp .env.example .env
# preenche as chaves no .env
node pix-cashin.js
```

## Arquivos

| Arquivo | Descrição |
|---|---|
| `pix-cashin.js` | Exemplo mínimo — PIX CashIn |
| `pix-cashout.js` | Exemplo mínimo — PIX CashOut |
| `cartao.js` | Exemplo mínimo — cartão de crédito |
| `boleto.js` | Exemplo mínimo — boleto bancário |
| `express-webhook.js` | Servidor Express para receber postback |
