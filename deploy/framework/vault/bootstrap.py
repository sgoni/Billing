import logging
import json

from framework.utils.network import resolve_host


class VaultBootstrap:

    def __init__(self, client):
        self.client = client

    # =========================
    # SAFE EXEC (IDEMPOTENTE)
    # =========================
    def safe(self, fn, msg):
        try:
            fn()
            logging.info(f"✅ {msg}")
        except Exception as e:
            if any(x in str(e).lower() for x in ["exists", "in use", "already"]):
                logging.info(f"ℹ️ {msg} (already exists)")
            else:
                logging.error(f"❌ {msg}: {e}")
                raise

    # =========================
    # ENABLE ENGINES
    # =========================
    def enable_engines(self):
        self.safe(
            lambda: self.client.sys.enable_secrets_engine(
                backend_type="database",
                path="database"
            ),
            "Database engine enabled"
        )

        self.safe(
            lambda: self.client.sys.enable_secrets_engine(
                backend_type="rabbitmq",
                path="rabbitmq"
            ),
            "RabbitMQ engine enabled"
        )

    # =========================
    # VALIDATION GUARD
    # =========================
    def is_vault_enabled(self, svc):
        return svc.get("vault", {}).get("enabled", False)

    # =========================
    # POSTGRES MULTI-TENANT
    # =========================
    def setup_postgres(self, svc):
        name = svc["name"]
        conn = svc["connection"]
        vault = svc["vault"]

        # host = resolve_host(svc)
        host = conn["internal_host"]
        connection_name = vault["connection_name"]

        roles = vault.get("roles", [])
        role_names = [r["name"] for r in roles]

        logging.info(f"🐘 Vault Postgres setup: {name}")

        # -------------------------
        # 1. CONNECTION
        # -------------------------
        self.safe(
            lambda: self.client.secrets.database.configure(
                name=connection_name,
                plugin_name="postgresql-database-plugin",
                allowed_roles=role_names,
                connection_url=(
                    f"postgresql://{conn['admin_user']}:{conn['admin_password']}"
                    f"@{host}:{conn['port']}/{conn['database']}"
                )
            ),
            f"Postgres connection {connection_name}"
        )

        # -------------------------
        # 2. ROLES (MULTI)
        # -------------------------
        creation_statements_template = """
        CREATE ROLE "{{name}}" WITH LOGIN PASSWORD '{{password}}' VALID UNTIL '{{expiration}}';

        GRANT CONNECT ON DATABASE "{{db}}" TO "{{name}}";
        GRANT USAGE ON SCHEMA public TO "{{name}}";

        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO "{{name}}";

        ALTER DEFAULT PRIVILEGES IN SCHEMA public
        GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO "{{name}}";
        """

        for role in roles:
            role_name = role["name"]

            logging.info(f"   ↳ creating role: {role_name}")

            self.safe(
                lambda r=role: self.client.secrets.database.create_role(
                    name=r["name"],
                    db_name=connection_name,
                    creation_statements=creation_statements_template.replace(
                        "{{db}}", conn["database"]
                    ),
                    default_ttl=r.get("ttl", "1h"),
                    max_ttl=r.get("max_ttl", "24h")
                ),
                f"Postgres role {role_name}"
            )

    # =========================
    # RABBITMQ MULTI-TENANT
    # =========================
    def setup_rabbitmq(self, svc):
        name = svc["name"]
        conn = svc["connection"]
        vault = svc["vault"]
        host = resolve_host(svc)

        # host = conn.get("docker_host") or conn["host"]
        roles = vault.get("roles", [])

        logging.info(f"🐇 Vault RabbitMQ setup: {name}")

        # -------------------------
        # 1. CONNECTION
        # -------------------------
        self.safe(
            lambda: self.client.secrets.rabbitmq.configure(
                connection_uri=f"http://{host}:{conn['management_port']}",
                username=conn["admin_user"],
                password=conn["admin_password"],
                verify_connection=False
            ),
            "RabbitMQ connection"
        )

        # -------------------------
        # 2. ROLES
        # -------------------------
        for role in roles:
            role_name = role["name"]

            logging.info(f"   ↳ creating role: {role_name}")

            self.safe(
                lambda r=role: self.client.secrets.rabbitmq.create_role(
                    name=r["name"],
                    tags=r.get("tags", ""),
                    vhosts=json.dumps(r.get("vhosts", {}))
                ),
                f"RabbitMQ role {role_name}"
            )

    # =========================
    # ENTRYPOINT
    # =========================
    def run(self, services):
        logging.info("🚀 Vault bootstrap starting...")

        self.enable_engines()

        for svc in services:
            name = svc.get("name", "unknown")
            logging.info(f"🔐 Setting up Vault for {name}")

            # 🔥 GUARD GLOBAL (NO MÁS KeyError)
            if not self.is_vault_enabled(svc):
                logging.info(f"⏭️ Vault disabled for {name}")
                continue

            svc_type = svc.get("type")

            if svc_type == "postgres":
                self.setup_postgres(svc)

            elif svc_type == "rabbitmq":
                self.setup_rabbitmq(svc)

            else:
                logging.warning(f"⚠️ Unsupported service type: {svc_type}")

        logging.info("✅ Vault bootstrap completed")
