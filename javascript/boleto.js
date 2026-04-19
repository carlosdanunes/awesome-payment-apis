/**
 * Silapay — Boleto Bancário
 * Documentação: https://api.silapay.pro/v1/transactions
 *
 * Requisitos:
 *   npm install axios dotenv
 *
 * Variáveis de ambiente (.env):
 *   SILAPAY_API_KEY=sua_api_key
 *   SILAPAY_SECRET_KEY=sua_secret_key
 */

require("dotenv").config();
const axios = require("axios");

const BASE_URL = "https://api.silapay.pro/v1";

const headers = {
    accept: "application/json",
    "content-type": "application/json",
    "api-key": process.env.SILAPAY_API_KEY,
    "secret-key": process.env.SILAPAY_SECRET_KEY,
};

// ─── 1. Criar Boleto ─────────────────────────────────────────────────────────

async function criarBoleto() {
    const body = {
        paymentMethod: "billet", // obrigatório
        value: 150.50,           // obrigatório
        dueDate: "2026-06-30T00:00:00.000Z", // obrigatório — não pode ser data passada
        description: "Pagamento de serviços",
        daysAfterDueDateToRegistrationCancellation: 15,
        externalReference: "REF-001",
        installmentCount: 1,

        customer: {              // obrigatório
            name: "João da Silva",
            email: "joao.silva@email.com",
            birthDate: "1990-05-20",
            phone: "5511987654321", // código de país (55) + DDD + número
            document: "12345678901", // CPF sem pontuação
            userAgent: "Mozilla/5.0 (compatible)",
            street: "Rua das Flores",
            addressNumber: "123",
            complement: "Apto 456",
            postalCode: "01234567",
            neighborhood: "Centro",
            city: "São Paulo",
            state: "SP",
            country: "Brasil",
        },

        // Desconto (opcional)
        discount: {
            value: 10.50,
            dueDateLimitDays: 5,
            type: "Fixed", // "Fixed" | "Percentage"
        },

        // Juros ao mês após vencimento (opcional)
        interest: {
            value: 2.0,
        },

        // Multa após vencimento (opcional)
        fine: {
            value: 2.0,
            type: "Percentage", // "Fixed" | "Percentage"
        },
    };

    try {
        const { data } = await axios.post(
            `${BASE_URL}/transactions`,
            body,
            { headers }
        );

        console.log("✅ Boleto criado com sucesso!");
        console.log("  Transação ID :", data.transaction.transactionId);
        console.log("  Status       :", data.transaction.status);
        console.log("  Código de barras:", data.transaction.barCode);
        console.log("  Criado em    :", data.transaction.createdAt);

        return data;
    } catch (err) {
        console.error("❌ Erro ao criar boleto:");
        console.error(err.response?.data ?? err.message);
        throw err;
    }
}

// ─── 2. Consultar Saldo ──────────────────────────────────────────────────────

async function consultarSaldo() {
    try {
        const { data } = await axios.get(
            `${BASE_URL}/user/finance/balance`,
            { headers }
        );

        console.log("💰 Saldo disponível: R$", data.balance);
        return data;
    } catch (err) {
        console.error("❌ Erro ao consultar saldo:");
        console.error(err.response?.data ?? err.message);
        throw err;
    }
}

// ─── 3. Handler de Webhook ───────────────────────────────────────────────────

/**
 * Statuses possíveis recebidos via webhook:
 *
 * PAID                        — Pago
 * LIQUIDATED                  — Liquidado
 * COMPLETED                   — Concluído
 * PENDING                     — Pendente
 * CREATED                     — Criado
 * FAILED                      — Falhou
 * REFUSED                     — Recusado
 * CANCELED                    — Cancelado
 * REJECTED                    — Rejeitado
 * PROCESSING                  — Processando
 * PROCESSED                   — Processado
 * DELETED                     — Excluído
 * RESTORED                    — Restaurado
 * PAYMENT_AUTHORIZED          — Pagamento autorizado
 * ANTICIPATED                 — Antecipado
 * OVERDUE                     — Vencido
 * UNKNOWN                     — Desconhecido
 * WAITING_PAYMENT             — Aguardando pagamento
 * REFUNDED                    — Reembolsado
 * REFUND_IN_PROGRESS          — Reembolso em andamento
 * PARTIALLY_REFUNDED          — Reembolsado parcialmente
 * AWAITING_RISK_ANALYSIS      — Aguardando análise de risco
 * APPROVED_BY_RISK_ANALYSIS   — Aprovado na análise de risco
 * REPROVED_BY_RISK_ANALYSIS   — Reprovado na análise de risco
 * BLOCKED                     — Bloqueado
 * CHARGEDBACK                 — Chargeback confirmado
 * CHARGEBACK_REQUESTED        — Chargeback solicitado
 * CHARGEBACK_DISPUTE          — Chargeback em disputa
 * AWAITING_CHARGEBACK_REVERSAL— Aguardando reversão de chargeback
 * DISPUTE                     — Em disputa
 */
function handleWebhook(payload) {
    // Exemplo de payload recebido:
    // {
    //   "id": "evt_e419ceea-d800-4845-a3fa-6a472d3b44c1",
    //   "status": "LIQUIDATED",
    //   "created": "2025-07-30T14:04:25.000Z",
    //   "data": {
    //     "value": "16.5",
    //     "txId": "bada2311-2eed-416f-871a-2ef6585ee822",
    //     "method": "pix",
    //     "txMessage": "Payment received.",
    //     "postbackUrl": "http://www.seusite.com.br/webhook/"
    //   }
    // }

    const { id, status, created, data } = payload;

    console.log(`📬 Webhook recebido — ID: ${id}`);
    console.log(`   Status  : ${status}`);
    console.log(`   Valor   : R$ ${data.value}`);
    console.log(`   Tx ID   : ${data.txId}`);
    console.log(`   Método  : ${data.method}`);
    console.log(`   Criado  : ${created}`);

    switch (status) {
        case "PAID":
        case "LIQUIDATED":
        case "COMPLETED":
            console.log("✅ Pagamento confirmado — liberar pedido/serviço");
            break;
        case "OVERDUE":
            console.log("⏰ Boleto vencido — notificar cliente");
            break;
        case "CANCELED":
        case "DELETED":
            console.log("🚫 Transação cancelada");
            break;
        case "FAILED":
        case "REFUSED":
        case "REJECTED":
            console.log("❌ Transação recusada/falhou");
            break;
        case "REFUNDED":
        case "PARTIALLY_REFUNDED":
            console.log("↩️ Reembolso efetuado");
            break;
        default:
            console.log(`ℹ️ Status não tratado: ${status}`);
    }
}

// ─── Execução ─────────────────────────────────────────────────────────────────

(async () => {
    await consultarSaldo();
    await criarBoleto();

    // Simulação de webhook recebido
    handleWebhook({
        id: "evt_e419ceea-d800-4845-a3fa-6a472d3b44c1",
        status: "LIQUIDATED",
        created: "2026-04-19T14:04:25.000Z",
        data: {
            value: "150.5",
            txId: "3f6f364b-1152-46f6-8ad4-5b5c18b13d69",
            method: "billet",
            txMessage: "Payment received.",
            postbackUrl: "http://www.seusite.com.br/webhook/",
        },
    });
})();
