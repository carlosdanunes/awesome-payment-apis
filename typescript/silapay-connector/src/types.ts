export interface PaymentProvider {
    pix(dados: Record<string, unknown>): Promise<unknown>;
    boleto(dados: Record<string, unknown>): Promise<unknown>;
    cartao(dados: Record<string, unknown>): Promise<unknown>;
    saldo(): Promise<unknown>;
}

export interface ConnectorOptions {
    provider?: "silapay" | "mercadopago" | "adyen" | string;
    apiKey?: string;
    secretKey?: string;
    accessToken?: string;
    merchantAccount?: string;
    baseURL?: string;
    timeout?: number;
}

export interface ApiError extends Error {
    response?: unknown;
    responseData?: unknown;
}
