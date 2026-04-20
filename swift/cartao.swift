// Silapay — Cartao de Credito
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

func cobrarCartao() {
    let payload: [String: Any] = [
        "paymentMethod": "creditCard", "value": 100, "totalValue": 110,
        "dueDate": "2026-06-30", "postbackUrl": "http://www.seusite.com.br/webhook/",
        "description": "Venda de produto", "installmentCount": 2, "installmentValue": 55,
        "discount": ["value": 1, "dueDateLimitDays": 2, "type": "PERCENTAGE"],
        "interest": ["value": 1], "fine": ["value": 0, "type": "PERCENTAGE"],
        "postalService": false,
        "callback": ["successUrl": "https://www.seusite.com/obrigado", "autoRedirect": false],
        "customer": [
            "name": "Joao da Silva", "email": "joao@email.com", "phone": "11999999999",
            "document": "12345678910", "userAgent": "Swift/URLSession",
            "street": "Rua das Flores", "complement": "Apto 1", "postalCode": "01306010",
            "neighborhood": "Centro", "city": "Sao Paulo", "state": "SP", "country": "Brasil"
        ],
        "card": [
            "cardHolderName": "JOAO DA SILVA", "cardNumber": "4242424242424242",
            "expirationMonth": "06", "expirationYear": "42", "cvv": "424"
        ],
        "cardOwner": ["name": "Joao da Silva", "document": "12345678910"],
        "products": [["name": "AirPod", "price": 55, "total": 110, "quantity": 2,
                      "image": "https://www.seusite.com/image.png"]]
    ]
    let (data, res) = sendSync(makeRequest("POST", path: "/transactions", body: payload))
    let code = (res as! HTTPURLResponse).statusCode
    guard code == 200 || code == 201 else { print("Erro HTTP \(code)"); return }
    if let json = try? JSONSerialization.jsonObject(with: data) as? [String: Any] {
        print("Cartao de credito cobrado com sucesso!")
        print("  Status          : \(json["status"] ?? "")")
        print("  Valor           : R$ \(json["value"] ?? "")")
        print("  Ultimos digitos : \(json["cardLastDigits"] ?? "")")
        print("  Autorizacao     : \(json["authorizationCode"] ?? "")")
        print("  Criado em       : \(json["createdAt"] ?? "")")
    }
}

func handleWebhook(_ payload: [String: Any]) {
    let status = payload["status"] as? String ?? ""
    print("Webhook recebido — ID: \(payload["id"] ?? "")")
    print("  Status  : \(status)")
    switch status {
    case "CONFIRMED", "PAID", "COMPLETED": print("  -> Pagamento aprovado — liberar pedido/servico")
    case "FAILED", "REFUSED", "REJECTED": print("  -> Cartao recusado/falhou")
    case "CANCELED":                       print("  -> Transacao cancelada")
    case "REFUNDED", "PARTIALLY_REFUNDED": print("  -> Reembolso efetuado")
    case "CHARGEDBACK":                    print("  -> Chargeback confirmado")
    default:                               print("  -> Status: \(status)")
    }
}

consultarSaldo(); print("")
cobrarCartao(); print("")
handleWebhook(["id": "evt_a1b2", "status": "CONFIRMED",
               "data": ["value": "100", "txId": "uuid", "method": "creditCard"]])
