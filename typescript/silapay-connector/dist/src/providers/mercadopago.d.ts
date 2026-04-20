import type { ConnectorOptions, PaymentProvider } from "../types";
export declare class MercadoPagoProvider implements PaymentProvider {
    private readonly accessToken?;
    constructor(options?: ConnectorOptions);
    pix(): Promise<never>;
    boleto(): Promise<never>;
    cartao(): Promise<never>;
    saldo(): Promise<never>;
}
