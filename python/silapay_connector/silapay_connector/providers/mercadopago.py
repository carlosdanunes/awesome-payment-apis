import os


class MercadoPagoProvider:
    def __init__(self, access_token=None):
        self.access_token = access_token or os.getenv("MERCADOPAGO_ACCESS_TOKEN")

    def pix(self, dados):
        raise NotImplementedError(
            "MercadoPago: metodo pix() ainda nao implementado. Consulte https://www.mercadopago.com.br/developers"
        )

    def boleto(self, dados):
        raise NotImplementedError(
            "MercadoPago: metodo boleto() ainda nao implementado. Consulte https://www.mercadopago.com.br/developers"
        )

    def cartao(self, dados):
        raise NotImplementedError(
            "MercadoPago: metodo cartao() ainda nao implementado. Consulte https://www.mercadopago.com.br/developers"
        )

    def saldo(self):
        raise NotImplementedError(
            "MercadoPago: metodo saldo() ainda nao implementado. Consulte https://www.mercadopago.com.br/developers"
        )
