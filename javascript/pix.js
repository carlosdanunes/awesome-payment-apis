/**
 * Silapay — PIX CashIn e CashOut
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

// ─── 1. PIX CashIn — Gerar cobrança ─────────────────────────────────────────

async function pixCashIn() {
    const body = {
        paymentMethod: "pix",          // obrigatório
        cashFlowType: "cashIn",        // obrigatório
        value: 50.00,                  // obrigatório
        postbackUrl: "http://www.seusite.com.br/webhook/", // obrigatório
        description: "Venda de produto", // opcional

        // Produtos (opcional)
        products: [
            {
                name: "AirPod",
                price: 1,
                total: 2,
                quantity: 2,
            },
        ],

        // Cliente
        customer: {
            name: "João da Silva",       // opcional
            email: "joao@email.com",     // opcional
            phone: "5511987654321",      // opcional
            document: "12345678910",     // CPF/CNPJ obrigatório
            userAgent: "Mozilla/5.0",    // opcional
            street: "Rua das Flores",    // opcional
            complement: "Apto 1",        // opcional
            postalCode: "01234567",      // opcional
            neighborhood: "Centro",      // opcional
            state: "SP",                 // opcional
            country: "Brasil",           // opcional
        },
    };

    try {
        const { data } = await axios.post(
            `${BASE_URL}/transactions`,
            body,
            { headers }
        );

        console.log("✅ PIX CashIn gerado com sucesso!");
        console.log("  Transação ID  :", data.id);
        console.log("  Status        :", data.status);
        console.log("  Pix Copia e Cola:", data.pixCopiaECola);
        console.log("  Criado em     :", data.createdAt);

        return data;
    } catch (err) {
        console.error("❌ Erro ao gerar PIX CashIn:");
        console.error(err.response?.data ?? err.message);
        throw err;
    }
}

// ─── 2. PIX CashOut — Efetuar pagamento ─────────────────────────────────────

async function pixCashOut() {
    const body = {
        paymentMethod: "pix",          // obrigatório
        cashFlowType: "cashOut",       // obrigatório
        value: 1,                      // obrigatório
        pixKey: "29242661066",         // obrigatório — chave PIX do destinatário
        pixKeyType: "cpf",             // obrigatório — "cpf" | "cnpj" | "phone" | "evp"
        postbackUrl: "http://www.seusite.com.br/webhook/", // obrigatório
        description: "Venda com Pix",  // opcional
    };

    try {
        const { data } = await axios.post(
            `${BASE_URL}/transactions`,
            body,
            { headers }
        );

        console.log("✅ PIX CashOut enviado com sucesso!");
        console.log("  Transação ID  :", data.id);
        console.log("  Status        :", data.status);
        console.log("  Chave PIX     :", data.pixKey);
        console.log("  Criado em     :", data.createdAt);

        return data;
    } catch (err) {
        console.error("❌ Erro ao enviar PIX CashOut:");
        console.error(err.response?.data ?? err.message);
        throw err;
    }
}

// ─── 3. Consultar Saldo ──────────────────────────────────────────────────────

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

// ─── Execução ─────────────────────────────────────────────────────────────────

(async () => {
    await consultarSaldo();
    await pixCashIn();
    await pixCashOut();
})();
