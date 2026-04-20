"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.SilapayProvider = void 0;
const axios_1 = __importDefault(require("axios"));
const BASE_URL = "https://api.silapay.pro/v1";
class SilapayProvider {
    constructor(options = {}) {
        const apiKey = options.apiKey || process.env.SILAPAY_API_KEY;
        const secretKey = options.secretKey || process.env.SILAPAY_SECRET_KEY;
        if (!apiKey || !secretKey) {
            throw new Error("Silapay: defina SILAPAY_API_KEY e SILAPAY_SECRET_KEY no ambiente.");
        }
        this.client = axios_1.default.create({
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
    async pix(dados) {
        return this.request("post", "/transactions", {
            paymentMethod: "pix",
            ...dados,
        });
    }
    async boleto(dados) {
        return this.request("post", "/transactions", {
            paymentMethod: "billet",
            ...dados,
        });
    }
    async cartao(dados) {
        return this.request("post", "/transactions", {
            paymentMethod: "creditCard",
            ...dados,
        });
    }
    async saldo() {
        return this.request("get", "/user/finance/balance");
    }
    async request(method, url, data) {
        try {
            const response = await this.client.request({ method, url, data });
            return response.data;
        }
        catch (error) {
            throw createApiError("Silapay", error);
        }
    }
}
exports.SilapayProvider = SilapayProvider;
function createApiError(provider, error) {
    if (!isAxiosError(error) || typeof error.response === "undefined") {
        return error instanceof Error ? error : new Error(String(error));
    }
    const responseData = error.response?.data;
    const apiError = new Error(`${provider} API error: ${serializeResponseData(responseData)}`);
    apiError.response = error.response;
    apiError.responseData = responseData;
    return apiError;
}
function serializeResponseData(data) {
    if (typeof data === "string") {
        return data;
    }
    return JSON.stringify(data);
}
function isAxiosError(error) {
    return axios_1.default.isAxiosError(error);
}
