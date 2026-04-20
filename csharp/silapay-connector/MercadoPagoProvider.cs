using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MercadoPagoProvider : PaymentConnector.IProvider
{
    private readonly string _accessToken;

    public MercadoPagoProvider(Dictionary<string, string> options)
    {
        _accessToken = options.GetValueOrDefault("accessToken") ?? Environment.GetEnvironmentVariable("MERCADOPAGO_ACCESS_TOKEN") ?? "";
    }

    public Task<Dictionary<string, object>> Pix(Dictionary<string, object> dados) =>
        throw new NotImplementedException("MercadoPago: metodo Pix() ainda nao implementado. Consulte https://www.mercadopago.com.br/developers");

    public Task<Dictionary<string, object>> Boleto(Dictionary<string, object> dados) =>
        throw new NotImplementedException("MercadoPago: metodo Boleto() ainda nao implementado. Consulte https://www.mercadopago.com.br/developers");

    public Task<Dictionary<string, object>> Cartao(Dictionary<string, object> dados) =>
        throw new NotImplementedException("MercadoPago: metodo Cartao() ainda nao implementado. Consulte https://www.mercadopago.com.br/developers");

    public Task<Dictionary<string, object>> Saldo() =>
        throw new NotImplementedException("MercadoPago: metodo Saldo() ainda nao implementado. Consulte https://www.mercadopago.com.br/developers");
}
