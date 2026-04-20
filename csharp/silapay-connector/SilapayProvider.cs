using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class SilapayProvider : PaymentConnector.IProvider
{
    private const string DefaultBaseUrl = "https://api.silapay.pro/v1";
    private static readonly HttpClient Http = new();

    private readonly string _baseUrl;
    private readonly string _apiKey;
    private readonly string _secretKey;

    public SilapayProvider(Dictionary<string, string> options)
    {
        _apiKey    = options.GetValueOrDefault("apiKey")    ?? Environment.GetEnvironmentVariable("SILAPAY_API_KEY")    ?? "";
        _secretKey = options.GetValueOrDefault("secretKey") ?? Environment.GetEnvironmentVariable("SILAPAY_SECRET_KEY") ?? "";

        if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_secretKey))
            throw new InvalidOperationException("Silapay: defina SILAPAY_API_KEY e SILAPAY_SECRET_KEY no ambiente.");

        _baseUrl = (options.GetValueOrDefault("baseUrl") ?? DefaultBaseUrl).TrimEnd('/');
    }

    public Task<Dictionary<string, object>> Pix(Dictionary<string, object> dados)
    {
        dados["paymentMethod"] = "pix";
        return Request(HttpMethod.Post, "/transactions", dados);
    }

    public Task<Dictionary<string, object>> Boleto(Dictionary<string, object> dados)
    {
        dados["paymentMethod"] = "billet";
        return Request(HttpMethod.Post, "/transactions", dados);
    }

    public Task<Dictionary<string, object>> Cartao(Dictionary<string, object> dados)
    {
        dados["paymentMethod"] = "creditCard";
        return Request(HttpMethod.Post, "/transactions", dados);
    }

    public Task<Dictionary<string, object>> Saldo() =>
        Request(HttpMethod.Get, "/user/finance/balance", null);

    private async Task<Dictionary<string, object>> Request(HttpMethod method, string path, Dictionary<string, object> body)
    {
        var req = new HttpRequestMessage(method, _baseUrl + path);
        req.Headers.Add("Accept",     "application/json");
        req.Headers.Add("api-key",    _apiKey);
        req.Headers.Add("secret-key", _secretKey);

        if (body != null)
            req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        var res = await Http.SendAsync(req);
        var json = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException($"Silapay API error ({(int)res.StatusCode}): {json}");

        return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new();
    }
}
