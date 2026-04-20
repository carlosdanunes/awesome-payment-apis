using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AdyenProvider : PaymentConnector.IProvider
{
    private readonly string _apiKey;

    public AdyenProvider(Dictionary<string, string> options)
    {
        _apiKey = options.GetValueOrDefault("apiKey") ?? Environment.GetEnvironmentVariable("ADYEN_API_KEY") ?? "";
    }

    public Task<Dictionary<string, object>> Pix(Dictionary<string, object> dados) =>
        throw new NotImplementedException("Adyen: metodo Pix() ainda nao implementado. Consulte https://docs.adyen.com");

    public Task<Dictionary<string, object>> Boleto(Dictionary<string, object> dados) =>
        throw new NotImplementedException("Adyen: metodo Boleto() ainda nao implementado. Consulte https://docs.adyen.com");

    public Task<Dictionary<string, object>> Cartao(Dictionary<string, object> dados) =>
        throw new NotImplementedException("Adyen: metodo Cartao() ainda nao implementado. Consulte https://docs.adyen.com");

    public Task<Dictionary<string, object>> Saldo() =>
        throw new NotImplementedException("Adyen: metodo Saldo() ainda nao implementado. Consulte https://docs.adyen.com");
}
