import logging
import subprocess

from framework.health.engine import HealthEngine
from framework.utils.waiters import wait_for_postgres, wait_rabbit
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
            conn = svc["connection"]
            host = resolve_host(svc)
            timeout = 60

            if svc["type"] == "postgres":
                logging.info(f"⏳ Waiting for Postgres {svc['name']}...")
                wait_for_postgres(host, conn["port"], timeout=timeout)

            elif svc["type"] == "rabbitmq":
                logging.info(f"⏳ Waiting for RabbitMQ {svc['name']}...")
                wait_rabbit(
                    host,
                    conn["management_port"],
                    conn["admin_user"],
                    conn["admin_password"],
                    timeout=timeout
                )

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
