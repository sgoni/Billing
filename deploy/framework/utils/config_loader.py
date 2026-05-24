from pathlib import Path
import yaml
from framework.config.models import ServicesConfig

BASE_DIR = Path(__file__).resolve().parent.parent


# def load_services_config():
#    config_path = BASE_DIR / "services.yml"
#
#    if not config_path.exists():
#        raise FileNotFoundError(f"Missing config file: {config_path}")
#
#    with open(config_path, "r") as f:
#        return yaml.safe_load(f)


def load_services_config(path: str) -> ServicesConfig:
    with open(path, "r") as f:
        raw = yaml.safe_load(f)

    return ServicesConfig(**raw)
