"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.PaymentConnector = void 0;
const adyen_1 = require("./providers/adyen");
const mercadopago_1 = require("./providers/mercadopago");
const silapay_1 = require("./providers/silapay");
const PROVIDERS = {
    silapay: silapay_1.SilapayProvider,
    mercadopago: mercadopago_1.MercadoPagoProvider,
    adyen: adyen_1.AdyenProvider,
};
class PaymentConnector {
    constructor({ provider = "silapay", ...options } = {}) {
        const Provider = PROVIDERS[provider];
        if (!Provider) {
            throw new Error(`Provider "${provider}" não suportado. Providers disponíveis: silapay, mercadopago, adyen`);
        }
        this.providerName = provider;
        this.provider = new Provider(options);
    }
    async pix(dados) {
        return this.provider.pix(dados);
    }
    async boleto(dados) {
        return this.provider.boleto(dados);
    }
    async cartao(dados) {
        return this.provider.cartao(dados);
    }
    async saldo() {
        return this.provider.saldo();
    }
}
exports.PaymentConnector = PaymentConnector;
