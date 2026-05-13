import consul
import logging
import os
import re
from urllib.parse import urlparse
from deploy_compose import load_services_config, resolve_env

logging.basicConfig(level=logging.INFO, format="%(asctime)s [%(levelname)s] %(message)s")


def get_consul_client():
    host = os.getenv("CONSUL_HOST", "localhost")
    port = int(os.getenv("CONSUL_PORT", 8500))
    return consul.Consul(host=host, port=port)


# -------------------------
# HELPERS
# -------------------------

def resolve_host_for_consul(instance):
    """
    Igual problema que antes:
    - Script corre fuera → localhost
    - Consul puede correr dentro → usar docker_host
    """
    if os.getenv("RUNNING_IN_DOCKER"):
        return resolve_env(instance.get("docker_host") or instance["host"])
    return resolve_env(instance["host"])


def extract_host_port_from_url(url):
    parsed = urlparse(url)
    return parsed.hostname, parsed.port or (443 if parsed.scheme == "https" else 80)


# -------------------------
# REGISTROS
# -------------------------

def register_postgres_services(c, instances):
    for instance in instances:
        try:
            consul_cfg = instance.get("consul")
            if not consul_cfg:
                continue

            name = instance["name"]
            service_name = consul_cfg.get("service_name", f"{name}-db")
            tags = consul_cfg.get("tags", ["postgres"])

            host = resolve_host_for_consul(instance)
            docker_host = resolve_env(instance.get("docker_host"))
            port = int(resolve_env(instance.get("port", 5432)))

            service_id = f"{service_name}-{name}"

            logging.info(f"🔧 Registering Postgres [{name}] in Consul...")
            logging.info(
                f"🔧 service_name: [{name}], service_id: [{service_id}], docker_host: [{docker_host}], host: [{host}],port: [{port}]")

            c.agent.service.register(
                name=service_name,
                service_id=service_id,
                address=docker_host,  # nombre del contenedor
                port=port,
                check={
                    "tcp": f"{docker_host}:{port}",
                    "interval": "10s",
                    "timeout": "3s"
                }
            )

            logging.info(f"✅ Registered {service_name} ({host}:{port})")

        except Exception as e:
            logging.error(f"❌ Error registering Postgres {instance.get('name')}: {e}")


def register_http_services(c, instances):
    for svc in instances:
        try:
            consul_cfg = svc.get("consul")
            if not consul_cfg:
                continue

            name = svc["name"]
            service_name = consul_cfg.get("service_name", name)
            tags = consul_cfg.get("tags", ["http"])

            url = resolve_env(svc["url"])
            host, port = extract_host_port_from_url(url)

            service_id = f"{service_name}-{name}"

            logging.info(f"🔧 Registering HTTP [{name}] in Consul...")

            c.agent.service.register(
                name=service_name,
                service_id=service_id,
                address=host,
                port=port,
                tags=tags,
                check={
                    "http": url,
                    "interval": "10s",
                    "timeout": "3s"
                }
            )

            logging.info(f"✅ Registered {service_name} ({url})")

        except Exception as e:
            logging.error(f"❌ Error registering HTTP {svc.get('name')}: {e}")


# -------------------------
# MAIN
# -------------------------

def register_all_services():
    config = load_services_config()
    c = get_consul_client()

    logging.info("🚀 Registering services in Consul...")

    register_postgres_services(c, config.get("postgres", []))
    register_http_services(c, config.get("http", []))

    logging.info("✅ Consul registration complete.")


if __name__ == "__main__":
    register_all_services()
