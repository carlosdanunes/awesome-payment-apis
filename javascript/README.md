# JavaScript — Silapay Integration

> Exemplos com Node.js puro usando `axios` e `dotenv`.

## Conteúdo

- [x] Boleto Bancário (`boleto.js`)
- [ ] PIX CashIn (gerar QR Code)
- [ ] PIX CashOut (transferir para chave PIX)
- [ ] Cartão de crédito
- [ ] Webhook handler com Express

## Stack

- Node.js 20+
- JavaScript (CommonJS)
- axios
- dotenv

## Como rodar

```bash
cd javascript
npm install
cp .env.example .env
# preencha as chaves no .env
node boleto.js
```

## Variáveis de ambiente

```env
SILAPAY_API_KEY=sua_api_key_aqui
SILAPAY_SECRET_KEY=sua_secret_key_aqui
```

> As chaves são geradas no portal Sila Pay → botão **API Key** no menu lateral.

## Arquivos

| Arquivo | Descrição |
|---|---|
| `boleto.js` | Criar boleto, consultar saldo e handler de webhook |
| `.env.example` | Modelo de variáveis de ambiente |
| `package.json` | Dependências do projeto |

---

## Boleto Bancário

**Endpoint:** `POST https://api.silapay.pro/v1/transactions`

### Campos obrigatórios

| Campo | Tipo | Descrição |
|---|---|---|
| `paymentMethod` | string | `"billet"` |
| `value` | number | Valor com até 2 casas decimais |
| `dueDate` | string | ISO 8601 — não pode ser data passada |
| `customer.name` | string | Nome completo |
| `customer.email` | string | E-mail |
| `customer.birthDate` | string | `YYYY-MM-DD` |
| `customer.phone` | string | Com DDI (ex: `5511987654321`) |
| `customer.document` | string | CPF sem pontuação |
| `customer.street` | string | Rua/Avenida |
| `customer.addressNumber` | string | Número |
| `customer.postalCode` | string | CEP sem traço |
| `customer.neighborhood` | string | Bairro |
| `customer.city` | string | Cidade |
| `customer.state` | string | UF (ex: `SP`) |
| `customer.country` | string | País (ex: `Brasil`) |

### Resposta

```json
{
  "message": "Transação criada com sucesso",
  "transaction": {
    "method": "billet",
    "status": "Pendente",
    "transactionId": "3f6f364b-1152-46f6-8ad4-5b5c18b13d69",
    "createdAt": "2026-03-27T13:18:18.876Z",
    "barCode": "23793391009000596809513000807506114030000000600"
  }
}
```

---

## Webhook — Statuses

| Status | Descrição |
|---|---|
| `PAID` | Pago |
| `LIQUIDATED` | Liquidado |
| `COMPLETED` | Concluído |
| `PENDING` | Pendente |
| `CREATED` | Criado |
| `FAILED` | Falhou |
| `REFUSED` | Recusado |
| `CANCELED` | Cancelado |
| `REJECTED` | Rejeitado |
| `PROCESSING` | Processando |
| `PROCESSED` | Processado |
| `DELETED` | Excluído |
| `RESTORED` | Restaurado |
| `PAYMENT_AUTHORIZED` | Pagamento autorizado |
| `ANTICIPATED` | Antecipado |
| `OVERDUE` | Vencido |
| `UNKNOWN` | Desconhecido |
| `WAITING_PAYMENT` | Aguardando pagamento |
| `REFUNDED` | Reembolsado |
| `REFUND_IN_PROGRESS` | Reembolso em andamento |
| `PARTIALLY_REFUNDED` | Reembolsado parcialmente |
| `AWAITING_RISK_ANALYSIS` | Aguardando análise de risco |
| `APPROVED_BY_RISK_ANALYSIS` | Aprovado na análise de risco |
| `REPROVED_BY_RISK_ANALYSIS` | Reprovado na análise de risco |
| `BLOCKED` | Bloqueado |
| `CHARGEDBACK` | Chargeback confirmado |
| `CHARGEBACK_REQUESTED` | Chargeback solicitado |
| `CHARGEBACK_DISPUTE` | Chargeback em disputa |
| `AWAITING_CHARGEBACK_REVERSAL` | Aguardando reversão de chargeback |
| `DISPUTE` | Em disputa |

### Payload do webhook

```json
{
  "id": "evt_e419ceea-d800-4845-a3fa-6a472d3b44c1",
  "status": "LIQUIDATED",
  "created": "2025-07-30T14:04:25.000Z",
  "data": {
    "value": "150.5",
    "txId": "3f6f364b-1152-46f6-8ad4-5b5c18b13d69",
    "method": "billet",
    "txMessage": "Payment received.",
    "postbackUrl": "http://www.seusite.com.br/webhook/"
  }
}
```

---

## Saldo

**Endpoint:** `GET https://api.silapay.pro/v1/user/finance/balance`

```json
{ "balance": 200 }
```
