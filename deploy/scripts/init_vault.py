import re
import socket
import subprocess
import json
import os
import logging
import time
import hvac
import psycopg2
from datetime import datetime
from deploy_compose import load_services_config  # importa desde tu script principal

# Log configuration
logging.basicConfig(level=logging.INFO, format="%(asctime)s [%(levelname)s] %(message)s")

# Detect environment
ENV = os.getenv("ENVIRONMENT", "dev")  # Can be passed from deploy.py
VAULT_CONTAINER = f"vault_{ENV}"

# Folder where to save results
OUTPUT_DIR = os.path.join(os.path.dirname(__file__), "../secrets")
os.makedirs(OUTPUT_DIR, exist_ok=True)


def check_postgres_instance(prefix: str, retries: int = 5, delay: int = 3, verbose: bool = False) -> dict:
    """Valida una instancia PostgreSQL usando un prefijo de variables de entorno."""
    required = [f"{prefix}_POSTGRES_HOST", f"{prefix}_POSTGRES_USER", f"{prefix}_POSTGRES_PASSWORD",
                f"{prefix}_POSTGRES_DB"]
    missing = [var for var in required if not os.getenv(var)]
    if missing:
        return {"status": "error", "missing_env": missing, "prefix": prefix}

    host = os.getenv(f"{prefix}_POSTGRES_HOST")
    port = int(os.getenv(f"{prefix}_POSTGRES_PORT", 5432))
    user = os.getenv(f"{prefix}_POSTGRES_USER")
    password = os.getenv(f"{prefix}_POSTGRES_PASSWORD")
    db = os.getenv(f"{prefix}_POSTGRES_DB")

    try:
        socket.gethostbyname(host)
    except socket.gaierror:
        return {"status": "unreachable", "error": f"DNS resolution failed for host '{host}'", "prefix": prefix}

    for attempt in range(1, retries + 1):
        if verbose:
            logging.debug(f"🔍 Connecting to {host}:{port}, DB={db} (Try {attempt})")
        try:
            with psycopg2.connect(
                    host=host, port=port, user=user, password=password, dbname=db, connect_timeout=3
            ):
                return {"status": "healthy", "host": host, "port": port, "db": db, "prefix": prefix}
        except Exception as e:
            logging.warning(f"❌ {prefix} not available ({e})")
            time.sleep(delay * attempt)

    return {"status": "unreachable", "error": "Max retries exceeded", "prefix": prefix}


def wait_for_postgres(instance, retries=10, delay=3):
    name = instance["name"]

    for attempt in range(1, retries + 1):
        result = check_postgres_instance(name.upper(), retries=1)

        if result.get("status") == "healthy":
            logging.info(f"✅ {name} ready for Vault")
            return True

        logging.info(f"⏳ Waiting for {name} ({attempt}/{retries})...")
        time.sleep(delay * attempt)

    logging.error(f"🚨 {name} never became ready")
    return False


def resolve_env(value: str):
    if not isinstance(value, str):
        return value

    match = re.findall(r"\${(.*?)}", value)
    for var in match:
        env_value = os.getenv(var)
        if env_value is None:
            raise ValueError(f"Missing env var: {var}")
        value = value.replace(f"${{{var}}}", env_value)

    return value


def configure_postgres_instances(root_token, instances: list):
    VAULT_ADDR = os.getenv("VAULT_ADDR", "http://localhost:8200")
    client = hvac.Client(url=VAULT_ADDR, token=root_token)

    if not client.is_authenticated():
        raise Exception("❌ Failed to authenticate to Vault.")

    logging.info("🔐 Configuring PostgreSQL instances in Vault...")

    # Enable engine una sola vez
    try:
        client.sys.enable_secrets_engine("database")
    except hvac.exceptions.InvalidRequest:
        logging.info("ℹ️ Secrets engine 'database' already enabled.")

    for instance in instances:
        try:
            name = instance["name"]

            if not wait_for_postgres(instance):
                logging.warning(f"⚠️ Skipping {name}, DB not ready")
                continue

            host = resolve_env(instance.get("docker_host") or instance["host"])
            port = resolve_env(instance.get("port", 5432))
            db = resolve_env(instance["db"])
            user = resolve_env(instance["user"])
            password = resolve_env(instance["password"])

            vault_cfg = instance.get("vault", {})
            role_name = vault_cfg.get("role_name", f"{name}-role")
            ttl = vault_cfg.get("ttl", "5m")
            max_ttl = vault_cfg.get("max_ttl", "30m")

            connection_name = f"{name}-postgres"

            logging.info(f"🔧 Configuring Vault for [{name}]...")

            # 🔌 Connection
            client.secrets.database.configure(
                name=connection_name,
                plugin_name="postgresql-database-plugin",
                allowed_roles=[role_name],
                connection_url=f"postgresql://{{{{username}}}}:{{{{password}}}}@{host}:{port}/{db}?sslmode=disable",
                username=user,
                password=password,
            )

            # 🔐 Role dinámico
            client.secrets.database.create_role(
                name=role_name,
                db_name=connection_name,
                creation_statements=f"""
                    CREATE ROLE "{{{{name}}}}" WITH LOGIN PASSWORD '{{{{password}}}}' VALID UNTIL '{{{{expiration}}}}';

                    GRANT CONNECT ON DATABASE "{db}" TO "{{{{name}}}}";
                    GRANT USAGE ON SCHEMA public TO "{{{{name}}}}";

                    GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO "{{{{name}}}}";

                    ALTER DEFAULT PRIVILEGES IN SCHEMA public
                    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO "{{{{name}}}}";

                    GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA public TO "{{{{name}}}}";

                    ALTER DEFAULT PRIVILEGES IN SCHEMA public
                    GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO "{{{{name}}}}";
                """,
                default_ttl=ttl,
                max_ttl=max_ttl,
            )

            logging.info(f"✅ Vault configured for {name} (role: {role_name})")

        except Exception as e:
            logging.error(f"❌ Error configuring {instance.get('name')}: {e}")


def run_cmd(cmd):
    """Run a command and return the output."""
    try:
        result = subprocess.run(cmd, capture_output=True, text=True, check=True)
        return result.stdout.strip()
    except subprocess.CalledProcessError as e:
        logging.error(f"❌ Error executing command: {cmd}: {e.stderr}")
        raise e


def init_and_unseal():
    "Stage/Prod Only: Initialize and Unlock Vault"
    logging.info(f"🚀 Initializing Vault in container {VAULT_CONTAINER}...")
    output = run_cmd([
        "docker", "exec", "-i", VAULT_CONTAINER,
        "vault", "operator", "init", "-key-shares=5", "-key-threshold=3", "-format=json"
    ])

    creds = json.loads(output)

    # Save file
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    filename = os.path.join(OUTPUT_DIR, f"vault_init_{timestamp}.json")
    with open(filename, "w") as f:
        json.dump(creds, f, indent=2)

    logging.info(f"✅ Vault initialized. Credentials saved in: {filename}")
    logging.info("⚠️ IMPORTANT! Save these keys/tokens in a secure manager (e.g., AWS Secrets Manager).")

    # Unseal with the first 3 keys
    for key in creds.get("unseal_keys_b64", [])[:3]:
        run_cmd(["docker", "exec", "-i", VAULT_CONTAINER, "vault", "operator", "unseal", key])

    logging.info("✅ Vault unlocked (unsealed).")
    return creds["root_token"]


if __name__ == "__main__":
    logging.info(
        f"🔎 Entorno: {ENV} | Vault: {VAULT_CONTAINER} | PostgreSQL: {os.getenv('POSTGRES_HOST')}:{os.getenv('POSTGRES_PORT')}")

    if ENV in ["stage", "prod"]:
        root_token = init_and_unseal()
    else:  # dev
        root_token = os.getenv("VAULT_DEV_TOKEN", "root")

    config = load_services_config()
    postgres_instances = config.get("postgres", [])

    configure_postgres_instances(root_token, postgres_instances)
