// Silapay — Boleto Bancario
// Requisitos: Swift 5.5+ / Foundation

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
    if let body = body { req.httpBody = try! JSONSerialization.data(withJSONObject: body) }
    return req
}

func sendSync(_ req: URLRequest) -> (Data, URLResponse) {
    var result: (Data, URLResponse)?
    let sem = DispatchSemaphore(value: 0)
    URLSession.shared.dataTask(with: req) { data, res, _ in result = (data!, res!); sem.signal() }.resume()
    sem.wait()
    return result!
}

func consultarSaldo() {
    let (data, _) = sendSync(makeRequest("GET", path: "/user/finance/balance"))
    if let json = try? JSONSerialization.jsonObject(with: data) as? [String: Any] {
        print("Saldo disponivel: R$ \(json["balance"] ?? "")")
    }
}

func criarBoleto() {
    let payload: [String: Any] = [
        "paymentMethod": "billet",
        "value":         150.50,
        "dueDate":       "2026-06-30T00:00:00.000Z",
        "description":   "Pagamento de servicos",
        "daysAfterDueDateToRegistrationCancellation": 15,
        "externalReference": "REF-001",
        "installmentCount":  1,
        "customer": [
            "name": "Joao da Silva", "email": "joao.silva@email.com",
            "birthDate": "1990-05-20", "phone": "5511987654321",
            "document": "12345678901", "userAgent": "Swift/URLSession",
            "street": "Rua das Flores", "addressNumber": "123",
            "complement": "Apto 456", "postalCode": "01234567",
            "neighborhood": "Centro", "city": "Sao Paulo", "state": "SP", "country": "Brasil"
        ],
        "discount": ["value": 10.50, "dueDateLimitDays": 5, "type": "Fixed"],
        "interest": ["value": 2.0],
        "fine":     ["value": 2.0, "type": "Percentage"]
    ]
    let (data, res) = sendSync(makeRequest("POST", path: "/transactions", body: payload))
    let code = (res as! HTTPURLResponse).statusCode
    guard code == 200 || code == 201 else { print("Erro HTTP \(code)"); return }
    if let json = try? JSONSerialization.jsonObject(with: data) as? [String: Any],
       let tx = json["transaction"] as? [String: Any] {
        print("Boleto criado com sucesso!")
        print("  Transacao ID     : \(tx["transactionId"] ?? "")")
        print("  Status           : \(tx["status"] ?? "")")
        print("  Codigo de barras : \(tx["barCode"] ?? "")")
        print("  Criado em        : \(tx["createdAt"] ?? "")")
    }
}

func handleWebhook(_ payload: [String: Any]) {
    let status = payload["status"] as? String ?? ""
    print("Webhook recebido — ID: \(payload["id"] ?? "")")
    print("  Status  : \(status)")
    switch status {
    case "PAID", "LIQUIDATED", "COMPLETED": print("  -> Pagamento confirmado — liberar pedido/servico")
    case "OVERDUE":                          print("  -> Boleto vencido — notificar cliente")
    case "CANCELED", "DELETED":             print("  -> Boleto cancelado")
    case "FAILED", "REFUSED", "REJECTED":  print("  -> Transacao recusada/falhou")
    case "REFUNDED", "PARTIALLY_REFUNDED": print("  -> Reembolso efetuado")
    case "CHARGEDBACK":                     print("  -> Chargeback confirmado")
    default:                                print("  -> Status: \(status)")
    }
}

consultarSaldo(); print("")
criarBoleto(); print("")
handleWebhook(["id": "evt_e419ceea", "status": "LIQUIDATED",
               "data": ["value": "150.5", "txId": "3f6f364b", "method": "billet"]])
