from silapay_connector import PaymentConnector


def main():
    pay = PaymentConnector(provider="silapay")

    print("Saldo:")
    print(pay.saldo())

    print("\nPIX CashIn:")
    print(
        pay.pix(
            {
                "cashFlowType": "cashIn",
                "value": 50,
                "postbackUrl": "http://www.seusite.com.br/webhook/",
                "description": "Venda de produto",
                "products": [{"name": "AirPod", "price": 1, "total": 2, "quantity": 2}],
                "customer": {
                    "name": "Joao da Silva",
                    "email": "joao@email.com",
                    "phone": "5511987654321",
                    "document": "12345678910",
                    "userAgent": "Mozilla/5.0",
                    "street": "Rua das Flores",
                    "complement": "Apto 1",
                    "postalCode": "01234567",
                    "neighborhood": "Centro",
                    "state": "SP",
                    "country": "Brasil",
                },
            }
        )
    )

    print("\nPIX CashOut:")
    print(
        pay.pix(
            {
                "cashFlowType": "cashOut",
                "value": 10,
                "pixKey": "12345678910",
                "pixKeyType": "cpf",
                "postbackUrl": "http://www.seusite.com.br/webhook/",
                "description": "Pagamento",
            }
        )
    )

    print("\nBoleto:")
    print(
        pay.boleto(
            {
                "value": 150.50,
                "dueDate": "2026-06-30T00:00:00.000Z",
                "description": "Pagamento de servicos",
                "daysAfterDueDateToRegistrationCancellation": 15,
                "externalReference": "REF-001",
                "installmentCount": 1,
                "customer": {
                    "name": "Joao da Silva",
                    "email": "joao.silva@email.com",
                    "birthDate": "1990-05-20",
                    "phone": "5511987654321",
                    "document": "12345678901",
                    "userAgent": "Mozilla/5.0",
                    "street": "Rua das Flores",
                    "addressNumber": "123",
                    "complement": "Apto 456",
                    "postalCode": "01234567",
                    "neighborhood": "Centro",
                    "city": "Sao Paulo",
                    "state": "SP",
                    "country": "Brasil",
                },
                "discount": {"value": 10.50, "dueDateLimitDays": 5, "type": "Fixed"},
                "interest": {"value": 2.0},
                "fine": {"value": 2.0, "type": "Percentage"},
            }
        )
    )

    print("\nCartao:")
    print(
        pay.cartao(
            {
                "value": 100,
                "totalValue": 110,
                "dueDate": "2026-06-30",
                "postbackUrl": "http://www.seusite.com.br/webhook/",
                "description": "Venda de produto",
                "installmentCount": 2,
                "installmentValue": 55,
                "discount": {
                    "value": 1,
                    "dueDateLimitDays": 2,
                    "type": "PERCENTAGE",
                },
                "interest": {"value": 1},
                "fine": {"value": 0, "type": "PERCENTAGE"},
                "postalService": False,
                "callback": {
                    "successUrl": "https://www.seusite.com/obrigado",
                    "autoRedirect": False,
                },
                "customer": {
                    "name": "Joao da Silva",
                    "email": "joao@email.com",
                    "phone": "11999999999",
                    "document": "12345678910",
                    "street": "Rua das Flores",
                    "complement": "Apto 1",
                    "postalCode": "01306010",
                    "city": "Sao Paulo",
                    "state": "SP",
                    "country": "Brasil",
                },
                "card": {
                    "cardHolderName": "JOAO DA SILVA",
                    "cardNumber": "4242424242424242",
                    "expirationMonth": "06",
                    "expirationYear": "42",
                    "cvv": "424",
                },
                "cardOwner": {
                    "name": "Joao da Silva",
                    "document": "12345678910",
                },
                "products": [{"name": "AirPod", "price": 55, "total": 110, "quantity": 2}],
            }
        )
    )

    pay2 = PaymentConnector(provider="mercadopago")

    print("\nTrocando provider sem mudar a interface:")
    try:
        pay2.pix(
            {
                "cashFlowType": "cashIn",
                "value": 50,
                "postbackUrl": "http://www.seusite.com.br/webhook/",
            }
        )
    except NotImplementedError as err:
        print(err)


if __name__ == "__main__":
    try:
        main()
    except Exception as err:
        print("Erro na execucao do exemplo:")
        print(getattr(err, "response_data", None) or str(err))
        raise
