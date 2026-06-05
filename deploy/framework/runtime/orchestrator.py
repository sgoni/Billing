from framework.runtime.waiters import wait_for_service
from framework.runtime.vault_runner import VaultRunner
from framework.runtime.health_factory import get_health_checker
from framework.runtime.ConsulRunner import ConsulRunner
from framework.health.build_health_summary import build_health_summary
from framework.health.print_health import print_health


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
            wait_for_service(svc)

        # -------------------------
        # 2. VAULT
        # -------------------------
        if self.vault_runner.vault:
            print("🔐 Vault bootstrap starting...")
            self.vault_runner.bootstrap(self.services)

        # -------------------------
        # 3. CONSUL
        # -------------------------
        print("🧭 Consul registration starting...")
        self.consul_runner.register(self.services)

        # -------------------------
        # 4. FINAL HEALTHCHECK
        # -------------------------
        summary = build_health_summary(self.services)
        print_health(summary)

        print("\n✅ Deployment successful")
