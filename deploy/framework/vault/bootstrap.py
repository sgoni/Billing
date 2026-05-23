import logging

from framework.utils.network import resolve_host


class VaultBootstrap:

    def __init__(self, client):
        self.client = client

    # =========================
    # GENERIC SAFE EXEC
    # =========================
    def safe(self, fn, msg):
        try:
            fn()
            logging.info(f"✅ {msg}")
        except Exception as e:
            if "exists" in str(e) or "in use" in str(e):
                logging.info(f"ℹ️ {msg} (already exists)")
            else:
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
    # POSTGRES MULTI INSTANCE
    # =========================
    def setup_postgres(self, svc):
        print("→ creating role:", svc["vault"])

        name = svc["name"]
        role = svc["vault"]["role_name"]
        host = resolve_host(svc)

        connection_name = f"{name}-db"

        logging.info(f"🐘 Vault Postgres setup: {name}")

        # 1. Connection
        self.safe(
            lambda: self.client.secrets.database.configure(
                name=connection_name,
                plugin_name="postgresql-database-plugin",
                allowed_roles=[role],
                connection_url=f"postgresql://{svc['user']}:{svc['password']}@{host}:{svc['port']}/{svc['db']}"
                # connection_url=f"postgresql://{svc['user']}:{svc['password']}@{svc['host']}:{svc['port']}/{svc['db']}?sslmode=disable",
            ),
            f"Postgres connection {connection_name}"
        )

        # 2. Role
        creation_statements = """
        CREATE ROLE "{{name}}" WITH LOGIN PASSWORD '{{password}}' VALID UNTIL '{{expiration}}';

        GRANT CONNECT ON DATABASE "{{db}}" TO "{{name}}";
        GRANT USAGE ON SCHEMA public TO "{{name}}";

        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO "{{name}}";

        ALTER DEFAULT PRIVILEGES IN SCHEMA public
        GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO "{{name}}";
        """

        self.safe(
            lambda: self.client.secrets.database.create_role(
                name=role,
                db_name=connection_name,
                creation_statements=creation_statements.replace("{{db}}", svc["db"]),
                default_ttl=svc["vault"]["ttl"],
                max_ttl=svc["vault"]["max_ttl"]
            ),
            f"Postgres role {role}"
        )

    # =========================
    # RABBITMQ
    # =========================
    def setup_rabbitmq(self, svc):
        role = svc["vault"]["role_name"]
        host = resolve_host(svc)

        logging.info(f"🐇 Vault RabbitMQ setup: {svc['name']}")

        # ✅ FIX connection
        self.safe(
            lambda: self.client.secrets.rabbitmq.configure(
                connection_uri=f"http://{host}:15672",
                username=svc["user"],
                password=svc["password"],
            ),
            "RabbitMQ connection"
        )

        # ✅ FIX role
        self.safe(
            lambda: self.client.secrets.rabbitmq.create_role(
                name=role,
                tags="administrator",
                vhosts='{"\/": {"configure": ".*", "write": ".*", "read": ".*"}}'
            ),
            f"RabbitMQ role {role}"
        )

    # =========================
    # ENTRYPOINT
    # =========================
    def run(self, services):
        logging.info("🚀 Vault bootstrap starting...")

        self.enable_engines()

        for svc in services:
            logging.info(f"🔐 Setting up Vault for {svc['name']}")

            if not svc.get("vault"):
                continue

            if svc["type"] == "postgres":
                self.setup_postgres(svc)

            elif svc["type"] == "rabbitmq":
                self.setup_rabbitmq(svc)

        logging.info("✅ Vault bootstrap completed")
