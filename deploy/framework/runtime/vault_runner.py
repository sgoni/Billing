import logging

from framework.runtime.resolver import resolve_host


class VaultRunner:

    def __init__(self, vault_manager):
        self.vault = vault_manager

    def bootstrap(self, services):
        print("🚀 Vault bootstrap starting...")

        for svc in services:
            if not svc.vault or not svc.vault.enabled:
                continue

            if svc.type == "postgres":
                self._setup_postgres(svc)

            elif svc.type == "rabbitmq":
                self._setup_rabbitmq(svc)

        print("✅ Vault bootstrap completed")

    # -------------------------
    # POSTGRES
    # -------------------------
    def _setup_postgres(self, svc):
        conn = svc.connection

        logging.info(f"🐘 Vault Postgres setup: {svc.name}")

        self.vault.enable_database_engine()

        # 🚨 SIEMPRE internal_host para Vault
        host = conn.internal_host

        self.vault.create_postgres_connection(
            name=svc.vault.connection_name,
            host=host,
            port=conn.port,
            database=conn.database,
            username=conn.admin_user,
            password=conn.admin_password,
        )

        for role in svc.vault.roles:
            self.vault.create_postgres_role(
                connection_name=svc.vault.connection_name,
                role_name=role.name,
                ttl=role.ttl,
                max_ttl=role.max_ttl,
                permissions=role.permissions,
            )

    # -------------------------
    # RABBITMQ
    # -------------------------
    def _setup_rabbitmq(self, svc):
        conn = svc.connection
        host = conn.internal_host  # 🔥 SIEMPRE interno

        print(f"🐇 Vault RabbitMQ setup: {svc.name}")

        self.vault.enable_rabbitmq_engine()

        self.vault.create_rabbitmq_connection(
            host=host,
            port=conn.management_port,
            username=conn.admin_user,
            password=conn.admin_password,
        )

        for role in svc.vault.roles:
            self.vault.create_rabbitmq_role(
                role_name=role.name,
                tags=role.tags,
                vhosts=role.vhosts,
            )
