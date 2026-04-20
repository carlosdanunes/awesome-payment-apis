import type { ConnectorOptions, PaymentProvider } from "../types";

export class AdyenProvider implements PaymentProvider {
    private readonly apiKey?: string;
    private readonly merchantAccount?: string;

    constructor(options: ConnectorOptions = {}) {
        this.apiKey = options.apiKey || process.env.ADYEN_API_KEY;
        this.merchantAccount =
            options.merchantAccount || process.env.ADYEN_MERCHANT_ACCOUNT;
    }

    async pix(): Promise<never> {
        throw new Error("Adyen: método pix() ainda não implementado. Consulte https://docs.adyen.com/");
    }

    async boleto(): Promise<never> {
        throw new Error(
            "Adyen: método boleto() ainda não implementado. Consulte https://docs.adyen.com/"
        );
    }

    async cartao(): Promise<never> {
        throw new Error(
            "Adyen: método cartao() ainda não implementado. Consulte https://docs.adyen.com/"
        );
    }

    async saldo(): Promise<never> {
        throw new Error("Adyen: método saldo() ainda não implementado. Consulte https://docs.adyen.com/");
    }
}
