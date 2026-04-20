import type { ConnectorOptions, PaymentProvider } from "./types";
export declare class PaymentConnector implements PaymentProvider {
    private readonly provider;
    readonly providerName: string;
    constructor({ provider, ...options }?: ConnectorOptions);
    pix(dados: Record<string, unknown>): Promise<unknown>;
    boleto(dados: Record<string, unknown>): Promise<unknown>;
    cartao(dados: Record<string, unknown>): Promise<unknown>;
    saldo(): Promise<unknown>;
}
