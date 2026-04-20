import type { ConnectorOptions, PaymentProvider } from "../types";
export declare class AdyenProvider implements PaymentProvider {
    private readonly apiKey?;
    private readonly merchantAccount?;
    constructor(options?: ConnectorOptions);
    pix(): Promise<never>;
    boleto(): Promise<never>;
    cartao(): Promise<never>;
    saldo(): Promise<never>;
}
