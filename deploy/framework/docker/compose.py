import subprocess
import os

from framework.utils.docker_prune import docker_prune


class DockerCompose:

    def __init__(self, environment: str):
        self.environment = environment
        self.file = self._resolve_compose_file()

    def _resolve_compose_file(self):
        base_dir = os.path.dirname(os.path.dirname(os.path.dirname(__file__)))

        file_name = f"docker_compose_{self.environment}.yml"

        path = os.path.join(
            base_dir,
            "dockers",
            file_name
        )

        # ⬇️ VALIDACIÓN TEMPRANA (AQUÍ)
        if not os.path.exists(path):
            raise FileNotFoundError(
                f"\n❌ Docker compose not found for env '{self.environment}'\n"
                f"Expected path: {path}\n"
                f"Available files in /dockers:\n"
                f"{os.listdir(os.path.join(base_dir, 'dockers'))}"
            )

        return path

    def up(self):
        cmd = ["docker", "compose", "-f", self.file, "up", "-d"]

        print(f"🐳 Starting docker-compose: {self.file}")
        print("🔧 CMD:", " ".join(cmd))  # ⬅️ AQUÍ

        subprocess.run(cmd, check=True)

    def down(self):
        cmd = ["docker", "compose", "-f", self.file, "down"]

        print(f"🐳 Stopping docker-compose: {self.file}")
        print("🔧 CMD:", " ".join(cmd))  # ⬅️ AQUÍ

        subprocess.run(cmd, check=True)

        docker_prune()
