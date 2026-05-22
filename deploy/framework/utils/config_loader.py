from pathlib import Path
import yaml

BASE_DIR = Path(__file__).resolve().parent.parent

def load_services_config():
    config_path = BASE_DIR / "services.yml"

    if not config_path.exists():
        raise FileNotFoundError(f"Missing config file: {config_path}")

    with open(config_path, "r") as f:
        return yaml.safe_load(f)