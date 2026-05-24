import json
import logging


class VaultManager:

    def __init__(self, client=None):
        self.client = client

    # -------------------------
    # ENABLE ENGINES
    # -------------------------
    def enable_database_engine(self):
        mounts = self.client.sys.list_mounted_secrets_engines()

        if "database/" in mounts:
            logging.info("⚠️ Database engine already enabled")
            return

        logging.info("✅ Enabling Database engine")

        self.client.sys.enable_secrets_engine(
            backend_type="database",
            path="database",
        )

    def enable_rabbitmq_engine(self):
        mounts = self.client.sys.list_mounted_secrets_engines()

        if "rabbitmq/" in mounts:
            logging.info("⚠️ RabbitMQ engine already enabled")
            return

        logging.info("✅ Enabling RabbitMQ engine")

        self.client.sys.enable_secrets_engine(
            backend_type="rabbitmq",
            path="rabbitmq",
        )

    # -------------------------
    # POSTGRES
    # -------------------------
    def create_postgres_connection(
            self, name, host, port, database, username, password
    ):
        logging.info(f"🐘 Creating Postgres connection: {name}")

        self.client.secrets.database.configure(
            name=name,
            plugin_name="postgresql-database-plugin",
            allowed_roles="*",

            # ✅ FIX 1: URL limpia
            connection_url=f"postgresql://{{username}}:{{password}}@{host}:{port}/{database}",

            username=username,
            password=password,

            # ✅ FIX 2: evitar validación inicial (clave en Docker)
            verify_connection=False,
        )

    def create_postgres_role(
            self, connection_name, role_name, ttl, max_ttl, permissions
    ):
        logging.info(f"🐘 Creating Postgres role: {role_name}")

        creation_statements = [
            f"CREATE ROLE \"{{name}}\" WITH LOGIN PASSWORD '{{password}}' VALID UNTIL '{{expiration}}';",
            f"GRANT {', '.join(permissions)} ON ALL TABLES IN SCHEMA public TO \"{{name}}\";",
        ]

        self.client.secrets.database.create_role(
            name=role_name,
            db_name=connection_name,
            creation_statements=creation_statements,
            default_ttl=ttl,
            max_ttl=max_ttl,
        )

    # -------------------------
    # RABBITMQ
    # -------------------------
    def create_rabbitmq_connection(self, host, port, username, password):
        logging.info("🐇 Creating RabbitMQ connection")

        self.client.secrets.rabbitmq.configure(
            connection_uri=f"http://{host}:{port}",

            # 🔥 CLAVE: credenciales separadas
            username=username,
            password=password,
        )

    def create_rabbitmq_role(self, role_name, tags, vhosts):
        logging.info(f"🐇 Creating RabbitMQ role: {role_name}")

        self.client.secrets.rabbitmq.create_role(
            name=role_name,
            tags=",".join(tags),  # 🔥 también importante
            vhosts=json.dumps(vhosts),  # 🔥 ESTE ES EL FIX
        )
