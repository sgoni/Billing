import requests


class RabbitHealth:

    def check(self, config):
        url = config.get("management_url")
        user = config.get("user")
        password = config.get("password")

        if not url:
            print("❌ RabbitMQ sin management_url")
            return False

        try:
            health_url = f"{url}/api/health/checks/virtual-hosts"

            print(f"🔌 Checking RabbitMQ: {health_url}")

            r = requests.get(
                health_url,
                auth=(user, password),
                timeout=3
            )

            return r.status_code == 200

        except Exception as e:
            print(f"❌ RabbitMQ error: {e}")
            return False
