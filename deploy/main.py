import argparse
import os
import subprocess
import sys
import logging

from framework.core.engine import DeployEngine
from framework.health.engine import HealthEngine
from framework.docker.compose import DockerCompose
from framework.utils.env_loader import load_env
from framework.orchestrator import Orchestrator
from framework.vault.client import VaultClient
from framework.vault.manager import VaultManager
from framework.vault.bootstrap import VaultBootstrap
from framework.utils.waiters import wait_for_vault
from dotenv import load_dotenv

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s"
)


def main():
    parser = argparse.ArgumentParser()

    parser.add_argument("--env", default="dev", help="Environment (dev/stage/prod)")
    parser.add_argument("--down", action="store_true", help="Stop infrastructure")
    parser.add_argument("--no-docker", action="store_true", help="Skip docker compose")

    args = parser.parse_args()
    ENV = args.env

    print(f"🌍 Environment: {ENV}")

    # =========================
    # 1. Load env
    # =========================
    load_env(ENV)

    # =========================
    # 2. Docker control
    # =========================
    docker = DockerCompose(ENV)

    if args.down:
        print("🧹 Shutting down infrastructure...")
        docker.down()
        sys.exit(0)

    if not args.no_docker:
        docker.up()

    # =========================
    # 3. Vault init (safe)
    # =========================
    vault_client = None
    vault_manager = None

    load_dotenv()
    logging.info("🔍 Initializing Vault...")

    try:
        vault_url = os.environ.get("VAULT_URL")
        vault_token = os.environ.get("VAULT_TOKEN")

        if vault_url:
            wait_for_vault(vault_url)  # 👈 AQUÍ

        if not vault_url or not vault_token:
            raise ValueError("Missing VAULT_URL or VAULT_TOKEN")

        vault_client = VaultClient(
            url=vault_url,
            token=vault_token
        )

        vault_manager = VaultManager()

        logging.info(f"Vault manager: {vault_manager}")
        logging.info("🔐 Vault connected")

    except Exception as e:
        logging.info(f"⚠️ Vault not available: {e}")
        logging.info("⚠️ Continuing WITHOUT Vault...")

    # =========================
    # 4. Load config
    # =========================
    engine = DeployEngine("services.yml")

    # =========================
    # 5. Vault bootstrap (ANTES de health)
    # =========================
    if vault_manager:
        bootstrap = VaultBootstrap(vault_client)
        bootstrap.run(engine.config["services"])

        # inyectar vault en el contexto del engine
        engine.context.vault = vault_manager

    # =========================
    # 6. Wait infra
    # =========================
    orchestrator = Orchestrator(engine.config["services"])
    orchestrator.wait_for_infra()

    # =========================
    # 7. Deploy lógico
    # =========================
    engine.run()

    # =========================
    # 8. Final health
    # =========================
    health = HealthEngine(engine.config["services"])

    try:
        health.run()
        print("\n✅ Deployment successful")

    except Exception as e:
        print("\n🚨 Final health failed. Docker logs:\n")
        subprocess.run(["docker", "compose", "logs"], check=False)
        raise e


if __name__ == "__main__":
    main()
