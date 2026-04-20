import type { ConnectorOptions, PaymentProvider } from "../types";
export declare class SilapayProvider implements PaymentProvider {
    private readonly client;
    constructor(options?: ConnectorOptions);
    pix(dados: Record<string, unknown>): Promise<unknown>;
    boleto(dados: Record<string, unknown>): Promise<unknown>;
    cartao(dados: Record<string, unknown>): Promise<unknown>;
    saldo(): Promise<unknown>;
    private request;
}
