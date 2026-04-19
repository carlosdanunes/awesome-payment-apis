# Pascal / Delphi — Silapay Integration

> Muito usado no Brasil em sistemas comerciais, PDVs e ERPs legados.

## Conteúdo planejado

- [ ] PIX CashIn
- [ ] PIX CashOut
- [ ] Cartão de crédito
- [ ] Boleto bancário
- [ ] Webhook handler

## Stack

- Delphi 11+ (RAD Studio) ou Free Pascal / Lazarus
- Indy (IdHTTP) ou NetHTTP
- SuperObject ou REST.Client (JSON)

## Como rodar

Abre o projeto `.dpr` no Delphi ou Lazarus e compila.

```pascal
// exemplo mínimo — PIX CashIn com TRESTClient
uses REST.Client, REST.Types, System.JSON;
```

## Notas

- Delphi é amplamente usado no Brasil em sistemas de frente de caixa, ERP e automação comercial
- Free Pascal + Lazarus é a alternativa gratuita e open source
- O componente `TRESTClient` / `TRESTRequest` (Delphi XE5+) facilita chamadas REST

## Status

> Aguardando contribuição da comunidade.
