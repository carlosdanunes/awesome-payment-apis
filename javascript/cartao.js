/**
 * Silapay — Cartão de Crédito
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

// ─── 1. Criar cobrança com Cartão de Crédito ─────────────────────────────────

async function cobrarCartao() {
  const body = {
    paymentMethod: "creditCard",   // obrigatório
    value: 100,                    // obrigatório
    totalValue: 110,               // obrigatório
    dueDate: "2026-06-30",         // obrigatório
    postbackUrl: "http://www.seusite.com.br/webhook/", // obrigatório
    description: "Venda de produto",
    installmentCount: 2,
    installmentValue: 55,

    // Desconto (opcional)
    discount: {
      value: 1,
      dueDateLimitDays: 2,
      type: "PERCENTAGE",          // "PERCENTAGE" | "FIXED"
    },

    // Juros (opcional)
    interest: {
      value: 1,
    },

    // Multa (opcional)
    fine: {
      value: 0,
      type: "PERCENTAGE",
    },

    postalService: false,

    // Redirecionamento após pagamento (opcional)
    callback: {
      successUrl: "https://www.seusite.com/obrigado",
      autoRedirect: false,
    },

    // Cliente — obrigatório
    customer: {
      name: "João da Silva",       // obrigatório
      email: "joao@email.com",     // obrigatório
      phone: "11999999999",        // obrigatório
      document: "12345678910",     // obrigatório — CPF/CNPJ
      userAgent: "Mozilla/5.0",    // opcional
      street: "Rua das Flores",    // obrigatório
      complement: "Apto 1",        // obrigatório
      postalCode: "01306010",      // obrigatório
      neighborhood: "Centro",      // opcional
      city: "São Paulo",           // obrigatório
      state: "SP",                 // obrigatório
      country: "Brasil",           // obrigatório
    },

    // Dados do cartão — obrigatório se não houver cardToken
    card: {
      // cardToken: "TOK_ap2o34u5atedmchrtisasprt", // usar quando disponível
      cardHolderName: "JOAO DA SILVA",  // obrigatório
      cardNumber: "4242424242424242",   // obrigatório
      expirationMonth: "06",            // obrigatório
      expirationYear: "42",             // obrigatório
      cvv: "424",                       // obrigatório
    },

    // Titular do cartão (pode ser diferente do cliente)
    cardOwner: {
      name: "João da Silva",       // obrigatório
      document: "12345678910",     // obrigatório
    },

    // Produtos (opcional)
    products: [
      {
        name: "AirPod",            // obrigatório
        price: 55,                 // obrigatório
        total: 110,                // obrigatório
        quantity: 2,               // obrigatório
        image: "https://www.seusite.com/image.png", // opcional
      },
    ],
  };

  try {
    const { data } = await axios.post(
      `${BASE_URL}/transactions`,
      body,
      { headers }
    );

    console.log("✅ Cartão de crédito cobrado com sucesso!");
    console.log("  Status          :", data.status);
    console.log("  Valor           : R$", data.value);
    console.log("  Últimos dígitos :", data.cardLastDigits);
    console.log("  Autorização     :", data.authorizationCode);
    console.log("  Criado em       :", data.createdAt);

    return data;
  } catch (err) {
    console.error("❌ Erro ao cobrar cartão de crédito:");
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

// ─── Execução ─────────────────────────────────────────────────────────────────

(async () => {
  await consultarSaldo();
  await cobrarCartao();
})();
