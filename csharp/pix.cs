// Silapay — PIX CashIn e CashOut
// Documentacao: https://api.silapay.pro/v1/transactions
//
// Requisitos:
//   .NET 6+  (System.Net.Http e System.Text.Json inclusos)
//
// Variaveis de ambiente:
//   SILAPAY_API_KEY=sua_api_key
//   SILAPAY_SECRET_KEY=sua_secret_key

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Pix
{
    const string BaseUrl   = "https://api.silapay.pro/v1";
    static string ApiKey    = Environment.GetEnvironmentVariable("SILAPAY_API_KEY")    ?? "SUA_API_KEY_AQUI";
    static string SecretKey = Environment.GetEnvironmentVariable("SILAPAY_SECRET_KEY") ?? "SUA_SECRET_KEY_AQUI";

    static readonly HttpClient Http = new();

    static HttpRequestMessage BuildRequest(HttpMethod method, string path, string? body = null)
    {
        var req = new HttpRequestMessage(method, BaseUrl + path);
        req.Headers.Add("Accept",      "application/json");
        req.Headers.Add("api-key",     ApiKey);
        req.Headers.Add("secret-key",  SecretKey);
        if (body != null)
            req.Content = new StringContent(body, Encoding.UTF8, "application/json");
        return req;
    }

    // ─── 1. Consultar Saldo ───────────────────────────────────────────────────

    static async Task ConsultarSaldo()
    {
        var res = await Http.SendAsync(BuildRequest(HttpMethod.Get, "/user/finance/balance"));
        res.EnsureSuccessStatusCode();
        var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        Console.WriteLine($"Saldo disponivel: R$ {doc.RootElement.GetProperty("balance")}");
    }

    // ─── 2. PIX CashIn — Gerar cobranca ──────────────────────────────────────

    static async Task PixCashIn()
    {
        var body = JsonSerializer.Serialize(new
        {
            paymentMethod = "pix",
            cashFlowType  = "cashIn",
            value         = 50.00,
            postbackUrl   = "http://www.seusite.com.br/webhook/",
            description   = "Venda de produto",
            products = new[] { new { name = "AirPod", price = 1, total = 2, quantity = 2 } },
            customer = new
            {
                name         = "Joao da Silva",
                email        = "joao@email.com",
                phone        = "5511987654321",
                document     = "12345678910",
                userAgent    = "C#/HttpClient",
                street       = "Rua das Flores",
                complement   = "Apto 1",
                postalCode   = "01234567",
                neighborhood = "Centro",
                state        = "SP",
                country      = "Brasil"
            }
        });

        var res = await Http.SendAsync(BuildRequest(HttpMethod.Post, "/transactions", body));
        res.EnsureSuccessStatusCode();

        var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync()).RootElement;
        Console.WriteLine("PIX CashIn gerado com sucesso!");
        Console.WriteLine($"  Transacao ID     : {doc.GetProperty("id")}");
        Console.WriteLine($"  Status           : {doc.GetProperty("status")}");
        Console.WriteLine($"  Pix Copia e Cola : {doc.GetProperty("pixCopiaECola")}");
        Console.WriteLine($"  Criado em        : {doc.GetProperty("createdAt")}");
    }

    // ─── 3. PIX CashOut — Efetuar pagamento ──────────────────────────────────

    static async Task PixCashOut()
    {
        var body = JsonSerializer.Serialize(new
        {
            paymentMethod = "pix",
            cashFlowType  = "cashOut",
            value         = 1,
            pixKey        = "29242661066",
            pixKeyType    = "cpf",   // "cpf" | "cnpj" | "phone" | "evp"
            postbackUrl   = "http://www.seusite.com.br/webhook/",
            description   = "Pagamento"
        });

        var res = await Http.SendAsync(BuildRequest(HttpMethod.Post, "/transactions", body));
        res.EnsureSuccessStatusCode();

        var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync()).RootElement;
        Console.WriteLine("PIX CashOut enviado com sucesso!");
        Console.WriteLine($"  Transacao ID : {doc.GetProperty("id")}");
        Console.WriteLine($"  Status       : {doc.GetProperty("status")}");
        Console.WriteLine($"  Chave PIX    : {doc.GetProperty("pixKey")}");
        Console.WriteLine($"  Criado em    : {doc.GetProperty("createdAt")}");
    }

    // ─── Webhook ──────────────────────────────────────────────────────────────

    static void HandleWebhook(string json)
    {
        var doc    = JsonDocument.Parse(json).RootElement;
        var status = doc.GetProperty("status").GetString()!;
        Console.WriteLine($"Webhook recebido — ID: {doc.GetProperty("id")}");
        Console.WriteLine($"  Status  : {status}");

        Console.WriteLine(status switch
        {
            "PAID" or "LIQUIDATED" or "COMPLETED"    => "  -> Pagamento confirmado — liberar pedido/servico",
            "OVERDUE"                                  => "  -> PIX expirado — notificar cliente",
            "FAILED" or "REFUSED" or "REJECTED"       => "  -> Transacao recusada/falhou",
            "REFUNDED" or "PARTIALLY_REFUNDED"        => "  -> Reembolso efetuado",
            "CHARGEDBACK"                              => "  -> Chargeback confirmado",
            _                                          => $"  -> Status: {status}"
        });
    }

    static async Task Main()
    {
        await ConsultarSaldo();
        Console.WriteLine();
        await PixCashIn();
        Console.WriteLine();
        await PixCashOut();
        Console.WriteLine();
        HandleWebhook("""{"id":"evt_e419ceea","status":"LIQUIDATED","data":{"value":"50.0","txId":"5ffa9f6f","method":"pix"}}""");
    }
}
