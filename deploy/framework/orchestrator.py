import subprocess

from framework.health.engine import HealthEngine
from framework.utils.waiters import wait_for_postgres
from framework.utils.network import resolve_host


class Orchestrator:

    def __init__(self, services):
        self.services = services

    def wait_for_infra(self):
        print("\n⏳ Waiting for infrastructure...")

        infra_services = [
            s for s in self.services
            if s["type"] in ["postgres", "rabbitmq"]
        ]

        # 🔥 PRE-CHECK (AQUÍ VA LA MEJORA)
        for svc in infra_services:
            if svc["type"] == "postgres":
                print(f"⏳ Waiting for Postgres {svc['name']}...")
                timeout = 40
            elif svc["type"] == "rabbitmq":
                timeout = 40

            host = resolve_host(svc)
            wait_for_postgres(host, svc["port"], timeout=timeout)

        # 👇 tu lógica existente sigue intacta
        health = HealthEngine(infra_services)

        try:
            health.run()

        except Exception as e:
            print("\n🚨 Infra failed. Fetching Docker logs...\n")

            subprocess.run(
                ["docker", "compose", "logs"],
                check=False
            )

            raise e
