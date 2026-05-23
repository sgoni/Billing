import requests
from framework.utils.network import build_http_url


class RabbitHealth:

    def check(self, svc):
        try:
            url = build_http_url(
                svc,
                port_key="management_port",
                path="/api/health/checks/virtual-hosts"
            )

            user = svc.get("user", "guest")
            password = svc.get("password", "guest")

            print(f"🔌 Checking RabbitMQ: {url}")

            r = requests.get(
                url,
                auth=(user, password),
                timeout=3
            )

            return r.status_code == 200

        except Exception as e:
            print(f"❌ RabbitMQ error: {e}")
            return False