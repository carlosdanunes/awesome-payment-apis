// Silapay — Cartao de Credito
// Requisitos: .NET 6+

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Cartao
{
    const string BaseUrl   = "https://api.silapay.pro/v1";
    static string ApiKey    = Environment.GetEnvironmentVariable("SILAPAY_API_KEY")    ?? "SUA_API_KEY_AQUI";
    static string SecretKey = Environment.GetEnvironmentVariable("SILAPAY_SECRET_KEY") ?? "SUA_SECRET_KEY_AQUI";
    static readonly HttpClient Http = new();

    static HttpRequestMessage BuildRequest(HttpMethod method, string path, string? body = null)
    {
        var req = new HttpRequestMessage(method, BaseUrl + path);
        req.Headers.Add("Accept", "application/json");
        req.Headers.Add("api-key", ApiKey);
        req.Headers.Add("secret-key", SecretKey);
        if (body != null) req.Content = new StringContent(body, Encoding.UTF8, "application/json");
        return req;
    }

    static async Task ConsultarSaldo()
    {
        var res = await Http.SendAsync(BuildRequest(HttpMethod.Get, "/user/finance/balance"));
        res.EnsureSuccessStatusCode();
        var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        Console.WriteLine($"Saldo disponivel: R$ {doc.RootElement.GetProperty("balance")}");
    }

    static async Task CobrarCartao()
    {
        var body = JsonSerializer.Serialize(new
        {
            paymentMethod    = "creditCard",
            value            = 100,
            totalValue       = 110,
            dueDate          = "2026-06-30",
            postbackUrl      = "http://www.seusite.com.br/webhook/",
            description      = "Venda de produto",
            installmentCount = 2,
            installmentValue = 55,
            discount  = new { value = 1, dueDateLimitDays = 2, type = "PERCENTAGE" },
            interest  = new { value = 1 },
            fine      = new { value = 0, type = "PERCENTAGE" },
            postalService = false,
            callback  = new { successUrl = "https://www.seusite.com/obrigado", autoRedirect = false },
            customer = new
            {
                name = "Joao da Silva", email = "joao@email.com", phone = "11999999999",
                document = "12345678910", userAgent = "C#/HttpClient",
                street = "Rua das Flores", complement = "Apto 1", postalCode = "01306010",
                neighborhood = "Centro", city = "Sao Paulo", state = "SP", country = "Brasil"
            },
            card = new
            {
                cardHolderName = "JOAO DA SILVA", cardNumber = "4242424242424242",
                expirationMonth = "06", expirationYear = "42", cvv = "424"
            },
            cardOwner = new { name = "Joao da Silva", document = "12345678910" },
            products = new[] { new { name = "AirPod", price = 55, total = 110, quantity = 2,
                                     image = "https://www.seusite.com/image.png" } }
        });

        var res = await Http.SendAsync(BuildRequest(HttpMethod.Post, "/transactions", body));
        res.EnsureSuccessStatusCode();

        var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync()).RootElement;
        Console.WriteLine("Cartao de credito cobrado com sucesso!");
        Console.WriteLine($"  Status          : {doc.GetProperty("status")}");
        Console.WriteLine($"  Valor           : R$ {doc.GetProperty("value")}");
        Console.WriteLine($"  Ultimos digitos : {doc.GetProperty("cardLastDigits")}");
        Console.WriteLine($"  Autorizacao     : {doc.GetProperty("authorizationCode")}");
        Console.WriteLine($"  Criado em       : {doc.GetProperty("createdAt")}");
    }

    static void HandleWebhook(string json)
    {
        var doc    = JsonDocument.Parse(json).RootElement;
        var status = doc.GetProperty("status").GetString()!;
        Console.WriteLine($"Webhook recebido — ID: {doc.GetProperty("id")}");
        Console.WriteLine($"  Status  : {status}");
        Console.WriteLine(status switch
        {
            "CONFIRMED" or "PAID" or "COMPLETED" => "  -> Pagamento aprovado — liberar pedido/servico",
            "FAILED" or "REFUSED" or "REJECTED"  => "  -> Cartao recusado/falhou",
            "CANCELED"                            => "  -> Transacao cancelada",
            "REFUNDED" or "PARTIALLY_REFUNDED"   => "  -> Reembolso efetuado",
            "CHARGEDBACK"                         => "  -> Chargeback confirmado",
            _                                     => $"  -> Status: {status}"
        });
    }

    static async Task Main()
    {
        await ConsultarSaldo();
        Console.WriteLine();
        await CobrarCartao();
        Console.WriteLine();
        HandleWebhook("""{"id":"evt_a1b2","status":"CONFIRMED","data":{"value":"100","txId":"uuid","method":"creditCard"}}""");
    }
}
