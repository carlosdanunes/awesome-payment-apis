import type { ConnectorOptions, PaymentProvider } from "../types";

export class MercadoPagoProvider implements PaymentProvider {
    private readonly accessToken?: string;

    constructor(options: ConnectorOptions = {}) {
        this.accessToken = options.accessToken || process.env.MERCADOPAGO_ACCESS_TOKEN;
    }

    async pix(): Promise<never> {
        throw new Error(
            "MercadoPago: método pix() ainda não implementado. Consulte https://www.mercadopago.com.br/developers"
        );
    }

    async boleto(): Promise<never> {
        throw new Error(
            "MercadoPago: método boleto() ainda não implementado. Consulte https://www.mercadopago.com.br/developers"
        );
    }

    async cartao(): Promise<never> {
        throw new Error(
            "MercadoPago: método cartao() ainda não implementado. Consulte https://www.mercadopago.com.br/developers"
        );
    }

    async saldo(): Promise<never> {
        throw new Error(
            "MercadoPago: método saldo() ainda não implementado. Consulte https://www.mercadopago.com.br/developers"
        );
    }
}
