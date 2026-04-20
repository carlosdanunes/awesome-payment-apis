using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PaymentConnector
{
    public interface IProvider
    {
        Task<Dictionary<string, object>> Pix(Dictionary<string, object> dados);
        Task<Dictionary<string, object>> Boleto(Dictionary<string, object> dados);
        Task<Dictionary<string, object>> Cartao(Dictionary<string, object> dados);
        Task<Dictionary<string, object>> Saldo();
    }

    private readonly IProvider _provider;
    public string ProviderName { get; }

    public PaymentConnector(string provider = "silapay", Dictionary<string, string> options = null)
    {
        options ??= new Dictionary<string, string>();
        ProviderName = provider;
        _provider = provider switch
        {
            "silapay"     => new SilapayProvider(options),
            "mercadopago" => new MercadoPagoProvider(options),
            "adyen"       => new AdyenProvider(options),
            _ => throw new ArgumentException(
                $"Provider \"{provider}\" nao suportado. Providers disponiveis: silapay, mercadopago, adyen"
            )
        };
    }

    public Task<Dictionary<string, object>> Pix(Dictionary<string, object> dados)    => _provider.Pix(dados);
    public Task<Dictionary<string, object>> Boleto(Dictionary<string, object> dados) => _provider.Boleto(dados);
    public Task<Dictionary<string, object>> Cartao(Dictionary<string, object> dados) => _provider.Cartao(dados);
    public Task<Dictionary<string, object>> Saldo()                                  => _provider.Saldo();
}
