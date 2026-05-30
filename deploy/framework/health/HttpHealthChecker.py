import requests


class HttpHealth:
    def __init__(self, timeout: int = 2):
        self.timeout = timeout

    def check(self, svc) -> bool:
        try:
            response = requests.get(svc.url, timeout=self.timeout)

            if 200 <= response.status_code < 300:
                return True

            print(f"⚠️ {svc.name} unhealthy (status {response.status_code})")
            return False

        except requests.RequestException as ex:
            print(f"⏳ HTTP check failed: {ex}")
            return False
