const SilapayProvider = require("./providers/silapay");
const MercadoPagoProvider = require("./providers/mercadopago");
const AdyenProvider = require("./providers/adyen");

const PROVIDERS = {
    silapay: SilapayProvider,
    mercadopago: MercadoPagoProvider,
    adyen: AdyenProvider,
};

class PaymentConnector {
    constructor({ provider = "silapay", ...options } = {}) {
        const Provider = PROVIDERS[provider];

        if (!Provider) {
            throw new Error(
                `Provider "${provider}" não suportado. Providers disponíveis: silapay, mercadopago, adyen`
            );
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

module.exports = PaymentConnector;
