import requests


class VaultBootstrap:
    def __init__(self, vault_addr: str, token: str):
        self.base = f"{vault_addr}/v1"
        self.headers = {"X-Vault-Token": token}

    # -------------------------
    # ENABLE ENGINE (idempotente)
    # -------------------------
    def enable_engine(self, path, engine):
        url = f"{self.base}/sys/mounts/{path}"

        res = requests.get(url, headers=self.headers)

        if res.status_code == 200:
            print(f"⚠️ {engine} already enabled at {path}")
            return

        print(f"✅ Enabling {engine} engine at {path}")

        requests.post(
            url,
            headers=self.headers,
            json={"type": engine}
        )

    # -------------------------
    # POSTGRES
    # -------------------------
    def configure_postgres(self, svc):
        vault = svc.vault

        self.enable_engine(vault.engine, "database")

        conn_url = (
            f"postgresql://{svc.connection.admin_user}:{svc.connection.admin_password}"
            f"@{svc.connection.internal_host}:{svc.connection.port}/{svc.connection.database}"
            f"?sslmode=disable"
        )

        print(f"🐘 Creating Postgres connection: {vault.connection_name}")

        requests.post(
            f"{self.base}/{vault.engine}/config/{vault.connection_name}",
            headers=self.headers,
            json={
                "plugin_name": "postgresql-database-plugin",
                "allowed_roles": ",".join([r.name for r in vault.roles]),
                "connection_url": conn_url,
            },
        )

        for role in vault.roles:
            print(f"🐘 Creating role: {role.name}")

            requests.post(
                f"{self.base}/{vault.engine}/roles/{role.name}",
                headers=self.headers,
                json={
                    "db_name": vault.connection_name,
                    "creation_statements": ",".join([
                        f'CREATE ROLE "{{name}}" WITH LOGIN PASSWORD \'{{password}}\' VALID UNTIL \'{{expiration}}\';',
                        f'GRANT {", ".join(role.permissions)} ON ALL TABLES IN SCHEMA public TO "{{name}}";'
                    ]),
                    "default_ttl": role.ttl,
                    "max_ttl": role.max_ttl,
                },
            )

    # -------------------------
    # RABBITMQ
    # -------------------------
    def configure_rabbitmq(self, svc):
        vault = svc.vault

        self.enable_engine(vault.engine, "rabbitmq")

        print("🐇 Creating RabbitMQ connection")

        requests.post(
            f"{self.base}/{vault.engine}/config/connection",
            headers=self.headers,
            json={
                "connection_uri": f"http://{svc.connection.internal_host}:{svc.connection.management_port}",
                "username": svc.connection.admin_user,
                "password": svc.connection.admin_password,
            },
        )

        for role in vault.roles:
            print(f"🐇 Creating role: {role.name}")

            requests.post(
                f"{self.base}/{vault.engine}/roles/{role.name}",
                headers=self.headers,
                json={
                    "tags": role.tags,
                    "vhosts": role.vhosts,
                },
            )
