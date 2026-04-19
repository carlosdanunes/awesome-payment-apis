# awesome-payment-apis — Exemplos por Linguagem

Integração com a API Silapay nas 20 linguagens mais usadas do mundo.

---

## Roadmap

| # | Linguagem | Framework principal | Mês | Status |
|---|---|---|---|---|
| 1 | TypeScript | NestJS | Mês 1 | Em andamento |
| 2 | JavaScript | Node.js + Express | Mês 1 | Em andamento |
| 3 | Java | Spring Boot | Mês 2 | Planejado |
| 4 | Python | FastAPI + requests | Mês 3 | Planejado |
| 5 | C# | ASP.NET Core | Mês 4+ | Comunidade |
| 6 | PHP | Laravel | Mês 4+ | Comunidade |
| 7 | Go | stdlib net/http | Mês 4+ | Comunidade |
| 8 | Ruby | Rails | Mês 4+ | Comunidade |
| 9 | Kotlin | Spring Boot / Ktor | Mês 4+ | Comunidade |
| 10 | Swift | Vapor | Mês 4+ | Comunidade |
| 11 | Rust | Axum / Actix | Mês 4+ | Comunidade |
| 12 | Scala | Play / Akka HTTP | Mês 4+ | Comunidade |
| 13 | Dart | Dart Frog / Flutter | Mês 4+ | Comunidade |
| 14 | Elixir | Phoenix | Mês 4+ | Comunidade |
| 15 | Groovy | Grails | Mês 4+ | Comunidade |
| 16 | Lua | OpenResty | Mês 4+ | Comunidade |
| 17 | Perl | Mojolicious | Mês 4+ | Comunidade |
| 18 | Haskell | Servant | Mês 4+ | Comunidade |
| 19 | Clojure | Ring + Compojure | Mês 4+ | Comunidade |
| 20 | R | plumber | Mês 4+ | Comunidade |
| 21 | Pascal / Delphi | TRESTClient / Lazarus | Mês 4+ | Comunidade |
| 22 | Visual Basic | VB.NET / VBA | Mês 4+ | Comunidade |

---

## Estrutura

```
awesome-payment-apis/
├── typescript/     ← Mês 1 — NestJS
├── javascript/     ← Mês 1 — Node.js + Express
├── java/           ← Mês 2 — Spring Boot
├── python/         ← Mês 3 — FastAPI
├── csharp/         ← comunidade — ASP.NET Core
├── php/            ← comunidade — Laravel
├── go/             ← comunidade — stdlib
├── ruby/           ← comunidade — Rails
├── kotlin/         ← comunidade — Spring Boot / Ktor
├── swift/          ← comunidade — Vapor
├── rust/           ← comunidade — Axum / Actix
├── scala/          ← comunidade — Play
├── dart/           ← comunidade — Dart Frog
├── elixir/         ← comunidade — Phoenix
├── groovy/         ← comunidade — Grails
├── lua/            ← comunidade — OpenResty
├── perl/           ← comunidade — Mojolicious
├── haskell/        ← comunidade — Servant
├── clojure/        ← comunidade — Ring
├── r/              ← comunidade — plumber
├── pascal-delphi/  ← comunidade — TRESTClient / Lazarus
└── visual-basic/   ← comunidade — VB.NET / VBA
```

---

## Métodos de pagamento cobertos em cada linguagem

| Método | cashFlowType | paymentMethod |
|---|---|---|
| PIX receber | `cashIn` | `pix` |
| PIX enviar | `cashOut` | `pix` |
| Cartão de crédito | `cashIn` | `creditCard` |
| Boleto bancário | `cashIn` | `boleto` |

---

## Como contribuir

Cada pasta tem um `README.md` com os arquivos planejados e o comando para rodar.
Se você domina uma das linguagens marcadas como "comunidade", abre um PR seguindo o padrão das pastas já implementadas (typescript/ e javascript/).
