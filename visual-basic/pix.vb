' Silapay — PIX CashIn e CashOut
' Documentacao: https://api.silapay.pro/v1/transactions
'
' Requisitos:
'   .NET 6+ (System.Net.Http e System.Text.Json inclusos)
'
' Variaveis de ambiente:
'   SILAPAY_API_KEY=sua_api_key
'   SILAPAY_SECRET_KEY=sua_secret_key
'
' Compilar: vbc pix.vb -r:System.Net.Http.dll

Imports System
Imports System.Net.Http
Imports System.Text
Imports System.Text.Json
Imports System.Threading.Tasks

Module Pix

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

    ' ─── 2. PIX CashIn ───────────────────────────────────────────────────────

    Async Function PixCashIn() As Task
        Dim body As String = "{" &
            """paymentMethod"":""pix""," &
            """cashFlowType"":""cashIn""," &
            """value"":50.00," &
            """postbackUrl"":""http://www.seusite.com.br/webhook/""," &
            """description"":""Venda de produto""," &
            """products"":[{""name"":""AirPod"",""price"":1,""total"":2,""quantity"":2}]," &
            """customer"":{" &
                """name"":""Joao da Silva""," &
                """email"":""joao@email.com""," &
                """phone"":""5511987654321""," &
                """document"":""12345678910""," &
                """userAgent"":""VB.NET/HttpClient""," &
                """street"":""Rua das Flores""," &
                """complement"":""Apto 1""," &
                """postalCode"":""01234567""," &
                """neighborhood"":""Centro""," &
                """state"":""SP""," &
                """country"":""Brasil""}}"

        Dim res = Await Http.SendAsync(BuildRequest(HttpMethod.Post, "/transactions", body))
        res.EnsureSuccessStatusCode()
        Dim doc = JsonDocument.Parse(Await res.Content.ReadAsStringAsync()).RootElement
        Console.WriteLine("PIX CashIn gerado com sucesso!")
        Console.WriteLine("  Transacao ID     : " & doc.GetProperty("id").ToString())
        Console.WriteLine("  Status           : " & doc.GetProperty("status").ToString())
        Console.WriteLine("  Pix Copia e Cola : " & doc.GetProperty("pixCopiaECola").ToString())
        Console.WriteLine("  Criado em        : " & doc.GetProperty("createdAt").ToString())
    End Function

    ' ─── 3. PIX CashOut ──────────────────────────────────────────────────────

    Async Function PixCashOut() As Task
        Dim body As String = "{" &
            """paymentMethod"":""pix""," &
            """cashFlowType"":""cashOut""," &
            """value"":1," &
            """pixKey"":""29242661066""," &
            """pixKeyType"":""cpf""," &
            """postbackUrl"":""http://www.seusite.com.br/webhook/""," &
            """description"":""Pagamento""}"

        Dim res = Await Http.SendAsync(BuildRequest(HttpMethod.Post, "/transactions", body))
        res.EnsureSuccessStatusCode()
        Dim doc = JsonDocument.Parse(Await res.Content.ReadAsStringAsync()).RootElement
        Console.WriteLine("PIX CashOut enviado com sucesso!")
        Console.WriteLine("  Transacao ID : " & doc.GetProperty("id").ToString())
        Console.WriteLine("  Status       : " & doc.GetProperty("status").ToString())
        Console.WriteLine("  Chave PIX    : " & doc.GetProperty("pixKey").ToString())
        Console.WriteLine("  Criado em    : " & doc.GetProperty("createdAt").ToString())
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
                Console.WriteLine("  -> PIX expirado — notificar cliente")
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
        PixCashIn().GetAwaiter().GetResult()
        Console.WriteLine()
        PixCashOut().GetAwaiter().GetResult()
        Console.WriteLine()
        HandleWebhook("{""id"":""evt_e419ceea"",""status"":""LIQUIDATED""," &
                      """data"":{""value"":""50.0"",""txId"":""5ffa9f6f"",""method"":""pix""}}")
    End Sub

End Module
