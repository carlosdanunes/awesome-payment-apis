// Silapay — Boleto Bancario
// Requisitos: .NET 6+

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Boleto
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

    static async Task CriarBoleto()
    {
        var body = JsonSerializer.Serialize(new
        {
            paymentMethod = "billet",
            value         = 150.50,
            dueDate       = "2026-06-30T00:00:00.000Z",
            description   = "Pagamento de servicos",
            daysAfterDueDateToRegistrationCancellation = 15,
            externalReference = "REF-001",
            installmentCount  = 1,
            customer = new
            {
                name          = "Joao da Silva",
                email         = "joao.silva@email.com",
                birthDate     = "1990-05-20",
                phone         = "5511987654321",
                document      = "12345678901",
                userAgent     = "C#/HttpClient",
                street        = "Rua das Flores",
                addressNumber = "123",
                complement    = "Apto 456",
                postalCode    = "01234567",
                neighborhood  = "Centro",
                city          = "Sao Paulo",
                state         = "SP",
                country       = "Brasil"
            },
            discount = new { value = 10.50, dueDateLimitDays = 5, type = "Fixed" },
            interest = new { value = 2.0 },
            fine     = new { value = 2.0, type = "Percentage" }
        });

        var res = await Http.SendAsync(BuildRequest(HttpMethod.Post, "/transactions", body));
        res.EnsureSuccessStatusCode();

        var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync()).RootElement;
        var tx  = doc.GetProperty("transaction");
        Console.WriteLine("Boleto criado com sucesso!");
        Console.WriteLine($"  Transacao ID     : {tx.GetProperty("transactionId")}");
        Console.WriteLine($"  Status           : {tx.GetProperty("status")}");
        Console.WriteLine($"  Codigo de barras : {tx.GetProperty("barCode")}");
        Console.WriteLine($"  Criado em        : {tx.GetProperty("createdAt")}");
    }

    static void HandleWebhook(string json)
    {
        var doc    = JsonDocument.Parse(json).RootElement;
        var status = doc.GetProperty("status").GetString()!;
        Console.WriteLine($"Webhook recebido — ID: {doc.GetProperty("id")}");
        Console.WriteLine($"  Status  : {status}");
        Console.WriteLine(status switch
        {
            "PAID" or "LIQUIDATED" or "COMPLETED" => "  -> Pagamento confirmado — liberar pedido/servico",
            "OVERDUE"                               => "  -> Boleto vencido — notificar cliente",
            "CANCELED" or "DELETED"                => "  -> Boleto cancelado",
            "FAILED" or "REFUSED" or "REJECTED"    => "  -> Transacao recusada/falhou",
            "REFUNDED" or "PARTIALLY_REFUNDED"     => "  -> Reembolso efetuado",
            "CHARGEDBACK"                           => "  -> Chargeback confirmado",
            _                                       => $"  -> Status: {status}"
        });
    }

    static async Task Main()
    {
        await ConsultarSaldo();
        Console.WriteLine();
        await CriarBoleto();
        Console.WriteLine();
        HandleWebhook("""{"id":"evt_e419ceea","status":"LIQUIDATED","data":{"value":"150.5","txId":"3f6f364b","method":"billet"}}""");
    }
}
