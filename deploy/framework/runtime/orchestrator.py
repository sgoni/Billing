from framework.runtime.waiters import wait_for_service
from framework.runtime.vault_runner import VaultRunner
from framework.runtime.health_factory import get_health_checker
from framework.runtime.ConsulRunner import ConsulRunner


class Orchestrator:

    def __init__(self, services, vault_manager, consul_manager):
        self.services = services
        self.vault_runner = VaultRunner(vault_manager)
        self.consul_runner = ConsulRunner(consul_manager)

    def run(self):
        # -------------------------
        # 1. WAIT INFRA
        # -------------------------
        for svc in self.services:
            if svc.type in ["postgres", "rabbitmq", "http"]:
                wait_for_service(svc)

        # -------------------------
        # 2. VAULT
        # -------------------------
        if self.vault_runner.vault:
            self.vault_runner.bootstrap(self.services)

        # -------------------------
        # 3. CONSUL
        # -------------------------
        self.consul_runner.register(self.services)

        # -------------------------
        # 4. WAIT INFRA
        # -------------------------
        for svc in self.services:
            wait_for_service(svc)

        # -------------------------
        # 5. FINAL HEALTHCHECK
        # -------------------------
        for svc in self.services:
            checker = get_health_checker(svc)

            print(f"🔎 Healthcheck {svc.name}")

            if not checker.check(svc):
                raise RuntimeError(f"{svc.name} failed healthcheck")

        print("\n✅ Deployment successful")
