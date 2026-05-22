from dotenv import load_dotenv
import os


def load_env(environment: str):
    base_dir = os.path.dirname(os.path.dirname(os.path.dirname(__file__)))
    # ↑ sube desde framework/utils → framework → deploy

    env_path = os.path.join(
        base_dir,
        "environments",
        f".env.{environment}"
    )

    if not os.path.exists(env_path):
        raise FileNotFoundError(f"{env_path} not found")

    print(f"📦 Loading env: {env_path}")
    load_dotenv(env_path)
