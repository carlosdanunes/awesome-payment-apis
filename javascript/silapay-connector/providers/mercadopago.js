class MercadoPagoProvider {
    constructor(options = {}) {
        this.accessToken = options.accessToken || process.env.MERCADOPAGO_ACCESS_TOKEN;
    }

    async pix() {
        throw new Error("MercadoPago: método pix() ainda não implementado. Consulte https://www.mercadopago.com.br/developers");
    }

    async boleto() {
        throw new Error("MercadoPago: método boleto() ainda não implementado. Consulte https://www.mercadopago.com.br/developers");
    }

    async cartao() {
        throw new Error("MercadoPago: método cartao() ainda não implementado. Consulte https://www.mercadopago.com.br/developers");
    }

    async saldo() {
        throw new Error("MercadoPago: método saldo() ainda não implementado. Consulte https://www.mercadopago.com.br/developers");
    }
}

module.exports = MercadoPagoProvider;
