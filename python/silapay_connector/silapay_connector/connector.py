from .providers.adyen import AdyenProvider
from .providers.mercadopago import MercadoPagoProvider
from .providers.silapay import SilapayProvider

PROVIDERS = {
    "silapay": SilapayProvider,
    "mercadopago": MercadoPagoProvider,
    "adyen": AdyenProvider,
}


class PaymentConnector:
    def __init__(self, provider="silapay", **options):
        provider_class = PROVIDERS.get(provider)

        if provider_class is None:
            raise ValueError(
                f'Provider "{provider}" nao suportado. Providers disponiveis: silapay, mercadopago, adyen'
            )

        self.provider_name = provider
        self.provider = provider_class(**options)

    def pix(self, dados):
        return self.provider.pix(dados)

    def boleto(self, dados):
        return self.provider.boleto(dados)

    def cartao(self, dados):
        return self.provider.cartao(dados)

    def saldo(self):
        return self.provider.saldo()
