import argparse
import subprocess

from framework.core.engine import DeployEngine
from framework.health.engine import HealthEngine
from framework.docker.compose import DockerCompose
from framework.utils.env_loader import load_env
from framework.orchestrator import Orchestrator

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--env", default="dev", help="Environment (dev/stage/prod)")

    args = parser.parse_args()
    ENV = args.env

    print(f"🌍 Environment: {ENV}")

    # 1. Load env
    load_env(ENV)

    # 2. Docker up
    docker = DockerCompose(ENV)
    docker.up()

    # 3. Deploy config
    engine = DeployEngine("services.yml")

    # 4. Wait infra (DB, MQ)
    orchestrator = Orchestrator(engine.config["services"])
    orchestrator.wait_for_infra()

    # 5. Deploy lógico (Vault, etc.)
    engine.run()

    # 6. Health final
    health = HealthEngine(engine.config["services"])
    try:
        health.run()
    except Exception as e:
        print("\n🚨 Final health failed. Docker logs:\n")
        subprocess.run(["docker", "compose", "logs"], check=False)
        raise e
