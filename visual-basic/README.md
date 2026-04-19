# Visual Basic — Silapay Integration

> VB.NET e VBA — presente em sistemas legados, automação Office e ERPs.

## Conteúdo planejado

- [ ] PIX CashIn
- [ ] PIX CashOut
- [ ] Cartão de crédito
- [ ] Boleto bancário
- [ ] Webhook handler

## Stack

- VB.NET 16+ (.NET 8)
- HttpClient (System.Net.Http)
- System.Text.Json ou Newtonsoft.Json

## Variantes cobertas

| Variante | Uso |
|---|---|
| VB.NET | Aplicações desktop e web modernas |
| VBA | Automação de planilhas Excel (relatórios financeiros) |

## Como rodar

```bash
# VB.NET via CLI
dotnet run
```

```vbnet
' exemplo mínimo — PIX CashIn
Imports System.Net.Http
Imports System.Text
```

## Notas

- VB.NET compartilha o runtime .NET com C# — qualquer lib .NET funciona
- VBA é útil para automatizar cobranças direto do Excel (boletos em massa, relatórios PIX)

## Status

> Aguardando contribuição da comunidade.
