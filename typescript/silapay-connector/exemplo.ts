import { PaymentConnector } from "./index";

async function main(): Promise<void> {
    const pay = new PaymentConnector({ provider: "silapay" });

    console.log("Saldo:");
    console.log(await pay.saldo());

    console.log("\nPIX CashIn:");
    console.log(
        await pay.pix({
            cashFlowType: "cashIn",
            value: 50,
            postbackUrl: "http://www.seusite.com.br/webhook/",
            description: "Venda de produto",
            products: [{ name: "AirPod", price: 1, total: 2, quantity: 2 }],
            customer: {
                name: "João da Silva",
                email: "joao@email.com",
                phone: "5511987654321",
                document: "12345678910",
                userAgent: "Mozilla/5.0",
                street: "Rua das Flores",
                complement: "Apto 1",
                postalCode: "01234567",
                neighborhood: "Centro",
                state: "SP",
                country: "Brasil",
            },
        })
    );

    console.log("\nPIX CashOut:");
    console.log(
        await pay.pix({
            cashFlowType: "cashOut",
            value: 10,
            pixKey: "12345678910",
            pixKeyType: "cpf",
            postbackUrl: "http://www.seusite.com.br/webhook/",
            description: "Pagamento",
        })
    );

    console.log("\nBoleto:");
    console.log(
        await pay.boleto({
            value: 150.5,
            dueDate: "2026-06-30T00:00:00.000Z",
            description: "Pagamento de serviços",
            daysAfterDueDateToRegistrationCancellation: 15,
            externalReference: "REF-001",
            installmentCount: 1,
            customer: {
                name: "João da Silva",
                email: "joao.silva@email.com",
                birthDate: "1990-05-20",
                phone: "5511987654321",
                document: "12345678901",
                userAgent: "Mozilla/5.0",
                street: "Rua das Flores",
                addressNumber: "123",
                complement: "Apto 456",
                postalCode: "01234567",
                neighborhood: "Centro",
                city: "São Paulo",
                state: "SP",
                country: "Brasil",
            },
            discount: { value: 10.5, dueDateLimitDays: 5, type: "Fixed" },
            interest: { value: 2 },
            fine: { value: 2, type: "Percentage" },
        })
    );

    console.log("\nCartão:");
    console.log(
        await pay.cartao({
            value: 100,
            totalValue: 110,
            dueDate: "2026-06-30",
            postbackUrl: "http://www.seusite.com.br/webhook/",
            description: "Venda de produto",
            installmentCount: 2,
            installmentValue: 55,
            discount: { value: 1, dueDateLimitDays: 2, type: "PERCENTAGE" },
            interest: { value: 1 },
            fine: { value: 0, type: "PERCENTAGE" },
            postalService: false,
            callback: {
                successUrl: "https://www.seusite.com/obrigado",
                autoRedirect: false,
            },
            customer: {
                name: "João da Silva",
                email: "joao@email.com",
                phone: "11999999999",
                document: "12345678910",
                street: "Rua das Flores",
                complement: "Apto 1",
                postalCode: "01306010",
                city: "São Paulo",
                state: "SP",
                country: "Brasil",
            },
            card: {
                cardHolderName: "JOAO DA SILVA",
                cardNumber: "4242424242424242",
                expirationMonth: "06",
                expirationYear: "42",
                cvv: "424",
            },
            cardOwner: {
                name: "João da Silva",
                document: "12345678910",
            },
            products: [{ name: "AirPod", price: 55, total: 110, quantity: 2 }],
        })
    );

    const pay2 = new PaymentConnector({ provider: "mercadopago" });

    console.log("\nTrocando provider sem mudar a interface:");
    try {
        await pay2.pix({
            cashFlowType: "cashIn",
            value: 50,
            postbackUrl: "http://www.seusite.com.br/webhook/",
        });
    } catch (error) {
        console.log(error instanceof Error ? error.message : String(error));
    }
}

main().catch((error) => {
    const responseData =
        typeof error === "object" && error !== null && "responseData" in error
            ? (error as { responseData?: unknown }).responseData
            : undefined;

    console.error("Erro na execução do exemplo:");
    console.error(responseData ?? (error instanceof Error ? error.message : String(error)));
    process.exit(1);
});
