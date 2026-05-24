import argparse
import os
import sys
import logging

from framework.consul.ConsulManager import ConsulManager
from framework.config.loader import load_services_config
from framework.runtime.orchestrator import Orchestrator
from framework.docker.compose import DockerCompose
from framework.utils.env_loader import load_env
from framework.vault.client import VaultClient
from framework.vault.manager import VaultManager
from framework.utils.waiters import wait_for_vault
from dotenv import load_dotenv

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s"
)


def main():
    parser = argparse.ArgumentParser()

    parser.add_argument("--env", default="dev")
    parser.add_argument("--down", action="store_true")
    parser.add_argument("--no-docker", action="store_true")

    args = parser.parse_args()
    ENV = args.env

    print(f"🌍 Environment: {ENV}")

    # =========================
    # 1. ENV
    # =========================
    load_env(ENV)

    # =========================
    # 2. DOCKER
    # =========================
    docker = DockerCompose(ENV)

    if args.down:
        print("🧹 Shutting down infrastructure...")
        docker.down()
        sys.exit(0)

    if not args.no_docker:
        docker.up()

    # =========================
    # 3. VAULT INIT
    # =========================
    vault_manager = None

    load_dotenv()
    logging.info("🔍 Initializing Vault...")

    try:
        vault_url = os.environ.get("VAULT_URL")
        vault_token = os.environ.get("VAULT_TOKEN")

        if vault_url:
            wait_for_vault(vault_url)

        if not vault_url or not vault_token:
            raise ValueError("Missing VAULT config")

        vault_client = VaultClient(
            url=vault_url,
            token=vault_token
        )

        vault_manager = VaultManager(vault_client)

        logging.info("🔐 Vault connected")

    except Exception as e:
        logging.info(f"⚠️ Vault not available: {e}")
        logging.info("⚠️ Continuing WITHOUT Vault...")

    # =========================
    # 3.5 CONSUL INIT
    # =========================
    consul_manager = None

    try:
        consul_host = os.environ.get("CONSUL_HOST", "localhost")
        consul_port = int(os.environ.get("CONSUL_PORT", 8500))

        logging.info("🧭 Initializing Consul...")

        consul_manager = ConsulManager(
            host=consul_host,
            port=consul_port
        )

        logging.info("🧭 Consul connected")

    except Exception as e:
        logging.info(f"⚠️ Consul not available: {e}")
        logging.info("⚠️ Continuing WITHOUT Consul...")

    # =========================
    # 4. LOAD CONFIG (🔥 NUEVO)
    # =========================
    config = load_services_config("deploy/services.yml")
    services = config.services

    # =========================
    # 5. ORCHESTRATION TOTAL
    # =========================
    orchestrator = Orchestrator(
        services=services,
        vault_manager=vault_manager,
        consul_manager=consul_manager
    )

    #for svc in services:
    #    print(svc.model_dump())

    orchestrator.run()


if __name__ == "__main__":
    main()
