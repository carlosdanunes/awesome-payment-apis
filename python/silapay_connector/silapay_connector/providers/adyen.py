import os


class AdyenProvider:
    def __init__(self, api_key=None, merchant_account=None):
        self.api_key = api_key or os.getenv("ADYEN_API_KEY")
        self.merchant_account = merchant_account or os.getenv("ADYEN_MERCHANT_ACCOUNT")

    def pix(self, dados):
        raise NotImplementedError(
            "Adyen: metodo pix() ainda nao implementado. Consulte https://docs.adyen.com/"
        )

    def boleto(self, dados):
        raise NotImplementedError(
            "Adyen: metodo boleto() ainda nao implementado. Consulte https://docs.adyen.com/"
        )

    def cartao(self, dados):
        raise NotImplementedError(
            "Adyen: metodo cartao() ainda nao implementado. Consulte https://docs.adyen.com/"
        )

    def saldo(self):
        raise NotImplementedError(
            "Adyen: metodo saldo() ainda nao implementado. Consulte https://docs.adyen.com/"
        )
