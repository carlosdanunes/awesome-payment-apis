# Java — Silapay Integration

> Mês 2 do roadmap. Exemplos com Spring Boot.

## Conteúdo planejado

- [ ] PIX CashIn (gerar QR Code)
- [ ] PIX CashOut (transferir para chave PIX)
- [ ] Cartão de crédito
- [ ] Boleto bancário
- [ ] Webhook handler
- [ ] Service + Controller + DTO com Spring Boot

## Stack

- Java 17+
- Spring Boot 3+
- Spring Web
- RestTemplate / WebClient
- Maven

## Como rodar

```bash
# com Maven
./mvnw spring-boot:run

# ou via IDE (IntelliJ IDEA recomendado)
```

## Arquivos planejados

| Arquivo | Descrição |
|---|---|
| `silapay-springboot/` | Projeto Spring Boot completo |
| `SilapayService.java` | Client HTTP para a API Silapay |
| `TransacaoController.java` | Endpoints REST |
| `PixCashInDto.java` | DTO de entrada para PIX |
| `CartaoDto.java` | DTO de entrada para cartão |
| `BoletoDto.java` | DTO de entrada para boleto |
| `WebhookController.java` | Receber postback da Silapay |

## Status

> Em construção — Mês 2 do roadmap.
