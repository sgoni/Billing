import argparse
import subprocess
import sys
import time
import requests
import psycopg2
import os
import re
import logging
import socket
from typing import List, Dict
from dotenv import load_dotenv
from pathlib import Path
from config_loader import load_services_config

# Basic logging settings
logging.basicConfig(level=logging.INFO, format="%(asctime)s [%(levelname)s] %(message)s")

# === Configuración de rutas ===
BASE_DIR = Path(__file__).resolve().parent.parent
DOCKER_DIR = BASE_DIR / "dockers"
ENV_DIR = BASE_DIR / "environments"


def load_env(environment: str):
    """Carga las variables desde el archivo .env del entorno dado."""
    env_file = ENV_DIR / f".env.{environment}"

    if not env_file.exists():
        raise FileNotFoundError(f"Not exist environment file: {env_file}")

    load_dotenv(dotenv_path=env_file)
    logging.info(f"✅ Environment variables loaded from {env_file}")


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


def check_postgres_from_config(instances: List[Dict], retries=5, delay=3):
    results = []

    logging.info(f"📦 Loaded instances: {[i['name'] for i in instances]}")
    for instance in instances:
        logging.info(f"➡️ Raw instance: {instance}")
        try:
            name = instance["name"]

            host = resolve_env(instance["host"])
            port = int(resolve_env(instance.get("port", 5432)))
            user = resolve_env(instance["user"])
            password = resolve_env(instance["password"])
            db = resolve_env(instance["db"])

            logging.info(f"🔍 Checking Postgres [{name}] at {host}:{port}/{db}")

            # DNS check
            socket.gethostbyname(host)

            success = False
            for attempt in range(1, retries + 1):
                try:
                    with psycopg2.connect(
                            host=host,
                            port=port,
                            user=user,
                            password=password,
                            dbname=db,
                            connect_timeout=3
                    ):
                        logging.info(f"✅ {name} healthy")
                        success = True
                        break
                except Exception as e:
                    logging.warning(f"❌ {name} attempt {attempt}: {e}")
                    time.sleep(delay * attempt)

            if not success:
                results.append({"name": name, "status": "unreachable"})
            else:
                results.append({"name": name, "status": "healthy"})

        except Exception as e:
            logging.error(f"💥 Error before checking [{instance.get('name')}]: {e}")
            results.append({
                "name": instance.get("name"),
                "status": "error",
                "error": str(e)
            })

    return results


def run_docker(environment: str, action: str):
    """Docker executes with the environment and action given."""
    compose_file = DOCKER_DIR / f"docker_compose_{environment}.yml"
    env_file = ENV_DIR / f".env.{environment}"

    if not compose_file.exists():
        raise FileNotFoundError(f"Not exist {compose_file}")

    cmd = [
        "docker", "compose",
        "-f", str(compose_file),
        "--env-file", str(env_file)
    ]

    if action == "up":
        cmd.extend([action, "-d"])  # raise in background
    else:
        cmd.append(action)

    logging.info(f"🔹 Executing: {' '.join(cmd)}")
    subprocess.run(cmd, check=True)


def _check_http(service: str, url: str, expected: int = 200, retries: int = 5, delay: int = 3):
    """HTTP generic Healthcheck."""

    # Relative path to the certificate from the current script
    cert_path = os.path.join(os.path.dirname(__file__), "certs", "aspnetcore-dev-cert.pfx")

    # Check if the certificate exists
    if os.path.exists(cert_path):
        logging.info(f"🔐 Using certificate in: {cert_path}")
        verify_option = cert_path
    else:
        logging.info(f"⚠️ Certificate not found in relative path, disabling SSL verification")
        verify_option = False

    for attempt in range(1, retries + 1):
        try:
            resp = requests.get(url, timeout=3, verify=verify_option)
            if resp.status_code == expected:
                logging.info(f"✅ {service} healthy in {url}")
                return True
            else:
                logging.info(f"⚠️ {service} replied {resp.status_code}, waiting {expected}")
        except requests.exceptions.RequestException as e:
            logging.info(f"❌ {service.capitalize()} not available ({e})")

        logging.info(f"⏳ Retry {service}... ({attempt}/{retries})")
        time.sleep(delay)

    print(f"🚨 {service.capitalize()} did not pass the healthcheck.")
    return False


def check_health_rabbitmq(retries: int = 5, delay: int = 3):
    """Verify if RabbitMQ Management API responds with auth."""
    url = os.getenv("RABBITMQ_URL", "http://localhost:15672/api/health/checks/virtual-hosts")
    user = os.getenv("RABBITMQ_USER", "guest")
    password = os.getenv("RABBITMQ_PASSWORD", "guest")

    for attempt in range(1, retries + 1):
        try:
            resp = requests.get(url, auth=(user, password), timeout=3)
            if resp.status_code == 200:
                logging.info(f"✅ RabbitMQ healthy at {url}")
                return True
            else:
                logging.info(f"⚠️ RabbitMQ replied {resp.status_code}, expected 200")
        except requests.exceptions.RequestException as e:
            logging.info(f"❌ RabbitMQ not available ({e})")

        logging.info(f"⏳ Retry RabbitMQ... ({attempt}/{retries})")
        time.sleep(delay)

    logging.info("🚨 RabbitMQ did not pass the healthcheck.")
    return False


def run_healthchecks():
    """Run all healthchecks."""
    logging.info("\n🔎 Executing Healthchecks for {Environment} ...")

    config = load_services_config()

    # HTTP services
    for svc in config.get("http", []):
        _check_http(
            svc["name"],
            resolve_env(svc["url"])
        )

    # RabbitMQ
    if "rabbitmq" in config:
        rmq = config["rabbitmq"]
        os.environ["RABBITMQ_HEALTH_URL"] = resolve_env(rmq["health_url"])
        os.environ["RABBITMQ_USER"] = resolve_env(rmq["credentials"]["user"])
        os.environ["RABBITMQ_PASSWORD"] = resolve_env(rmq["credentials"]["password"])
        check_health_rabbitmq()

    # Postgres dinámico
    if "postgres" in config:
        results = check_postgres_from_config(config["postgres"])
        for r in results:
            logging.info(f"🔎 {r['name']}: {r['status'].upper()}")


def clean_docker_volumes():
    try:
        # Remove all unused Docker volumes
        subprocess.run(["docker", "volume", "prune", "-f"], check=True)
        print("✅ Unused volumes removed successfully.")
    except subprocess.CalledProcessError as e:
        print(f"❌ Error while removing volumes: {e}")


def deploy(environment: str, action: str):
    load_env(environment)
    run_docker(environment, action)

    if action == "up":
        # Consul
        subprocess.run(
            [sys.executable, os.path.join(os.path.dirname(__file__), "register_services_consul.py")],
            check=True,
            env={**os.environ, "ENVIRONMENT": environment}
        )

        # Wait for Vault to be ready before init
        vault_url = os.getenv("VAULT_URL", "http://localhost:8200/v#1/sys/health")
        _check_http("Vault", vault_url, retries=8, delay=5)  ##

        ## Vault
        subprocess.run(
            [sys.executable, os.path.join(os.path.dirname(__file__), "vault_deploy.py")],
            check=True,
            env={**os.environ, "ENVIRONMENT": environment}
        )

        run_healthchecks()

    if action == "down":
        clean_docker_volumes()


# -------------------------
# MAIN
# -------------------------

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Deploy con Docker Compose")
    parser.add_argument("environment", choices=["dev", "stage", "prod"], help="Environment to deploy")
    parser.add_argument("action", choices=["up", "down", "ps", "logs"], help="Action to execute")
    args = parser.parse_args()

    deploy(args.environment, args.action)
