import json
import os

import requests

BASE_URL = "https://api.silapay.pro/v1"


class SilapayProvider:
    def __init__(self, api_key=None, secret_key=None, base_url=BASE_URL, timeout=30):
        api_key = api_key or os.getenv("SILAPAY_API_KEY")
        secret_key = secret_key or os.getenv("SILAPAY_SECRET_KEY")

        if not api_key or not secret_key:
            raise ValueError(
                "Silapay: defina SILAPAY_API_KEY e SILAPAY_SECRET_KEY no ambiente."
            )

        self.base_url = base_url.rstrip("/")
        self.timeout = timeout
        self.session = requests.Session()
        self.session.headers.update(
            {
                "accept": "application/json",
                "content-type": "application/json",
                "api-key": api_key,
                "secret-key": secret_key,
            }
        )

    def pix(self, dados):
        body = {"paymentMethod": "pix", **dados}
        return self._request("POST", "/transactions", json_body=body)

    def boleto(self, dados):
        body = {"paymentMethod": "billet", **dados}
        return self._request("POST", "/transactions", json_body=body)

    def cartao(self, dados):
        body = {"paymentMethod": "creditCard", **dados}
        return self._request("POST", "/transactions", json_body=body)

    def saldo(self):
        return self._request("GET", "/user/finance/balance")

    def _request(self, method, path, json_body=None):
        try:
            response = self.session.request(
                method=method,
                url=f"{self.base_url}{path}",
                json=json_body,
                timeout=self.timeout,
            )
            response.raise_for_status()

            if not response.content:
                return None

            return response.json()
        except requests.HTTPError as err:
            raise create_api_error("Silapay", err) from err


def create_api_error(provider, err):
    response_data = parse_response_data(err.response)
    message = f"{provider} API error: {serialize_response_data(response_data)}"
    api_error = RuntimeError(message)
    api_error.response = err.response
    api_error.response_data = response_data
    return api_error


def parse_response_data(response):
    try:
        return response.json()
    except ValueError:
        return response.text


def serialize_response_data(data):
    if isinstance(data, str):
        return data

    return json.dumps(data, ensure_ascii=False)
