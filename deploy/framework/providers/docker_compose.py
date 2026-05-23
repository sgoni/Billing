import subprocess


class DockerComposeProvider:

    def __init__(self, file="docker-compose.yml"):
        self.file = file

    def up(self):
        print("🐳 Starting docker-compose...")
        subprocess.run(
            ["docker-compose", "-f", self.file, "up", "-d"],
            check=True
        )

    def down(self):
        print("🧹 Stopping docker-compose...")
        subprocess.run(
            ["docker-compose", "-f", self.file, "down"],
            check=True
        )
