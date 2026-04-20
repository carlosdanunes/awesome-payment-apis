using System;
using System.Collections.Generic;
using System.Text.Json;

async Task Main()
{
    var pay = new PaymentConnector("silapay");

    Console.WriteLine("Saldo:");
    Console.WriteLine(JsonSerializer.Serialize(await pay.Saldo()));

    Console.WriteLine("\nPIX CashIn:");
    Console.WriteLine(JsonSerializer.Serialize(await pay.Pix(new() {
        ["cashFlowType"] = "cashIn",
        ["value"]        = 50,
        ["postbackUrl"]  = "http://www.seusite.com.br/webhook/",
        ["description"]  = "Venda de produto",
        ["products"]     = new[] { new Dictionary<string,object> { ["name"]="AirPod",["price"]=1,["total"]=2,["quantity"]=2 } },
        ["customer"]     = new Dictionary<string,object> {
            ["name"]="Joao da Silva",["email"]="joao@email.com",
            ["phone"]="5511987654321",["document"]="12345678910",
            ["userAgent"]="CSharp/HttpClient",["street"]="Rua das Flores",
            ["complement"]="Apto 1",["postalCode"]="01234567",
            ["neighborhood"]="Centro",["state"]="SP",["country"]="Brasil"
        }
    })));

    Console.WriteLine("\nPIX CashOut:");
    Console.WriteLine(JsonSerializer.Serialize(await pay.Pix(new() {
        ["cashFlowType"] = "cashOut", ["value"] = 10,
        ["pixKey"] = "12345678910", ["pixKeyType"] = "cpf",
        ["postbackUrl"] = "http://www.seusite.com.br/webhook/",
        ["description"] = "Pagamento"
    })));

    Console.WriteLine("\nBoleto:");
    Console.WriteLine(JsonSerializer.Serialize(await pay.Boleto(new() {
        ["value"] = 150.50, ["dueDate"] = "2026-06-30T00:00:00.000Z",
        ["description"] = "Pagamento de servicos",
        ["daysAfterDueDateToRegistrationCancellation"] = 15,
        ["externalReference"] = "REF-001", ["installmentCount"] = 1,
        ["customer"] = new Dictionary<string,object> {
            ["name"]="Joao da Silva",["email"]="joao.silva@email.com",
            ["birthDate"]="1990-05-20",["phone"]="5511987654321",
            ["document"]="12345678901",["userAgent"]="CSharp/HttpClient",
            ["street"]="Rua das Flores",["addressNumber"]="123",
            ["complement"]="Apto 456",["postalCode"]="01234567",
            ["neighborhood"]="Centro",["city"]="Sao Paulo",["state"]="SP",["country"]="Brasil"
        },
        ["discount"] = new Dictionary<string,object> { ["value"]=10.50, ["dueDateLimitDays"]=5, ["type"]="Fixed" },
        ["interest"] = new Dictionary<string,object> { ["value"]=2.0 },
        ["fine"]     = new Dictionary<string,object> { ["value"]=2.0, ["type"]="Percentage" }
    })));

    Console.WriteLine("\nCartao:");
    Console.WriteLine(JsonSerializer.Serialize(await pay.Cartao(new() {
        ["value"]=100,["totalValue"]=110,["dueDate"]="2026-06-30",
        ["postbackUrl"]="http://www.seusite.com.br/webhook/",
        ["description"]="Venda de produto",
        ["installmentCount"]=2,["installmentValue"]=55,
        ["discount"]=new Dictionary<string,object>{["value"]=1,["dueDateLimitDays"]=2,["type"]="PERCENTAGE"},
        ["interest"]=new Dictionary<string,object>{["value"]=1},
        ["fine"]=new Dictionary<string,object>{["value"]=0,["type"]="PERCENTAGE"},
        ["postalService"]=false,
        ["callback"]=new Dictionary<string,object>{["successUrl"]="https://www.seusite.com/obrigado",["autoRedirect"]=false},
        ["customer"]=new Dictionary<string,object>{
            ["name"]="Joao da Silva",["email"]="joao@email.com",
            ["phone"]="11999999999",["document"]="12345678910",
            ["street"]="Rua das Flores",["complement"]="Apto 1",
            ["postalCode"]="01306010",["city"]="Sao Paulo",["state"]="SP",["country"]="Brasil"
        },
        ["card"]=new Dictionary<string,object>{
            ["cardHolderName"]="JOAO DA SILVA",["cardNumber"]="4242424242424242",
            ["expirationMonth"]="06",["expirationYear"]="42",["cvv"]="424"
        },
        ["cardOwner"]=new Dictionary<string,object>{["name"]="Joao da Silva",["document"]="12345678910"},
        ["products"]=new[]{new Dictionary<string,object>{["name"]="AirPod",["price"]=55,["total"]=110,["quantity"]=2}}
    })));

    // Trocar provider sem mudar a interface
    Console.WriteLine("\nTrocando provider sem mudar a interface:");
    var pay2 = new PaymentConnector("mercadopago");
    try {
        await pay2.Pix(new() { ["cashFlowType"]="cashIn", ["value"]=50, ["postbackUrl"]="http://www.seusite.com.br/webhook/" });
    } catch (NotImplementedException e) {
        Console.WriteLine(e.Message);
    }
}

try { await Main(); }
catch (Exception e) {
    Console.Error.WriteLine("Erro na execucao do exemplo:");
    Console.Error.WriteLine(e.Message);
}
