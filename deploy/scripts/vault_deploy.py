import os
import re
import sys
import yaml
import logging
import json
from pathlib import Path

import hvac

# =========================
# Logging
# =========================
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s"
)

# =========================
# Vault Config
# =========================
VAULT_ADDR = os.getenv("VAULT_ADDR", "http://localhost:8200")
VAULT_TOKEN = os.getenv("VAULT_DEV_TOKEN")

if not VAULT_TOKEN:
    raise Exception("VAULT_DEV_TOKEN no definido en entorno")

client = hvac.Client(url=VAULT_ADDR, token=VAULT_TOKEN)


# =========================
# Validaciones Vault
# =========================
def ensure_vault_ready():
    if not client.sys.is_initialized():
        raise Exception("Vault no está inicializado")

    if client.sys.is_sealed():
        raise Exception("Vault está sellado")

    if not client.is_authenticated():
        raise Exception("Autenticación con Vault falló")

    logging.info(f"✅ Vault listo en {VAULT_ADDR}")


# =========================
# Utils
# =========================
def resolve_env(value):
    if isinstance(value, str):
        matches = re.findall(r"\$\{(.+?)\}", value)
        for m in matches:
            value = value.replace(f"${{{m}}}", os.getenv(m, ""))
    return value


def deep_resolve(obj):
    if isinstance(obj, dict):
        return {k: deep_resolve(v) for k, v in obj.items()}
    elif isinstance(obj, list):
        return [deep_resolve(i) for i in obj]
    else:
        return resolve_env(obj)


# =========================
# Config Loader
# =========================
def load_services_config(path=None):
    if path:
        config_path = Path(path)
    else:
        # subir un nivel desde /scripts/
        config_path = Path(__file__).resolve().parent.parent / "services.yml"

    if not config_path.exists():
        raise FileNotFoundError(f"Missing config file: {config_path}")

    logging.info(f"📄 Loading config: {config_path}")

    with open(config_path, "r") as f:
        return yaml.safe_load(f)


# =========================
# Enable Engines
# =========================
def enable_engine(path, engine):
    try:
        client.sys.enable_secrets_engine(
            backend_type=engine,
            path=path
        )
        logging.info(f"✅ Engine '{path}' habilitado")

    except hvac.exceptions.InvalidRequest as e:
        if "path is already in use" in str(e):
            logging.info(f"ℹ️ Engine '{path}' ya existe")
        else:
            raise


def enable_engines():
    enable_engine("database", "database")
    enable_engine("rabbitmq", "rabbitmq")


# =========================
# PostgreSQL Setup
# =========================
def setup_postgres(pg):
    name = pg["name"]
    db = pg["db"]
    db_host = pg.get("docker_host", pg["host"])

    connection_name = f"{name}-db"
    role_name = pg["vault"]["role_name"]

    logging.info(f"🐘 Configurando Postgres: {name}")

    # 1. Configurar conexión
    client.secrets.database.configure(
        name=connection_name,
        plugin_name="postgresql-database-plugin",
        allowed_roles=[role_name],
        connection_url=f"postgresql://{pg['user']}:{pg['password']}@{db_host}:{pg['port']}/{db}?sslmode=disable",
        #connection_url=f"postgresql://{pg['user']}:{pg['password']}@{pg['host']}:{pg['port']}/{db}?sslmode=disable",
        username=pg["user"],
        password=pg["password"]
    )

    # 2. Creation statements
    creation_statements = f"""
        CREATE ROLE "{{{{name}}}}" WITH LOGIN PASSWORD '{{{{password}}}}' VALID UNTIL '{{{{expiration}}}}';

        GRANT CONNECT ON DATABASE "{db}" TO "{{{{name}}}}";
        GRANT USAGE ON SCHEMA public TO "{{{{name}}}}";

        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO "{{{{name}}}}";

        ALTER DEFAULT PRIVILEGES IN SCHEMA public
        GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO "{{{{name}}}}";

        GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA public TO "{{{{name}}}}";

        ALTER DEFAULT PRIVILEGES IN SCHEMA public
        GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO "{{{{name}}}}";
    """

    # 3. Crear rol dinámico
    client.secrets.database.create_role(
        name=role_name,
        db_name=connection_name,
        creation_statements=creation_statements,
        default_ttl=pg["vault"]["ttl"],
        max_ttl=pg["vault"]["max_ttl"]
    )

    logging.info(f"✅ Role dinámico creado: {role_name}")


# =========================
# RabbitMQ Setup
# =========================

def setup_rabbitmq(rabbit):
    logging.info("🐇 Configurando RabbitMQ")

    host = rabbit.get("docker_host", "localhost")
    connection_uri = f"http://{host}:15672"

    client.secrets.rabbitmq.configure(
        connection_uri=connection_uri,
        username=rabbit["credentials"]["user"],
        password = rabbit["credentials"]["password"]
    )

    client.secrets.rabbitmq.create_role(
        name="default",
        tags="administrator",
        vhosts=json.dumps({
            "/": {
                "configure": ".*",
                "write": ".*",
                "read": ".*"
            }
        })
    )

    logging.info("✅ RabbitMQ role dinámico creado")


# =========================
# Main
# =========================
def main():
    config_path = sys.argv[1] if len(sys.argv) > 1 else None

    ensure_vault_ready()

    config = load_services_config(config_path)
    config = deep_resolve(config)

    enable_engines()

    # PostgreSQL
    for pg in config.get("postgres", []):
        setup_postgres(pg)

    # RabbitMQ
    if "rabbitmq" in config:
        setup_rabbitmq(config["rabbitmq"])

    logging.info("🎉 Vault deployment completado")


if __name__ == "__main__":
    main()
