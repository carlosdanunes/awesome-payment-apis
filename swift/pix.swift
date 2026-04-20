// Silapay — PIX CashIn e CashOut
// Documentacao: https://api.silapay.pro/v1/transactions
//
// Requisitos:
//   Swift 5.5+  (Foundation incluido)
//
// Variaveis de ambiente:
//   SILAPAY_API_KEY=sua_api_key
//   SILAPAY_SECRET_KEY=sua_secret_key
//
// Compilar e rodar:
//   swiftc pix.swift -o pix && ./pix

import Foundation

let baseURL   = "https://api.silapay.pro/v1"
let apiKey    = ProcessInfo.processInfo.environment["SILAPAY_API_KEY"]    ?? "SUA_API_KEY_AQUI"
let secretKey = ProcessInfo.processInfo.environment["SILAPAY_SECRET_KEY"] ?? "SUA_SECRET_KEY_AQUI"

func makeRequest(_ method: String, path: String, body: [String: Any]? = nil) -> URLRequest {
    var req = URLRequest(url: URL(string: baseURL + path)!)
    req.httpMethod = method
    req.setValue("application/json", forHTTPHeaderField: "Accept")
    req.setValue("application/json", forHTTPHeaderField: "Content-Type")
    req.setValue(apiKey,    forHTTPHeaderField: "api-key")
    req.setValue(secretKey, forHTTPHeaderField: "secret-key")
    if let body = body {
        req.httpBody = try! JSONSerialization.data(withJSONObject: body)
    }
    return req
}

func sendSync(_ req: URLRequest) -> (Data, URLResponse) {
    var result: (Data, URLResponse)?
    let sem = DispatchSemaphore(value: 0)
    URLSession.shared.dataTask(with: req) { data, res, _ in
        result = (data!, res!)
        sem.signal()
    }.resume()
    sem.wait()
    return result!
}

// ─── 1. Consultar Saldo ───────────────────────────────────────────────────────

func consultarSaldo() {
    let (data, _) = sendSync(makeRequest("GET", path: "/user/finance/balance"))
    if let json = try? JSONSerialization.jsonObject(with: data) as? [String: Any] {
        print("Saldo disponivel: R$ \(json["balance"] ?? "")")
    }
}

// ─── 2. PIX CashIn ───────────────────────────────────────────────────────────

func pixCashIn() {
    let payload: [String: Any] = [
        "paymentMethod": "pix",
        "cashFlowType":  "cashIn",
        "value":         50.00,
        "postbackUrl":   "http://www.seusite.com.br/webhook/",
        "description":   "Venda de produto",
        "products": [["name": "AirPod", "price": 1, "total": 2, "quantity": 2]],
        "customer": [
            "name":         "Joao da Silva",
            "email":        "joao@email.com",
            "phone":        "5511987654321",
            "document":     "12345678910",
            "userAgent":    "Swift/URLSession",
            "street":       "Rua das Flores",
            "complement":   "Apto 1",
            "postalCode":   "01234567",
            "neighborhood": "Centro",
            "state":        "SP",
            "country":      "Brasil"
        ]
    ]
    let (data, res) = sendSync(makeRequest("POST", path: "/transactions", body: payload))
    let code = (res as! HTTPURLResponse).statusCode
    guard code == 200 || code == 201 else {
        print("Erro ao gerar PIX CashIn: HTTP \(code)")
        return
    }
    if let json = try? JSONSerialization.jsonObject(with: data) as? [String: Any] {
        print("PIX CashIn gerado com sucesso!")
        print("  Transacao ID     : \(json["id"] ?? "")")
        print("  Status           : \(json["status"] ?? "")")
        print("  Pix Copia e Cola : \(json["pixCopiaECola"] ?? "")")
        print("  Criado em        : \(json["createdAt"] ?? "")")
    }
}

// ─── 3. PIX CashOut ──────────────────────────────────────────────────────────

func pixCashOut() {
    let payload: [String: Any] = [
        "paymentMethod": "pix",
        "cashFlowType":  "cashOut",
        "value":         1,
        "pixKey":        "29242661066",
        "pixKeyType":    "cpf",
        "postbackUrl":   "http://www.seusite.com.br/webhook/",
        "description":   "Pagamento"
    ]
    let (data, res) = sendSync(makeRequest("POST", path: "/transactions", body: payload))
    let code = (res as! HTTPURLResponse).statusCode
    guard code == 200 || code == 201 else {
        print("Erro ao enviar PIX CashOut: HTTP \(code)")
        return
    }
    if let json = try? JSONSerialization.jsonObject(with: data) as? [String: Any] {
        print("PIX CashOut enviado com sucesso!")
        print("  Transacao ID : \(json["id"] ?? "")")
        print("  Status       : \(json["status"] ?? "")")
        print("  Chave PIX    : \(json["pixKey"] ?? "")")
        print("  Criado em    : \(json["createdAt"] ?? "")")
    }
}

// ─── Webhook ──────────────────────────────────────────────────────────────────

func handleWebhook(_ payload: [String: Any]) {
    let status = payload["status"] as? String ?? ""
    print("Webhook recebido — ID: \(payload["id"] ?? "")")
    print("  Status  : \(status)")

    switch status {
    case "PAID", "LIQUIDATED", "COMPLETED":
        print("  -> Pagamento confirmado — liberar pedido/servico")
    case "OVERDUE":
        print("  -> PIX expirado — notificar cliente")
    case "FAILED", "REFUSED", "REJECTED":
        print("  -> Transacao recusada/falhou")
    case "REFUNDED", "PARTIALLY_REFUNDED":
        print("  -> Reembolso efetuado")
    case "CHARGEDBACK":
        print("  -> Chargeback confirmado")
    default:
        print("  -> Status: \(status)")
    }
}

// ─── Main ─────────────────────────────────────────────────────────────────────

consultarSaldo()
print("")
pixCashIn()
print("")
pixCashOut()
print("")
handleWebhook([
    "id": "evt_e419ceea-d800-4845-a3fa-6a472d3b44c1",
    "status": "LIQUIDATED",
    "created": "2026-04-19T14:04:25.000Z",
    "data": ["value": "50.0", "txId": "5ffa9f6f", "method": "pix"]
])
