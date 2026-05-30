import subprocess


def docker_prune():
    print("🧹 Cleaning Docker resources...")

    try:
        # Volúmenes no usados
        subprocess.run(
            ["docker", "volume", "prune", "-f"],
            check=True
        )

        # Redes no usadas
        subprocess.run(
            ["docker", "network", "prune", "-f"],
            check=True
        )

        print("✅ Docker cleanup completed")

    except subprocess.CalledProcessError as ex:
        print(f"❌ Docker prune failed: {ex}")
