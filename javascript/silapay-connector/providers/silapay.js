const axios = require("axios");

const BASE_URL = "https://api.silapay.pro/v1";

class SilapayProvider {
    constructor(options = {}) {
        const apiKey = options.apiKey || process.env.SILAPAY_API_KEY;
        const secretKey = options.secretKey || process.env.SILAPAY_SECRET_KEY;

        if (!apiKey || !secretKey) {
            throw new Error("Silapay: defina SILAPAY_API_KEY e SILAPAY_SECRET_KEY no ambiente.");
        }

        this.client = axios.create({
            baseURL: options.baseURL || BASE_URL,
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
        } catch (err) {
            throw createApiError("Silapay", err);
        }
    }
}

function createApiError(provider, err) {
    if (!err.response?.data) {
        return err;
    }

    const error = new Error(`${provider} API error: ${serializeErrorData(err.response.data)}`);
    error.response = err.response;
    error.cause = err;

    return error;
}

function serializeErrorData(data) {
    return typeof data === "string" ? data : JSON.stringify(data);
}

module.exports = SilapayProvider;
