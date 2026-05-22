import subprocess

from framework.health.engine import HealthEngine


class Orchestrator:

    def __init__(self, services):
        self.services = services

    def wait_for_infra(self):
        print("\n⏳ Waiting for infrastructure...")

        infra_services = [
            s for s in self.services
            if s["type"] in ["postgres", "rabbitmq"]
        ]

        #print(f"🔎 Infra services: {[s['name'] for s in infra_services]}")
        health = HealthEngine(infra_services)

        try:
            health.run()

        except Exception as e:
            print("\n🚨 Infra failed. Fetching Docker logs...\n")

            # ⬇️ AQUÍ agregas logs automáticos
            subprocess.run(
                ["docker", "compose", "logs"],
                check=False
            )

            raise e