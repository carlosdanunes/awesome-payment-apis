' Silapay — Cartao de Credito
' Documentacao: https://api.silapay.pro/v1/transactions
'
' Requisitos:
'   .NET 6+ (System.Net.Http e System.Text.Json inclusos)
'
' Variaveis de ambiente:
'   SILAPAY_API_KEY=sua_api_key
'   SILAPAY_SECRET_KEY=sua_secret_key
'
' Compilar: vbc cartao.vb -r:System.Net.Http.dll

Imports System
Imports System.Net.Http
Imports System.Text
Imports System.Text.Json
Imports System.Threading.Tasks

Module Cartao

    Const BaseUrl As String = "https://api.silapay.pro/v1"
    ReadOnly ApiKey As String = If(Environment.GetEnvironmentVariable("SILAPAY_API_KEY"), "SUA_API_KEY_AQUI")
    ReadOnly SecretKey As String = If(Environment.GetEnvironmentVariable("SILAPAY_SECRET_KEY"), "SUA_SECRET_KEY_AQUI")
    ReadOnly Http As New HttpClient()

    Function BuildRequest(method As HttpMethod, path As String, Optional body As String = Nothing) As HttpRequestMessage
        Dim req As New HttpRequestMessage(method, BaseUrl & path)
        req.Headers.Add("Accept",     "application/json")
        req.Headers.Add("api-key",    ApiKey)
        req.Headers.Add("secret-key", SecretKey)
        If body IsNot Nothing Then
            req.Content = New StringContent(body, Encoding.UTF8, "application/json")
        End If
        Return req
    End Function

    ' ─── 1. Consultar Saldo ──────────────────────────────────────────────────

    Async Function ConsultarSaldo() As Task
        Dim res = Await Http.SendAsync(BuildRequest(HttpMethod.Get, "/user/finance/balance"))
        res.EnsureSuccessStatusCode()
        Dim doc = JsonDocument.Parse(Await res.Content.ReadAsStringAsync())
        Console.WriteLine("Saldo disponivel: R$ " & doc.RootElement.GetProperty("balance").ToString())
    End Function

    ' ─── 2. Cobrar Cartao de Credito ─────────────────────────────────────────

    Async Function CobrarCartao() As Task
        Dim body As String = "{" &
            """paymentMethod"":""creditCard""," &
            """value"":100," &
            """totalValue"":110," &
            """dueDate"":""2026-06-30""," &
            """postbackUrl"":""http://www.seusite.com.br/webhook/""," &
            """description"":""Venda de produto""," &
            """installmentCount"":2," &
            """installmentValue"":55," &
            """postalService"":false," &
            """discount"":{""value"":1,""dueDateLimitDays"":2,""type"":""PERCENTAGE""}," &
            """interest"":{""value"":1}," &
            """fine"":{""value"":0,""type"":""PERCENTAGE""}," &
            """callback"":{""successUrl"":""https://www.seusite.com/obrigado"",""autoRedirect"":false}," &
            """customer"":{" &
                """name"":""Joao da Silva""," &
                """email"":""joao@email.com""," &
                """phone"":""11999999999""," &
                """document"":""12345678910""," &
                """userAgent"":""VB.NET/HttpClient""," &
                """street"":""Rua das Flores""," &
                """complement"":""Apto 1""," &
                """postalCode"":""01306010""," &
                """neighborhood"":""Centro""," &
                """city"":""Sao Paulo""," &
                """state"":""SP""," &
                """country"":""Brasil""}," &
            """card"":{" &
                """cardHolderName"":""JOAO DA SILVA""," &
                """cardNumber"":""4242424242424242""," &
                """expirationMonth"":""06""," &
                """expirationYear"":""42""," &
                """cvv"":""424""}," &
            """cardOwner"":{""name"":""Joao da Silva"",""document"":""12345678910""}," &
            """products"":[{""name"":""AirPod"",""price"":55,""total"":110,""quantity"":2," &
                           """image"":""https://www.seusite.com/image.png""}]}"

        Dim res = Await Http.SendAsync(BuildRequest(HttpMethod.Post, "/transactions", body))
        res.EnsureSuccessStatusCode()
        Dim doc = JsonDocument.Parse(Await res.Content.ReadAsStringAsync()).RootElement
        Console.WriteLine("Cartao de credito cobrado com sucesso!")
        Console.WriteLine("  Status          : " & doc.GetProperty("status").ToString())
        Console.WriteLine("  Valor           : R$ " & doc.GetProperty("value").ToString())
        Console.WriteLine("  Ultimos digitos : " & doc.GetProperty("cardLastDigits").ToString())
        Console.WriteLine("  Autorizacao     : " & doc.GetProperty("authorizationCode").ToString())
        Console.WriteLine("  Criado em       : " & doc.GetProperty("createdAt").ToString())
    End Function

    ' ─── Webhook ─────────────────────────────────────────────────────────────

    Sub HandleWebhook(json As String)
        Dim doc    = JsonDocument.Parse(json).RootElement
        Dim status = doc.GetProperty("status").GetString()
        Console.WriteLine("Webhook recebido — ID: " & doc.GetProperty("id").ToString())
        Console.WriteLine("  Status  : " & status)
        Select Case status
            Case "CONFIRMED", "PAID", "COMPLETED"
                Console.WriteLine("  -> Pagamento aprovado — liberar pedido/servico")
            Case "FAILED", "REFUSED", "REJECTED"
                Console.WriteLine("  -> Cartao recusado/falhou")
            Case "CANCELED"
                Console.WriteLine("  -> Transacao cancelada")
            Case "REFUNDED", "PARTIALLY_REFUNDED"
                Console.WriteLine("  -> Reembolso efetuado")
            Case "CHARGEDBACK"
                Console.WriteLine("  -> Chargeback confirmado")
            Case Else
                Console.WriteLine("  -> Status: " & status)
        End Select
    End Sub

    ' ─── Main ────────────────────────────────────────────────────────────────

    Sub Main()
        ConsultarSaldo().GetAwaiter().GetResult()
        Console.WriteLine()
        CobrarCartao().GetAwaiter().GetResult()
        Console.WriteLine()
        HandleWebhook("{""id"":""evt_a1b2"",""status"":""CONFIRMED""," &
                      """data"":{""value"":""100"",""txId"":""uuid"",""method"":""creditCard""}}")
    End Sub

End Module
