"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AdyenProvider = void 0;
class AdyenProvider {
    constructor(options = {}) {
        this.apiKey = options.apiKey || process.env.ADYEN_API_KEY;
        this.merchantAccount =
            options.merchantAccount || process.env.ADYEN_MERCHANT_ACCOUNT;
    }
    async pix() {
        throw new Error("Adyen: método pix() ainda não implementado. Consulte https://docs.adyen.com/");
    }
    async boleto() {
        throw new Error("Adyen: método boleto() ainda não implementado. Consulte https://docs.adyen.com/");
    }
    async cartao() {
        throw new Error("Adyen: método cartao() ainda não implementado. Consulte https://docs.adyen.com/");
    }
    async saldo() {
        throw new Error("Adyen: método saldo() ainda não implementado. Consulte https://docs.adyen.com/");
    }
}
exports.AdyenProvider = AdyenProvider;
