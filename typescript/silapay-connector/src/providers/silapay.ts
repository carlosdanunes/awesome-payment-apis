import axios, { AxiosError, AxiosInstance } from "axios";

import type { ApiError, ConnectorOptions, PaymentProvider } from "../types";

const BASE_URL = "https://api.silapay.pro/v1";

export class SilapayProvider implements PaymentProvider {
    private readonly client: AxiosInstance;

    constructor(options: ConnectorOptions = {}) {
        const apiKey = options.apiKey || process.env.SILAPAY_API_KEY;
        const secretKey = options.secretKey || process.env.SILAPAY_SECRET_KEY;

        if (!apiKey || !secretKey) {
            throw new Error("Silapay: defina SILAPAY_API_KEY e SILAPAY_SECRET_KEY no ambiente.");
        }

        this.client = axios.create({
            baseURL: options.baseURL || BASE_URL,
            timeout: options.timeout || 30000,
            headers: {
                accept: "application/json",
                "content-type": "application/json",
                "api-key": apiKey,
                "secret-key": secretKey,
            },
        });
    }

    async pix(dados: Record<string, unknown>): Promise<unknown> {
        return this.request("post", "/transactions", {
            paymentMethod: "pix",
            ...dados,
        });
    }

    async boleto(dados: Record<string, unknown>): Promise<unknown> {
        return this.request("post", "/transactions", {
            paymentMethod: "billet",
            ...dados,
        });
    }

    async cartao(dados: Record<string, unknown>): Promise<unknown> {
        return this.request("post", "/transactions", {
            paymentMethod: "creditCard",
            ...dados,
        });
    }

    async saldo(): Promise<unknown> {
        return this.request("get", "/user/finance/balance");
    }

    private async request(
        method: "get" | "post",
        url: string,
        data?: Record<string, unknown>
    ): Promise<unknown> {
        try {
            const response = await this.client.request({ method, url, data });
            return response.data;
        } catch (error) {
            throw createApiError("Silapay", error);
        }
    }
}

function createApiError(provider: string, error: unknown): Error {
    if (!isAxiosError(error) || typeof error.response === "undefined") {
        return error instanceof Error ? error : new Error(String(error));
    }

    const responseData = error.response?.data;
    const apiError = new Error(
        `${provider} API error: ${serializeResponseData(responseData)}`
    ) as ApiError;

    apiError.response = error.response;
    apiError.responseData = responseData;

    return apiError;
}

function serializeResponseData(data: unknown): string {
    if (typeof data === "string") {
        return data;
    }

    return JSON.stringify(data);
}

function isAxiosError(error: unknown): error is AxiosError {
    return axios.isAxiosError(error);
}
