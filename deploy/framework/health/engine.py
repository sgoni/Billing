import time

from framework.health.registry import HealthRegistry


class HealthEngine:

    def __init__(self, services):
        self.services = services
        self.registry = HealthRegistry()

    def _get_timeout(self, svc):
        # ⬇️ AQUÍ defines tiempos por tipo
        if svc["type"] == "postgres":
            return 30
        elif svc["type"] == "rabbitmq":
            return 20
        elif svc["type"] == "http":
            return 15
        return 10  # default

    def run(self):
        for svc in self.services:
            checker = self.registry.get(svc["type"])

            if not checker:
                continue

            timeout = self._get_timeout(svc)
            start = time.time()

            print(f"🔎 Healthcheck {svc['name']} (timeout={timeout}s)")

            while time.time() - start < timeout:
                if checker.check(svc):
                    print("✅ Healthy")
                    break
                else:
                    print("⏳ waiting...")
                    time.sleep(2)
            else:
                raise Exception(f"🚨 {svc['name']} failed healthcheck after {timeout}s")
