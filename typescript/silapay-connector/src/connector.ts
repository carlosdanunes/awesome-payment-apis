import { AdyenProvider } from "./providers/adyen";
import { MercadoPagoProvider } from "./providers/mercadopago";
import { SilapayProvider } from "./providers/silapay";
import type { ConnectorOptions, PaymentProvider } from "./types";

const PROVIDERS: Record<string, new (options: ConnectorOptions) => PaymentProvider> = {
    silapay: SilapayProvider,
    mercadopago: MercadoPagoProvider,
    adyen: AdyenProvider,
};

export class PaymentConnector implements PaymentProvider {
    private readonly provider: PaymentProvider;
    readonly providerName: string;

    constructor({ provider = "silapay", ...options }: ConnectorOptions = {}) {
        const Provider = PROVIDERS[provider];

        if (!Provider) {
            throw new Error(
                `Provider "${provider}" não suportado. Providers disponíveis: silapay, mercadopago, adyen`
            );
        }

        this.providerName = provider;
        this.provider = new Provider(options);
    }

    async pix(dados: Record<string, unknown>): Promise<unknown> {
        return this.provider.pix(dados);
    }

    async boleto(dados: Record<string, unknown>): Promise<unknown> {
        return this.provider.boleto(dados);
    }

    async cartao(dados: Record<string, unknown>): Promise<unknown> {
        return this.provider.cartao(dados);
    }

    async saldo(): Promise<unknown> {
        return this.provider.saldo();
    }
}
