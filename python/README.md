# Python — Silapay Integration

> Mês 3 do roadmap. Exemplos simples com requests e FastAPI.

## Conteúdo planejado

- [ ] PIX CashIn (gerar QR Code)
- [ ] PIX CashOut (transferir para chave PIX)
- [ ] Cartão de crédito
- [ ] Boleto bancário
- [ ] Webhook handler com FastAPI

## Stack

- Python 3.11+
- requests (exemplos simples)
- FastAPI (webhook handler)
- python-dotenv

## Como rodar

```bash
pip install requests python-dotenv fastapi uvicorn
cp .env.example .env
# preenche as chaves no .env
python pix_cashin.py
```

## Arquivos planejados

| Arquivo | Descrição |
|---|---|
| `pix_cashin.py` | Exemplo mínimo — PIX CashIn |
| `pix_cashout.py` | Exemplo mínimo — PIX CashOut |
| `cartao.py` | Exemplo mínimo — cartão de crédito |
| `boleto.py` | Exemplo mínimo — boleto bancário |
| `fastapi_webhook.py` | Servidor FastAPI para receber postback |

## Status

> Em construção — Mês 3 do roadmap.
