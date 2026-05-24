import yaml
from framework.config.models import ServicesConfig

import yaml
from pathlib import Path
from framework.config.models import ServicesConfig


def load_services_config(path: str) -> ServicesConfig:
    p = Path(path)

    # 🔥 si no es absoluto, resolver desde raíz del proyecto
    if not p.is_absolute():
        base_dir = Path(__file__).resolve().parents[3]
        p = base_dir / path

    if not p.exists():
        raise FileNotFoundError(f"services.yml not found at: {p}")

    print(f"📦 Loading services config: {p}")

    with open(p, "r") as f:
        raw = yaml.safe_load(f)

    return ServicesConfig(**raw)
