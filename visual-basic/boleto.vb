' Silapay — Boleto Bancario
' Documentacao: https://api.silapay.pro/v1/transactions
'
' Requisitos:
'   .NET 6+ (System.Net.Http e System.Text.Json inclusos)
'
' Variaveis de ambiente:
'   SILAPAY_API_KEY=sua_api_key
'   SILAPAY_SECRET_KEY=sua_secret_key
'
' Compilar: vbc boleto.vb -r:System.Net.Http.dll

Imports System
Imports System.Net.Http
Imports System.Text
Imports System.Text.Json
Imports System.Threading.Tasks

Module Boleto

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

    ' ─── 2. Criar Boleto ─────────────────────────────────────────────────────

    Async Function CriarBoleto() As Task
        Dim body As String = "{" &
            """paymentMethod"":""billet""," &
            """value"":150.50," &
            """dueDate"":""2026-06-30T00:00:00.000Z""," &
            """description"":""Pagamento de servicos""," &
            """daysAfterDueDateToRegistrationCancellation"":15," &
            """externalReference"":""REF-001""," &
            """installmentCount"":1," &
            """customer"":{" &
                """name"":""Joao da Silva""," &
                """email"":""joao.silva@email.com""," &
                """birthDate"":""1990-05-20""," &
                """phone"":""5511987654321""," &
                """document"":""12345678901""," &
                """userAgent"":""VB.NET/HttpClient""," &
                """street"":""Rua das Flores""," &
                """addressNumber"":""123""," &
                """complement"":""Apto 456""," &
                """postalCode"":""01234567""," &
                """neighborhood"":""Centro""," &
                """city"":""Sao Paulo""," &
                """state"":""SP""," &
                """country"":""Brasil""}," &
            """discount"":{""value"":10.50,""dueDateLimitDays"":5,""type"":""Fixed""}," &
            """interest"":{""value"":2.0}," &
            """fine"":{""value"":2.0,""type"":""Percentage""}}"

        Dim res = Await Http.SendAsync(BuildRequest(HttpMethod.Post, "/transactions", body))
        res.EnsureSuccessStatusCode()
        Dim root = JsonDocument.Parse(Await res.Content.ReadAsStringAsync()).RootElement
        Console.WriteLine("Boleto criado com sucesso!")
        Dim tx = root.GetProperty("transaction")
        Console.WriteLine("  Transacao ID      : " & tx.GetProperty("transactionId").ToString())
        Console.WriteLine("  Status            : " & tx.GetProperty("status").ToString())
        Console.WriteLine("  Codigo de barras  : " & tx.GetProperty("barCode").ToString())
        Console.WriteLine("  Criado em         : " & tx.GetProperty("createdAt").ToString())
    End Function

    ' ─── Webhook ─────────────────────────────────────────────────────────────

    Sub HandleWebhook(json As String)
        Dim doc    = JsonDocument.Parse(json).RootElement
        Dim status = doc.GetProperty("status").GetString()
        Console.WriteLine("Webhook recebido — ID: " & doc.GetProperty("id").ToString())
        Console.WriteLine("  Status  : " & status)
        Select Case status
            Case "PAID", "LIQUIDATED", "COMPLETED"
                Console.WriteLine("  -> Pagamento confirmado — liberar pedido/servico")
            Case "OVERDUE"
                Console.WriteLine("  -> Boleto vencido — notificar cliente")
            Case "CANCELED"
                Console.WriteLine("  -> Boleto cancelado")
            Case "FAILED", "REFUSED", "REJECTED"
                Console.WriteLine("  -> Transacao recusada/falhou")
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
        CriarBoleto().GetAwaiter().GetResult()
        Console.WriteLine()
        HandleWebhook("{""id"":""evt_e419ceea"",""status"":""LIQUIDATED""," &
                      """data"":{""value"":""150.5"",""txId"":""3f6f364b"",""method"":""billet""}}")
    End Sub

End Module
