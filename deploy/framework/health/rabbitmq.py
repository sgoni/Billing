import requests
from framework.utils.network import resolve_host


class RabbitHealth:

    def check(self, svc):
        try:
            conn = svc.connection

            host = resolve_host(svc)

            user = conn.admin_user
            password = conn.admin_password
            url = f"http://{host}:{conn.management_port}/api/overview"

            print(f"🔌 Checking RabbitMQ: {url}")

            r = requests.get(
                url,
                auth=(user, password),
                timeout=3
            )

            # 🔥 acepta 200 como healthy
            if r.status_code == 200:
                return True

            print(f"⚠️ Rabbit status: {r.status_code}")
            return False

        except Exception as e:
            print(f"❌ RabbitMQ error: {e}")
            return False
