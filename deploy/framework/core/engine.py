import os
import yaml
from framework.core.registry import ProviderRegistry
from framework.core.context import Context
from framework.utils.resolver import resolve_dict


class DeployEngine:

    def __init__(self, config_path: str):
        base_dir = os.path.dirname(
            os.path.dirname(
                os.path.dirname(__file__)
            )
        )

        full_path = os.path.join(base_dir, config_path)

        if not os.path.exists(full_path):
            raise FileNotFoundError(f"{full_path} not found")

        print(f"📦 Loading services config: {full_path}")

        with open(full_path, encoding="utf-8") as f:
            raw_config = yaml.safe_load(f)

        self.config = resolve_dict(raw_config)
        self.registry = ProviderRegistry()
        self.context = Context()

    def run(self):
        services = self._resolve_dependencies(self.config["services"])

        for svc in services:
            provider = self.registry.get(svc["type"])
            print(f"🚀 Deploying {svc['name']} ({svc['type']})")
            provider.deploy(svc, self.context)

    def _resolve_dependencies(self, services):
        resolved = []
        unresolved = services.copy()

        while unresolved:
            for svc in unresolved[:]:
                deps = svc.get("depends_on", [])

                if all(dep in [s["name"] for s in resolved] for dep in deps):
                    resolved.append(svc)
                    unresolved.remove(svc)

        return resolved
