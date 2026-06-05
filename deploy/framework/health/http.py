import requests

class HttpHealth:

    def check(self, svc):
        try:
            response = requests.get(svc.url, timeout=2)
            return 200 <= response.status_code < 300

        except Exception:
            return False  # 🔥 CLAVE